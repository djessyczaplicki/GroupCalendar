using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;
using GroupCalendar.Core;
using GroupCalendar.Data.Network;
using GroupCalendar.View;
using System.Windows;

namespace GroupCalendar
{

    public partial class MainWindow : Window
    {
        private bool loginUIShowing = true;

        public MainWindow()
        {
            StaticResources.mainWindow = this;
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
                    Success(e);
                }
            });
        }

        private async void Success(UserEventArgs e)
        {
            WindowStyle = WindowStyle.None;
            ApplicationState.SetValue("token", e.User.Credential.IdToken);
            var user = await Repository.GetUserByIdAsync(e.User.Uid);
            if (user.Groups.Length == 0)
            {
                // TODO: Control here if user has no group, and take the user to group creation
                return;
            }
            ApplicationState.SetValue("group_id", user.Groups[0]);
            HideLoginUI();
        }


        private void HideLoginUI()
        {
            Frame.Content = new TimetablePage();
            loginUIShowing = false;
        }

        private void ShowLoginUi()
        {
            Frame.Visibility = Visibility.Visible;
            loginUIShowing = true;
        }
    }
}
