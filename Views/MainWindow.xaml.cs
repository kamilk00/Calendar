using Calendar.ViewModels;
using System.Windows;

namespace Calendar
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
            appCalendar.MouseDoubleClick += AppCalendar_MouseDoubleClick;

        }


        private void AppCalendar_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (DataContext is MainWindowViewModel viewModel)
                viewModel.CalendarDoubleClick();

        }

    }

}