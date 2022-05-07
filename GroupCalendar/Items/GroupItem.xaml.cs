using GroupCalendar.Core;
using GroupCalendar.View;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

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
            DrawerHost.CloseDrawerCommand.Execute(null, null);
            StaticResources.mainWindow.Frame.Content = new EditGroupPage();
        }
    }
}
