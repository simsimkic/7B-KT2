using InitialProject.Model;
using InitialProject.Repository;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InitialProject.View.Guide
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        private readonly TourReservationRepository _tourReservationRepository;
        public Tour SelectedTour { get; set; }
        public SeriesCollection AgeSeriesCollection { get; set; }

        public SeriesCollection VaucherSeriesCollection { get; set; }
        public string[] AgeLabels { get; set; }
        public string[] VaucherLabels { get; set; }
        public Statistics(Tour selectedTour, TourReservationRepository tourReservationRepository )
        {
            InitializeComponent();
            DataContext = this;

            SelectedTour = selectedTour;
            _tourReservationRepository = tourReservationRepository;

            AgeSeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "<18",
                    Values = new ChartValues<double> { _tourReservationRepository.GetUnder18Count(SelectedTour) },
                    Stroke = Brushes.Black, // Set the outline color of the columns to black
                    Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0x00)), // Set the fill color of the first column to yellow
                    ColumnPadding = 30, // Set the column padding to 0 to remove the space between columns
                    MaxColumnWidth = 120 // Increase the maximum width of the columns to 120
                },
                new ColumnSeries
                {   
                    Title = "18-50",
                    Values = new ChartValues<double> { _tourReservationRepository.GetBetween18And50Count(SelectedTour) },
                    Stroke = Brushes.Black, // Set the outline color of the columns to black
                    Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF)), // Set the fill color of the second column to blue
                    ColumnPadding = 30, // Set the column padding to 0 to remove the space between columns
                    MaxColumnWidth = 120 // Increase the maximum width of the columns to 120
                },
                new ColumnSeries
                {
                    Title = "50+",
                    Values = new ChartValues<double> { _tourReservationRepository.GetAbove50Count(SelectedTour) },
                    Stroke = Brushes.Black, // Set the outline color of the columns to black
                    Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x66, 0x00)), // Set the fill color of the third column to orange
                    ColumnPadding = 30, // Set the column padding to 0 to remove the space between columns
                    MaxColumnWidth = 120 // Increase the maximum width of the columns to 120
                }
            };

            AgeLabels = new[] {"Age Groups"};

            VaucherSeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Yes",
                    Values = new ChartValues<double> { _tourReservationRepository.GetUsedVoucherCount(SelectedTour) },
                    Stroke = Brushes.Black, // Set the outline color of the columns to black
                    Fill = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x00)), // Set the fill color of the first column to green
                    ColumnPadding = 30, // Set the column padding to 0 to remove the space between columns
                    MaxColumnWidth = 120 // Increase the maximum width of the columns to 120
                },
                new ColumnSeries
                {
                    Title = "No",
                    Values = new ChartValues<double> { _tourReservationRepository.GetUnusedVoucherCount(SelectedTour)  },
                    Stroke = Brushes.Black, // Set the outline color of the columns to black
                    Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00)), // Set the fill color of the second column to blue
                    ColumnPadding = 30, // Set the column padding to 0 to remove the space between columns
                    MaxColumnWidth = 120 // Increase the maximum width of the columns to 120
                }
            };

            VaucherLabels = new[] { "Used A Voucher?" };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
