using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;


namespace Calendar
{

    public partial class EventFormWindow : Window
    {

        public Event NewEvent { get; set; }
        private ObservableCollection<Event> Events;


        public EventFormWindow(ObservableCollection<Event> events)
        {

            InitializeComponent();
            Events = events;
            NewEvent = new Event
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };
            DataContext = this;
            InitializeTimeComboBoxes();

        }


        public EventFormWindow(Event existingEvent, ObservableCollection<Event> events)
        {

            InitializeComponent();
            Events = events;
            NewEvent = new Event
            {
                ID = existingEvent.ID,
                StartDate = existingEvent.StartDate,
                EndDate = existingEvent.EndDate,
                Title = existingEvent.Title,
                Description = existingEvent.Description,
                Notification = existingEvent.Notification
            };

            DataContext = this;
            InitializeTimeComboBoxes();
            SetSelectedHoursAndMinutes();

        }


        private void SetSelectedHoursAndMinutes()
        {

            cmbStartHours.SelectedItem = NewEvent.StartDate.Hour;
            cmbStartMinutes.SelectedItem = NewEvent.StartDate.Minute;
            cmbEndHours.SelectedItem = NewEvent.EndDate.Hour;
            cmbEndMinutes.SelectedItem = NewEvent.EndDate.Minute;

        }


        //time comboboxes initialization
        public void InitializeTimeComboBoxes()
        {

            for (int hour = 0; hour < 24; hour++)
            {

                cmbStartHours.Items.Add(hour);
                cmbEndHours.Items.Add(hour);

            }

            for (int minute = 0; minute < 60; minute++)
            {

                cmbStartMinutes.Items.Add(minute);
                cmbEndMinutes.Items.Add(minute);

            }

        }

        //save event and close window -> go back to the list of events
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            //validate user input
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || datePickerStart.SelectedDate == null || datePickerEnd.SelectedDate == null ||
                cmbEndHours == null || cmbEndMinutes == null || cmbStartHours == null || cmbStartMinutes == null)
            {

                MessageBox.Show("Please fill in all required fields correctly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            if (NewEvent.ID == 0)
            {

                NewEvent = new Event
                {
                    ID = GetNextEventID(),
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    StartDate = new DateTime(datePickerStart.SelectedDate.Value.Year, datePickerStart.SelectedDate.Value.Month, datePickerStart.SelectedDate.Value.Day,
                    (int)cmbStartHours.SelectedItem, (int)cmbStartMinutes.SelectedItem, 0),
                    EndDate = new DateTime(datePickerEnd.SelectedDate.Value.Year, datePickerEnd.SelectedDate.Value.Month, datePickerEnd.SelectedDate.Value.Day,
                    (int)cmbEndHours.SelectedItem, (int)cmbEndMinutes.SelectedItem, 0),
                    Notification = chkNotification.IsChecked ?? false
                };

            }

            else
            {

                NewEvent.Title = txtTitle.Text;
                NewEvent.Description = txtDescription.Text;
                NewEvent.StartDate = new DateTime(datePickerStart.SelectedDate.Value.Year, datePickerStart.SelectedDate.Value.Month, datePickerStart.SelectedDate.Value.Day,
                (int)cmbStartHours.SelectedItem, (int)cmbStartMinutes.SelectedItem, 0);
                NewEvent.EndDate = new DateTime(datePickerEnd.SelectedDate.Value.Year, datePickerEnd.SelectedDate.Value.Month, datePickerEnd.SelectedDate.Value.Day,
                (int)cmbEndHours.SelectedItem, (int)cmbEndMinutes.SelectedItem, 0);
                NewEvent.Notification = chkNotification.IsChecked ?? false;

            }

            if (NewEvent.StartDate > NewEvent.EndDate)
            {

                MessageBox.Show("Start Date cannot be greater than End Date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            DialogResult = true;
            Close();

        }


        //get id for new event
        private int GetNextEventID()
        {

            if (Events.Count == 0)
                return 1;

            int maxID = Events.Max(ev => ev.ID);
            return maxID + 1;

        }


        //cancel - > go back to the list of events
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = false;
            Close();

        }

    }

}