using ChatApp.Classes;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace ChatApp
{
    public partial class Login : Window
    {
        private MainWindow MainWindow { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private string Email { get; set; }
        private string Firstname { get; set; }
        private string Lastname { get; set; }
        private bool RememberMe { get; set; }
        public Login()
        {
            InitializeComponent();
            CheckRememberMe();
            _ = TbUsername.Focus();
        }

        private void CheckRememberMe()
        {
            if (ConfigurationManager.AppSettings["rememberMe"].ToString().Equals("true"))
            {
                CbRememberMe.IsChecked = true;
                TbUsername.Text = ConfigurationManager.AppSettings["rememberUsername"].ToString();
                TbPassword.Password = ConfigurationManager.AppSettings["rememberPassword"].ToString();
            }
            else
            {
                CbRememberMe.IsChecked = false;
                TbUsername.Text = "";
                TbPassword.Password = "";
            }
        }

        private async void DoLogin()
        {
            Username = TbUsername.Text;
            Password = TbPassword.Password;
            RememberMe = (bool)CbRememberMe.IsChecked;

            var reqBody = new Dictionary<string, string>
            {
                { "username", Username},
                { "password", Password }
            };
            var response = await Functions.HttpClient.PostAsync($"{ Functions.ServerURL }login", new FormUrlEncodedContent(reqBody));
            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);
            string message = json.Property("message").Value.ToString();

            if (message.Equals("Valid password"))
            {
                if (RememberMe)
                {
                    Functions.UpdateSetting("rememberMe", "true");
                    Functions.UpdateSetting("rememberUsername", Username);
                    Functions.UpdateSetting("rememberPassword", Password);
                }
                else
                {
                    Functions.UpdateSetting("rememberMe", "false");
                    Functions.UpdateSetting("rememberUsername", "");
                    Functions.UpdateSetting("rememberPassword", "");
                }

                JObject userInfo = JObject.Parse(json.Property("user").Value.ToString());
                Functions.UpdateSetting("userID", userInfo.Property("_id").Value.ToString());
                Functions.UpdateSetting("username", Username);
                Functions.UpdateSetting("email", userInfo.Property("Email").Value.ToString());
                Functions.UpdateSetting("firstname", userInfo.Property("Firstname").Value.ToString());
                Functions.UpdateSetting("lastname", userInfo.Property("Lastname").Value.ToString());

                Hide();
                MainWindow = new MainWindow();
                MainWindow.Show();
            }
            else
            {
                MessageBox.Show(message, "Pluco chat", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private async void DoRegister()
        {
            Username = TbUsername.Text;
            Password = TbPassword.Password;
            Email = TbEmail.Text;
            Firstname = TbFirstname.Text;
            Lastname = TbLastname.Text;

            var reqBody = new Dictionary<string, string>
            {
                { "username", Username},
                { "password", Password },
                { "email", Email },
                { "firstname", Firstname },
                { "lastname", Lastname }
            };
            var response = await Functions.HttpClient.PostAsync($"{ Functions.ServerURL }signup", new FormUrlEncodedContent(reqBody));
            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);
            string message = json.Property("message").Value.ToString();

            if (message.ToLower().Equals("true"))
            {
                MessageBox.Show("A new account successfully created!", "Pluco chat", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                SwitchToLoginForm();
            }
            else if (message.ToLower().Equals("false"))
            {
                MessageBox.Show("Error occured while network transmission!", "Pluco chat", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
            }
            else
            {
                MessageBox.Show(message, "Pluco chat", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            DoLogin();
        }

        private void TbUsername_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (Title.Equals("Login"))
                        DoLogin();
                    else
                        DoRegister();
                    break;
                default: break;
            }
        }

        private void TbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (Title.Equals("Login")) 
                        DoLogin();
                    else
                        DoRegister();
                    break;
                default: break;
            }
        }

        private void TbEmail_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (Title.Equals("Register"))
                        DoRegister();
                    break;
                default: break;
            }
        }

        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Title = "Register";
            RegisterPanel.Visibility = Visibility.Visible;
            CbRememberMe.Visibility = Visibility.Collapsed;
            BtnLogin.Visibility = Visibility.Collapsed;
            BtnSignUp.Visibility = Visibility.Collapsed;
            BtnCreateAccount.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Visible;

            TbUsername.Text = "";
            TbPassword.Password = "";
            TbEmail.Text = "";
            TbFirstname.Text = "";
            TbLastname.Text = "";
            _ = TbUsername.Focus();
        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (Title.Equals("Register"))
                DoRegister();
        }

        private void SwitchToLoginForm()
        {
            Title = "Login";
            RegisterPanel.Visibility = Visibility.Collapsed;
            CbRememberMe.Visibility = Visibility.Visible;
            BtnLogin.Visibility = Visibility.Visible;
            BtnSignUp.Visibility = Visibility.Visible;
            BtnCreateAccount.Visibility = Visibility.Collapsed;
            BtnBack.Visibility = Visibility.Collapsed;

            TbUsername.Text = "";
            TbPassword.Password = "";
            TbEmail.Text = "";
            TbFirstname.Text = "";
            TbLastname.Text = "";
            _ = TbUsername.Focus();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            SwitchToLoginForm();
        }
    }
}
