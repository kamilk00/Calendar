using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Calendar
{

    public partial class MainWindow : Window
    {

        private string EventsFilePath = System.IO.Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Events.json");
        public ObservableCollection<Event> Events { get; set; }
        private DispatcherTimer notificationTimer;


        public MainWindow()
        {

            InitializeComponent();
            LoadEvents();
            appCalendar.DisplayDate = DateTime.Today;
            notificationTimer = new DispatcherTimer();
            notificationTimer.Interval = TimeSpan.FromMinutes(1); //check for events every 1 minute
            notificationTimer.Tick += NotificationTimer_Tick;
            notificationTimer.Start();

        }


        //open new window with list of events for selected date
        private void Calendar_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (sender is System.Windows.Controls.Calendar calendar)
            {

                DateTime selectedDate = calendar.SelectedDate ?? DateTime.Today;

                var eventListWindow = new EventListWindow(selectedDate, Events, EventsFilePath);
                eventListWindow.Owner = this;

                //hide the main window
                this.Visibility = Visibility.Collapsed; 
                eventListWindow.ShowDialog();

                //show the main window again
                this.Visibility = Visibility.Visible; 

            }

        }


        private void NotificationTimer_Tick(object sender, EventArgs e)
        {

            LoadEvents();
            //check for upcoming events and display notifications
            foreach (var ev in Events)
            {

                var startTime = ev.StartDate.AddMinutes(-15);
                if (startTime > DateTime.Now && startTime <= DateTime.Now.AddMinutes(1) && ev.Notification) //check for events within the next minute
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

            // Show the notification for 5 seconds
            notification.ShowBalloonTip(title, description, BalloonIcon.Info);
        }

    


        //read events from JSON file
        private void LoadEvents()
        {

            if (File.Exists(EventsFilePath))
            {

                var jsonString = File.ReadAllText(EventsFilePath);
                Events = JsonSerializer.Deserialize<ObservableCollection<Event>>(jsonString);

            }

            else
                Events = new ObservableCollection<Event>();

        }


        private ImageSource ConvertBitmapToImageSource(Bitmap bitmap)
        {

            using (MemoryStream stream = new MemoryStream())
            {

                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;

            }

        }

    }

}