using GroupCalendar.Core;
using System.Windows.Controls;

namespace GroupCalendar.View
{
    /// <summary>
    /// Lógica de interacción para EventPage.xaml
    /// </summary>
    public partial class EventPage : Page
    {
        public EventPage()
        {
            InitializeComponent();
        }

        private void BackButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplicationState.PurgeValue("event_id");
            StaticResources.mainWindow.Frame.Content = new TimetablePage();
        }

        private void EditEventClick(object sender, System.Windows.RoutedEventArgs e)
        {
            StaticResources.mainWindow.Frame.Content = new EditEventPage();
        }
    }
}
