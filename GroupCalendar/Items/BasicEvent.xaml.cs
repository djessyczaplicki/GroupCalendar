using GroupCalendar.Core;
using GroupCalendar.Data.Remote.Model;
using GroupCalendar.View;
using System.Windows.Controls;
using System.Windows.Media;

namespace GroupCalendar.Items
{
    /// <summary>
    /// Lógica de interacción para BasicEvent.xaml
    /// </summary>
    public partial class BasicEvent : UserControl
    {
        public EventModel EventModel { get; internal set; }

        public BasicEvent(EventModel eventModel)
        {
            InitializeComponent();
            EventModel = eventModel;
            EventName.Text = eventModel.Name;
            EventDescription.Text = eventModel.Description;
            EventBackground.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(eventModel.Color.ToString());
        }

        private void EventBackgroundMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplicationState.SetValue("event_id", EventModel.Id);
            StaticResources.mainWindow.Frame.Content = new EventPage();
        }
    }
}
