using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace Calendar
{

    public partial class EventListWindow : Window
    {

        public ObservableCollection<Event> SelectedEvents { get; set; }
        public ObservableCollection<Event> Events { get; set; }
        DateTime SelectedDate { get; set; }
        private string EventsFilePath { get; set; }

        public EventListWindow(DateTime _selectedDate, ObservableCollection<Event> _events, string _eventsFilePath)
        {

            InitializeComponent();
            SelectedDate = _selectedDate;
            Events = _events;
            SelectEvents();
            EventsFilePath = _eventsFilePath;
            DataContext = this;

        }


        //add new event
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            var eventFormWindow = new EventFormWindow(Events);
            eventFormWindow.Owner = this;
            //hide the current window
            this.Visibility = Visibility.Collapsed;

            if (eventFormWindow.ShowDialog() == true)
            {

                if (SelectedDate >= eventFormWindow.NewEvent.StartDate && SelectedDate <= eventFormWindow.NewEvent.EndDate)
                    SelectedEvents.Add(eventFormWindow.NewEvent);

                Events.Add(eventFormWindow.NewEvent);
                SaveEvents();
            
            }

        }


        //update the event
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

            if (eventListBox.SelectedItem is Event selectedEvent)
            {

                var eventFormWindow = new EventFormWindow(selectedEvent, Events);
                eventFormWindow.Owner = this;
                //hide the current window
                this.Visibility = Visibility.Collapsed;

                if (eventFormWindow.ShowDialog() == true)
                {

                    int index = Events.IndexOf(selectedEvent);
                    Events[index] = eventFormWindow.NewEvent;
                    if (SelectedDate >= eventFormWindow.NewEvent.StartDate && SelectedDate <= eventFormWindow.NewEvent.EndDate)
                    {

                        index = SelectedEvents.IndexOf(selectedEvent);
                        SelectedEvents[index] = eventFormWindow.NewEvent;

                    }

                    SaveEvents();

                }

            }

        }


        //delete the event 
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (eventListBox.SelectedItem is Event selectedEvent)
            {

                var result = MessageBox.Show("Are you sure you want to delete this event?", "Confirmation", 
                                            MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                 
                    SelectedEvents.Remove(selectedEvent);
                    Events.Remove(selectedEvent);
                    SaveEvents();

                }

            }

        }


        //select events for selected date
        private void SelectEvents()
        {
            SelectedEvents = new ObservableCollection<Event>(Events.Where(ev => ev.StartDate.Date <= SelectedDate.Date &&
                                                                        ev.EndDate.Date >= SelectedDate.Date));
        }


        //save events in JSON file
        private void SaveEvents()
        {

            var jsonString = JsonSerializer.Serialize(Events);
            File.WriteAllText(EventsFilePath, jsonString);

        }


        //go back to the main window
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = false;
            Close();

        }

    }

}