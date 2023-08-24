using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Calendar.Models;
using System.Windows;
using System.Text.Json;

namespace Calendar.ViewModels
{

    public class EventListViewModel : ViewModelBase
    {

        private EventListWindow _eventListWindow;
        public ObservableCollection<EventModel> SelectedEvents { get; set; }
        public ObservableCollection<EventModel> Events { get; set; }
        public DateTime SelectedDate { get; set; }
        public string EventsFilePath { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand GoBackCommand { get; }


        public EventListViewModel(DateTime selectedDate, ObservableCollection<EventModel> events, string eventsFilePath, EventListWindow eventListWindow)
        {

            _eventListWindow = eventListWindow;
            SelectedDate = selectedDate;
            Events = events;
            SelectedEvents = new ObservableCollection<EventModel>(Events.Where(ev => ev.StartDate.Date <= SelectedDate.Date && ev.EndDate.Date >= SelectedDate.Date));
            EventsFilePath = eventsFilePath;
            AddCommand = new RelayCommand(param => AddEvent());
            EditCommand = new RelayCommand(param => EditEvent());
            DeleteCommand = new RelayCommand(param => DeleteEvent());
            GoBackCommand = new RelayCommand(param => GoBack());

        }


        //add new event
        private void AddEvent()
        {

            var eventFormWindow = new EventFormWindow(Events);
            eventFormWindow.Owner = _eventListWindow;
            //hide the current window
            _eventListWindow.Visibility = Visibility.Collapsed;

            if (eventFormWindow.ShowDialog() == true)
            {

                if (SelectedDate >= eventFormWindow.NewEvent.StartDate && SelectedDate <= eventFormWindow.NewEvent.EndDate)
                    SelectedEvents.Add(eventFormWindow.NewEvent);

                Events.Add(eventFormWindow.NewEvent);
                SaveEvents();

            }

        }


        //update the event
        private void EditEvent()
        {

            if (_eventListWindow.eventListBox.SelectedItem is EventModel selectedEvent)
            {

                var eventFormWindow = new EventFormWindow(selectedEvent, Events);
                eventFormWindow.Owner = _eventListWindow;
                //hide the current window
                _eventListWindow.Visibility = Visibility.Collapsed;

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
        private void DeleteEvent()
        {

            if (_eventListWindow.eventListBox.SelectedItem is EventModel selectedEvent)
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


        //go back to the main window
        private void GoBack()
        {

            _eventListWindow.DialogResult = false;
            _eventListWindow.Close();

        }


        //save events in JSON file
        private void SaveEvents()
        {

            var jsonString = JsonSerializer.Serialize(Events);
            File.WriteAllText(EventsFilePath, jsonString);

        }

    }

}