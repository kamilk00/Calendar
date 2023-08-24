using System;
using System.Collections.ObjectModel;
using System.Windows;
using Calendar.Models;
using Calendar.ViewModels;

namespace Calendar
{

    public partial class EventListWindow : Window
    {

        public EventListWindow(DateTime selectedDate, ObservableCollection<EventModel> events, string eventsFilePath)
        {

            InitializeComponent();
            DataContext = new EventListViewModel(selectedDate, events, eventsFilePath, this);

        }

    }

}