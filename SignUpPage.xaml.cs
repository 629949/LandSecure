using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignUpPage : Page
    {
        private static readonly HttpClient client = new HttpClient();

        public SignUpPage()
        {
            this.InitializeComponent();
        }
        private async void Signup_Click(object sender, RoutedEventArgs e)
        {
            string firstname = FirstNameBox.Text;
            string lastname = LastNameBox.Text;
            string username = usrname.Text;
            string password = pwd.Password;
            string confirmpassword = confirmpwd.Password;

            if (password != confirmpassword)
            {
                await new MessageDialog("Passwords do not match").ShowAsync();
                return;
            }

            var user = new
            {
                FirstName = firstname,
                LastName = lastname,
                Username = username,
                Password = password,
                CreatedAt = DateTime.Now.ToString()
            };

            string json = JsonConvert.SerializeObject(user);

            string firebaseUrl = "https://landapp-2e30d-default-rtdb.firebaseio.com/users.json";

            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            System.Net.Http.HttpResponseMessage response = await client.PostAsync(firebaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                await new MessageDialog("Account created successfully!").ShowAsync();
            }
            else
            {
                await new MessageDialog("Failed to save data to Firebase.").ShowAsync();
            }
        }
    }
}
