using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using HttpClient = System.Net.Http.HttpClient;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LandSecure
{
   
    public class VerificationForm
    {
        public int Id { get; set; }
        public string PropertyId { get; set; }
        public string Location { get; set; }
        public string PropertyType { get; set; }
        public string Size { get; set; }
        public string OwnerName { get; set; }
        public string NationalId { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string TitleDeedPath { get; set; }
        public string IdCopyPath { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }

    }

    public sealed partial class Verifying : Page
    {

        private int currentStep = 1;

        private VerificationForm formData;
        private static readonly HttpClient client = new HttpClient();
        public Verifying()
        {
            this.InitializeComponent(); 
            
            formData = new VerificationForm
            {
                CreatedDate = DateTime.Now,
                Status = "Pending"
            };

            // Set DataContext for binding
            this.DataContext = formData;
        }

        private void nextbtn(object sender, RoutedEventArgs e)
        {
            // Validate current step
            if (!ValidateCurrentStep())
            {
                return;
            }

            if (currentStep < 3)
            {
                currentStep++;
                UpdateStepVisibility();
            }
            else
            {
                // Final step - submit to database
                SubmitToDatabase();
            }
        }

        private void backbtn(object sender, RoutedEventArgs e)
        {
            if (currentStep > 1)
            {
                currentStep--;
                UpdateStepVisibility();
            }
        }

        private void UpdateStepVisibility()
        {
           
            Step1Panel.Visibility = Visibility.Collapsed;
            Step2Panel.Visibility = Visibility.Collapsed;
            Step3Panel.Visibility = Visibility.Collapsed;

            switch (currentStep)
            {
                case 1:
                    Step1Panel.Visibility = Visibility.Visible;
                    StepIndicator.Text = "Step 1 of 3: Property Details";
                    ProgressBar.Value = 33;
                    BackBtn.Visibility = Visibility.Collapsed;
                    NextBtn.Content = "Next";
                    break;
                case 2:
                    Step2Panel.Visibility = Visibility.Visible;
                    StepIndicator.Text = "Step 2 of 3: Owner Information";
                    ProgressBar.Value = 66;
                    BackBtn.Visibility = Visibility.Visible;
                    NextBtn.Content = "Next";
                    break;
                case 3:
                    Step3Panel.Visibility = Visibility.Visible;
                    StepIndicator.Text = "Step 3 of 3: Upload Documents";
                    ProgressBar.Value = 100;
                    BackBtn.Visibility = Visibility.Visible;
                    NextBtn.Content = "Submit";
                    break;
            }
        }

        private bool ValidateCurrentStep()
        {
            switch (currentStep)
            {
                case 1:
                    if (string.IsNullOrWhiteSpace(formData.PropertyId))
                    {
                        ShowError("Please enter Property ID");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(formData.Location))
                    {
                        ShowError("Please enter Location");
                        return false;
                    }
                    break;
                case 2:
                    if (string.IsNullOrWhiteSpace(formData.OwnerName))
                    {
                        ShowError("Please enter Owner Name");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(formData.Contact))
                    {
                        ShowError("Please enter Contact Number");
                        return false;
                    }
                    break;
            }
            return true;
        }

        private async void ShowError(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Validation Error",
                Content = message,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }

        private async void UploadTitleDeed_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                formData.TitleDeedPath = file.Path;
                TitleDeedStatus.Text = $"✅ {file.Name}";
                TitleDeedStatus.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(
                    Windows.UI.Colors.Green);
            }
        }

        private async void UploadIdCopy_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                formData.IdCopyPath = file.Path;
                IdCopyStatus.Text = $"✅ {file.Name}";
                IdCopyStatus.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(
                    Windows.UI.Colors.Green);
            }
        }

        private async void SubmitToDatabase()
        {
            try
            {
                // Step 3 validation (ensure files uploaded)
                if (TitleDeedStatus.Text == "No file selected" || IdCopyStatus.Text == "No file selected")
                {
                    await new MessageDialog("Please upload required documents.").ShowAsync();
                    return;
                }

                // Build verification data object
                var verificationData = new
                {
                    PropertyId = PropertyIdBox.Text,
                    Location = LocationBox.Text,
                    PropertyType = (PropertyTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Size = SizeBox.Text,

                    OwnerName = OwnerNameBox.Text,
                    NationalId = identification.Text,
                    Contact = contactinfo.Text,
                    Email = emailaddress.Text,

                    TitleDeedFile = TitleDeedStatus.Text,
                    IdCopyFile = IdCopyStatus.Text,
                    Notes = NotesBox.Text,

                    Status = "Pending Verification",
                    SubmittedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Firebase Realtime DB path
                string json = JsonConvert.SerializeObject(verificationData);

                string firebaseUrl = "https://landapp-2e30d-default-rtdb.firebaseio.com/verifications.json";

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                System.Net.Http.HttpResponseMessage response = await client.PostAsync(firebaseUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    await new MessageDialog("Failed to submit verification request.").ShowAsync();
                    return;
                }

                // Success dialog
                ContentDialog successDialog = new ContentDialog
                {
                    Title = "Success!",
                    Content = "Verification request submitted successfully.\nYou will receive updates soon.",
                    CloseButtonText = "OK"
                };

                await successDialog.ShowAsync();

                Frame.Navigate(typeof(HomePage));
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to submit: {ex.Message}",
                    CloseButtonText = "OK"
                };
                await errorDialog.ShowAsync();
            }
        }



        private void OpenLoginPopup_Click(object sender, RoutedEventArgs e)
        {
            LoginPopupOverlay.Visibility = Visibility.Visible;
        }

        private void CloseLoginPopup_Click(object sender, RoutedEventArgs e)
        {
            LoginPopupOverlay.Visibility = Visibility.Collapsed;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginUsernameBox.Text.Trim();
            string password = LoginPasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await new MessageDialog("Please enter username and password.").ShowAsync();
                return;
            }

            string firebaseUrl = "https://landapp-2e30d-default-rtdb.firebaseio.com/users.json";

            var response = await client.GetAsync(new Uri(firebaseUrl));

            if (!response.IsSuccessStatusCode)
            {
                await new MessageDialog("Unable to connect to server!").ShowAsync();
                return;
            }

            string jsonData = await response.Content.ReadAsStringAsync();

            var usersDict = JsonConvert.DeserializeObject<Dictionary<string, UserModel>>(jsonData);

            if (usersDict == null)
            {
                await new MessageDialog("No users found in database.").ShowAsync();
                return;
            }

            var user = usersDict.Values.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            if (user != null)
            {
                // SUCCESS 🎉
                LoginPopupOverlay.Visibility = Visibility.Collapsed;

                await new MessageDialog($"Welcome {user.FirstName}!").ShowAsync();
            }
            else
            {
                // WRONG LOGIN ❌
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Login Failed",
                    Content = "Incorrect username or password.",
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }


        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignUpPage));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoginPopupOverlay.Visibility = Visibility.Visible;

            // allow interaction ONLY with the popup
            LoginPopupOverlay.IsHitTestVisible = true;
        }

        private void PropertyIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

     
        private void clearform(object sender, RoutedEventArgs e)
        {
           

        }
    }
}