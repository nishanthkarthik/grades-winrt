using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GradeSharp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Login();
        }

        async Task Login()
        {
            if (String.IsNullOrEmpty(RollBox.Text) || String.IsNullOrEmpty(PasswordBox.Password))
            {
                Notify("Empty field", "Please fill in all the fields");
                return;
            }
            ProgressRing.IsActive = true;
            HttpResponseMessage httpResponse = await AttemptLoginClient();
            ProgressRing.IsActive = false;
            if (httpResponse.RequestMessage.RequestUri.AbsoluteUri == "https://www.iitm.ac.in/viewgrades/student.html")
            {
                Notify("Wrong credentials", "Please try again with valid credentials");
                return;
            }
            if (httpResponse.RequestMessage.RequestUri.AbsoluteUri == "https://www.iitm.ac.in/viewgrades/studentauth/studpass.php")
            {
                Frame.Navigate(typeof(GradePage), httpResponse);
            }
        }

        async Task<WebResponse> AttemptLogin()
        {
            try
            {
                var webRequest = WebRequest.Create("https://www.iitm.ac.in/viewgrades/studentauth/login.php");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Proxy = null;
                using (var writer = new StreamWriter(await webRequest.GetRequestStreamAsync()))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("rollno=");
                    stringBuilder.Append(Uri.EscapeDataString(RollBox.Text));
                    stringBuilder.Append("&pwd=");
                    stringBuilder.Append(Uri.EscapeDataString(PasswordBox.Password));
                    stringBuilder.Append("submit=Submit");
                    await writer.WriteAsync(stringBuilder.ToString());
                }
                WebResponse loginResponse = await webRequest.GetResponseAsync();
                return loginResponse;
            }
            catch (Exception exception)
            {
                Notify("Exception", exception.Message);
                throw;
            }
        }

        async Task<HttpResponseMessage> AttemptLoginClient()
        {
            try
            {
                List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("rollno", RollBox.Text),
                    new KeyValuePair<string, string>("pwd", PasswordBox.Password),
                    new KeyValuePair<string, string>("submit", "Submit")
                };
                HttpContent httpContent = new FormUrlEncodedContent(keyValuePairs);
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync("https://www.iitm.ac.in/viewgrades/studentauth/login.php", httpContent);
                return response;
            }
            catch (Exception exception)
            {
                Notify("Exception", exception.Message);
                throw;
            }
        }

        async void Notify(string title, string text)
        {
            MessageDialog message = new MessageDialog(text, title);
            await message.ShowAsync();
        }

    }
}
