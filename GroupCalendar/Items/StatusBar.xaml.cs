using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupCalendar.Items
{
    /// <summary>
    /// Lógica de interacción para StatusBar.xaml
    /// </summary>
    public partial class StatusBar : UserControl
    {

        public StatusBar()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void StatusBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StaticResources.mainWindow.DragMove();
            }
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            StaticResources.mainWindow.WindowState = WindowState.Minimized;
        }

        private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            StaticResources.mainWindow.Close();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            if (StaticResources.mainWindow.WindowState == WindowState.Maximized)
            {
                StaticResources.mainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                StaticResources.mainWindow.WindowState = WindowState.Maximized;
            }
        }
    }
}
