using System.Collections.ObjectModel;
using System.Windows;
using Calendar.Models;
using Calendar.ViewModels;

namespace Calendar
{

    public partial class EventFormWindow : Window
    {

        public EventModel NewEvent { get; set; }


        public EventFormWindow(ObservableCollection<EventModel> events)
        {

            InitializeComponent();
            DataContext = new EventFormViewModel(events, this);

        }


        public EventFormWindow(EventModel existingEvent, ObservableCollection<EventModel> events)
        {

            InitializeComponent();
            DataContext = new EventFormViewModel(existingEvent, events, this);

        }

    }

}