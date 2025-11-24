using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
                

                ContentDialog successDialog = new ContentDialog
                {
                    Title = "Success!",
                    Content = $"Verification request submitted successfully!\n\nProperty ID: {formData.PropertyId}\nOwner: {formData.OwnerName}\n\nYou will receive updates via {formData.Contact}",
                    CloseButtonText = "OK"
                };
                await successDialog.ShowAsync();

                // Navigate back or to confirmation page
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

        private void PropertyIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

     
        private void clearform(object sender, RoutedEventArgs e)
        {
           

        }
    }
}