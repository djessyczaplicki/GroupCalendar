using GroupCalendar.ViewModel.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupCalendar.Items
{
    /// <summary>
    /// Lógica de interacción para UserItem.xaml
    /// </summary>
    public partial class UserItem : UserControl
    {

        public string Role
        {
            get { return (string)GetValue(RoleProperty); }
            set { SetValue(RoleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Role.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(string), typeof(UserItem), new PropertyMetadata(""));


        public string FullNameYou
        {
            get { return (string)GetValue(FullNameYouProperty); }
            set { SetValue(FullNameYouProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FullNameYou.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FullNameYouProperty =
            DependencyProperty.Register("FullNameYou", typeof(string), typeof(UserItem), new PropertyMetadata(""));




        public string UserId
        {
            get { return (string)GetValue(UserIdProperty); }
            set { SetValue(UserIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserIdProperty =
            DependencyProperty.Register("UserId", typeof(string), typeof(UserItem), new PropertyMetadata(""));



        public ICommand MakeAdmin
        {
            get { return (ICommand)GetValue(MakeAdminProperty); }
            set { SetValue(MakeAdminProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MakeAdmin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MakeAdminProperty =
            DependencyProperty.Register("MakeAdmin", typeof(ICommand), typeof(UserItem), new PropertyMetadata(new RelayCommand(o => o.ToString())));



        public ICommand RemoveAdmin
        {
            get { return (ICommand)GetValue(RemoveAdminProperty); }
            set { SetValue(RemoveAdminProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveAdmin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveAdminProperty =
            DependencyProperty.Register("RemoveAdmin", typeof(ICommand), typeof(UserItem), new PropertyMetadata(new RelayCommand(o => o.ToString())));



        public ICommand RemoveUser
        {
            get { return (ICommand)GetValue(RemoveUserProperty); }
            set { SetValue(RemoveUserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveUser.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveUserProperty =
            DependencyProperty.Register("RemoveUser", typeof(ICommand), typeof(UserItem), new PropertyMetadata(new RelayCommand(o => o.ToString())));


        public UserItem()
        {
            InitializeComponent();
        }
    }
}
