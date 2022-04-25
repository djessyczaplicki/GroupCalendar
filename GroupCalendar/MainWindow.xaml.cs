using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;
using GroupCalendar.Core;
using System;
using System.Windows;

namespace GroupCalendar
{

    public partial class MainWindow : Window
    {
        private bool loginUIShowing = true;

        public MainWindow()
        {
            FirebaseUI.Initialize(new FirebaseUIConfig
            {
                ApiKey = "AIzaSyBUAAkXJdnTrSWSAh1mW5fXrPwiw3fI-dY",
                AuthDomain = "groupcalendar-53829.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider(),
                    new GoogleProvider().AddScopes("email")
                    // and others
                },
                PrivacyPolicyUrl = "https://example.org",
                TermsOfServiceUrl = "https://example.org",
                IsAnonymousAllowed = false,
                UserRepository = new FileUserRepository("FirebaseSample") // persist data into %AppData%\FirebaseSample
            });
            InitializeComponent();

            FirebaseUI.Instance.Client.AuthStateChanged += AuthStateChanged;
        }

        private void AuthStateChanged(object? sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e.User == null || e.User.Credential.IsExpired())
                {
                    ShowLoginUi();
                }
                else if (loginUIShowing)
                {
                    HideLoginUI();
                    LoginButton.Content = e!.User!.Info.Uid + " " + e.User.Info.Email;
                    ApplicationState.SetValue("token", e.User.Credential.IdToken);
                }
            });
        }

        private void HideLoginUI()
        {
            Frame.Source = new Uri("View/EventPage.xaml", UriKind.Relative);
            loginUIShowing = false;
        }

        private void ShowLoginUi()
        {
            Frame.Visibility = Visibility.Visible;
            loginUIShowing = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
