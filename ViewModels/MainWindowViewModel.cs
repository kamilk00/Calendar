using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Calendar.Models;
using System.IO;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Media.Imaging;
using System.Text.Json;
using System.Windows;

namespace Calendar.ViewModels
{

    public class MainWindowViewModel : ViewModelBase
    {

        private MainWindow _mainWindow;
        private string EventsFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, 
                                                    "Events.json");
        private DispatcherTimer notificationTimer;
        public ObservableCollection<EventModel> Events { get; set; }
        public ICommand DoubleClickCommand { get; }
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }


        public MainWindowViewModel(MainWindow mainWindow)
        {

            _mainWindow = mainWindow;
            Events = new ObservableCollection<EventModel>();
            DoubleClickCommand = new RelayCommand(param => CalendarDoubleClick());
            LoadEvents();
            InitializeNotificationTimer();

        }


        private void InitializeNotificationTimer()
        {

            notificationTimer = new DispatcherTimer();
            notificationTimer.Interval = TimeSpan.FromMinutes(1); //check for events every 1 minute
            notificationTimer.Tick += NotificationTimer_Tick;
            notificationTimer.Start();

        }


        //open new window with list of events for selected date
        public void CalendarDoubleClick()
        {

            var eventListWindow = new EventListWindow(SelectedDate, Events, EventsFilePath);
            eventListWindow.Owner = _mainWindow;

            //hide the main window
            _mainWindow.Visibility = Visibility.Collapsed;
            eventListWindow.ShowDialog();

            //show the main window again
            _mainWindow.Visibility = Visibility.Visible;

        }


        private void NotificationTimer_Tick(object sender, EventArgs e)
        {

            LoadEvents();
            //check for upcoming events and display notifications
            foreach (var ev in Events)
            {

                var startTime = ev.StartDate.AddMinutes(-15);
                //check for events within the next minute and show notification
                if (startTime > DateTime.Now && startTime <= DateTime.Now.AddMinutes(1) && ev.Notification) 
                    ShowNotification(ev.Title, ev.Description, ev.StartDate);

            }

        }


        private void ShowNotification(string title, string description, DateTime eventDate)
        {

            var startTime = eventDate;

            string iconUri = "pack://application:,,,/icon.ico";
            var iconImageSource = new BitmapImage(new Uri(iconUri));

            var notification = new TaskbarIcon
            {
                IconSource = iconImageSource,
                ToolTipText = $"Event starts at {startTime}",
            };

            notification.ShowBalloonTip(title, description, BalloonIcon.Info);

        }


        //read events from JSON file
        private void LoadEvents()
        {

            if (File.Exists(EventsFilePath))
            {

                var jsonString = File.ReadAllText(EventsFilePath);
                Events = JsonSerializer.Deserialize<ObservableCollection<EventModel>>(jsonString);

            }
            
            else
                Events = new ObservableCollection<EventModel>();

        }

    }

}