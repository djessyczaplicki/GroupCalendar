using GroupCalendar.Data.Remote.Model;
using System.Collections.Generic;
using System.Windows;

namespace GroupCalendar.ViewModel.DependencyObjects
{
    public class EventModelDO : DependencyObject
    {
        public List<EventModel> EventModels
        {
            get { return (List<EventModel>)GetValue(EventModelsProperty); }
            set { SetValue(EventModelsProperty, value); }
        }
        public static readonly DependencyProperty EventModelsProperty =
            DependencyProperty.Register("eventModels", typeof(List<EventModel>), typeof(EventModelDO), new PropertyMetadata(new List<EventModel>()));

    }
}
