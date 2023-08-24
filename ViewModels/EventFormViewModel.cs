using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Calendar.Models;
using System.Windows;
using System.Linq;

namespace Calendar.ViewModels
{

    public class EventFormViewModel : ViewModelBase
    {

        public EventFormWindow _eventFormWindow;
        public EventModel NewEvent { get; set; }
        public ObservableCollection<EventModel> Events { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }


        public EventFormViewModel(ObservableCollection<EventModel> events, EventFormWindow eventFormWindow)
        {

            Events = events;
            SaveCommand = new RelayCommand(param => SaveEvent());
            CancelCommand = new RelayCommand(param => Cancel());
            _eventFormWindow = eventFormWindow;
            NewEvent = new EventModel
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };
            InitializeTimeComboBoxes();

        }


        public EventFormViewModel(EventModel existingEvent, ObservableCollection<EventModel> events, EventFormWindow eventFormWindow)
        {

            Events = events;
            SaveCommand = new RelayCommand(param => SaveEvent());
            CancelCommand = new RelayCommand(param => Cancel());
            _eventFormWindow = eventFormWindow;
            NewEvent = new EventModel
            {
                ID = existingEvent.ID,
                StartDate = existingEvent.StartDate,
                EndDate = existingEvent.EndDate,
                Title = existingEvent.Title,
                Description = existingEvent.Description,
                Notification = existingEvent.Notification
            };
            InitializeTimeComboBoxes();
            SetSelectedHoursAndMinutes();

        }


        //save event and close window -> go back to the list of events
        private void SaveEvent()
        {

            //validate user input
            if (string.IsNullOrWhiteSpace(_eventFormWindow.txtTitle.Text) || _eventFormWindow.datePickerStart.SelectedDate == null 
                || _eventFormWindow.datePickerEnd.SelectedDate == null ||
                _eventFormWindow.cmbEndHours == null || _eventFormWindow.cmbEndMinutes == null || _eventFormWindow.cmbStartHours == null 
                || _eventFormWindow.cmbStartMinutes == null)
            {

                MessageBox.Show("Please fill in all required fields correctly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            if (NewEvent.ID == 0)
            {

                NewEvent = new EventModel
                {
                    ID = GetNextEventID(),
                    Title = _eventFormWindow.txtTitle.Text,
                    Description = _eventFormWindow.txtDescription.Text,
                    StartDate = new DateTime(_eventFormWindow.datePickerStart.SelectedDate.Value.Year, _eventFormWindow.datePickerStart.SelectedDate.Value.Month, 
                    _eventFormWindow.datePickerStart.SelectedDate.Value.Day,
                    (int)_eventFormWindow.cmbStartHours.SelectedItem, (int)_eventFormWindow.cmbStartMinutes.SelectedItem, 0),
                    EndDate = new DateTime(_eventFormWindow.datePickerEnd.SelectedDate.Value.Year, _eventFormWindow.datePickerEnd.SelectedDate.Value.Month, 
                    _eventFormWindow.datePickerEnd.SelectedDate.Value.Day,
                    (int)_eventFormWindow.cmbEndHours.SelectedItem, (int)_eventFormWindow.cmbEndMinutes.SelectedItem, 0),
                    Notification = _eventFormWindow.chkNotification.IsChecked ?? false
                };

            }

            else
            {

                NewEvent.Title = _eventFormWindow.txtTitle.Text;
                NewEvent.Description = _eventFormWindow.txtDescription.Text;
                NewEvent.StartDate = new DateTime(_eventFormWindow.datePickerStart.SelectedDate.Value.Year, _eventFormWindow.datePickerStart.SelectedDate.Value.Month, 
                    _eventFormWindow.datePickerStart.SelectedDate.Value.Day,
                (int)_eventFormWindow.cmbStartHours.SelectedItem, (int)_eventFormWindow.cmbStartMinutes.SelectedItem, 0);
                NewEvent.EndDate = new DateTime(_eventFormWindow.datePickerEnd.SelectedDate.Value.Year, _eventFormWindow.datePickerEnd.SelectedDate.Value.Month,
                    _eventFormWindow.datePickerEnd.SelectedDate.Value.Day,
                (int)_eventFormWindow.cmbEndHours.SelectedItem, (int)_eventFormWindow.cmbEndMinutes.SelectedItem, 0);
                NewEvent.Notification = _eventFormWindow.chkNotification.IsChecked ?? false;

            }

            if (NewEvent.StartDate > NewEvent.EndDate)
            {

                MessageBox.Show("Start Date cannot be greater than End Date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            _eventFormWindow.NewEvent = NewEvent;
            _eventFormWindow.DialogResult = true;
            _eventFormWindow.Close();

        }


        private void Cancel()
        {

            _eventFormWindow.DialogResult = false;
            _eventFormWindow.Close();

        }


        private void SetSelectedHoursAndMinutes()
        {

            _eventFormWindow.cmbStartHours.SelectedItem = NewEvent.StartDate.Hour;
            _eventFormWindow.cmbStartMinutes.SelectedItem = NewEvent.StartDate.Minute;
            _eventFormWindow.cmbEndHours.SelectedItem = NewEvent.EndDate.Hour;
            _eventFormWindow.cmbEndMinutes.SelectedItem = NewEvent.EndDate.Minute;

        }


        //time comboboxes initialization
        public void InitializeTimeComboBoxes()
        {

            for (int hour = 0; hour < 24; hour++)
            {

                _eventFormWindow.cmbStartHours.Items.Add(hour);
                _eventFormWindow.cmbEndHours.Items.Add(hour);

            }

            for (int minute = 0; minute < 60; minute++)
            {

                _eventFormWindow.cmbStartMinutes.Items.Add(minute);
                _eventFormWindow.cmbEndMinutes.Items.Add(minute);

            }

        }


        //get id for new event
        private int GetNextEventID()
        {

            if (Events.Count == 0)
                return 1;

            int maxID = Events.Max(ev => ev.ID);
            return maxID + 1;

        }


    }

}