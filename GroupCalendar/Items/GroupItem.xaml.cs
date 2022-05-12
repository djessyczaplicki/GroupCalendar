using GroupCalendar.Core;
using GroupCalendar.View;
using GroupCalendar.ViewModel.Commands;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupCalendar.Items
{
    /// <summary>
    /// Lógica de interacción para GroupItem.xaml
    /// </summary>
    public partial class GroupItem : UserControl
    {

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(GroupItem), new PropertyMetadata(string.Empty));



        public string GroupImage
        {
            get { return (string)GetValue(GroupImageProperty); }
            set { SetValue(GroupImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupImageProperty =
            DependencyProperty.Register("GroupImage", typeof(string), typeof(GroupItem), new PropertyMetadata(""));





        public bool IsAdmin
        {
            get { return (bool)GetValue(IsAdminProperty); }
            set { SetValue(IsAdminProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAdmin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAdminProperty =
            DependencyProperty.Register("IsAdmin", typeof(bool), typeof(GroupItem), new PropertyMetadata(false));





        public ICommand LeaveGroupCommand
        {
            get { return (ICommand)GetValue(LeaveGroupCommandProperty); }
            set { SetValue(LeaveGroupCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeaveGroupCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeaveGroupCommandProperty =
            DependencyProperty.Register("LeaveGroupCommand", typeof(ICommand), typeof(GroupItem), new PropertyMetadata(new RelayCommand(o => o.ToString())));




        public Guid GroupId
        {
            get { return (Guid)GetValue(GroupIdProperty); }
            set { SetValue(GroupIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupIdProperty =
            DependencyProperty.Register("GroupId", typeof(Guid), typeof(GroupItem), new PropertyMetadata(new Guid()));



        public GroupItem()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationState.SetValue("group_id", GroupId);
            ApplicationState.SetValue("is_editing", true);
            DrawerHost.CloseDrawerCommand.Execute(null, null);
            StaticResources.mainWindow.Frame.Content = new EditGroupPage();
        }
    }
}
