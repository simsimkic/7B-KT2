using InitialProject.Model;
using InitialProject.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace InitialProject.View.Guest1
{
    /// <summary>
    /// Interaction logic for SendRequest.xaml
    /// </summary>
    public partial class SendRequest : Window 
    {
        private readonly RescheduleRequestRepository _rescheduleRequestRepository;
        public AccommodationReservation SelectedReservation { get; set; }

        private DateTime _newStartDate;
        public DateTime NewStartDate
        {
            get { return _newStartDate; }
            set
            {
                if (_newStartDate != value)
                {
                    _newStartDate = value;
                    OnPropertyChanged(nameof(NewStartDate));
                }
            }
        }

        private DateTime _newEndDate;
        public DateTime NewEndDate
        {
            get { return _newEndDate; }
            set
            {
                if (_newEndDate != value)
                {
                    _newEndDate = value;
                    OnPropertyChanged(nameof(NewEndDate));
                }
            }
        }
        public SendRequest(AccommodationReservation selectedReservation,RescheduleRequestRepository rescheduleRequestRepository)
        {
            InitializeComponent();
            DataContext = this;

            SelectedReservation = selectedReservation;
             _rescheduleRequestRepository = rescheduleRequestRepository;  
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Are you sure you want to reserve another date: {NewStartDatePicker:d} - {NewEndDatePicker:d}?", "Reschedule Request Confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (DatesValid(SelectedReservation) && NumberOfDaysValid(SelectedReservation))
                {
                    var rescheduleRequest = new RescheduleRequest(SelectedReservation, NewStartDate, NewEndDate);
                    _rescheduleRequestRepository.Save(rescheduleRequest);
                    MessageBox.Show("Reschedule request sent successfully.", "Reschedule Request", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
        }

        private bool DatesValid(AccommodationReservation selectedReservation)
        {
            NewStartDate = NewStartDatePicker.SelectedDate.GetValueOrDefault();
            NewEndDate = NewEndDatePicker.SelectedDate.GetValueOrDefault();

            if (NewStartDate < DateTime.Today)
            {
                MessageBox.Show("The new start date must not be earlier than today's date.", "Reschedule Request Error", MessageBoxButton.OK);
                return false;
            }
            else if (NewStartDate == selectedReservation.StartDate && NewEndDate == selectedReservation.EndDate)
            {
                MessageBox.Show("Selected dates are the same as the existing reservation dates. Please select different dates.", "Reschedule Request Error", MessageBoxButton.OK);
                return false;
            }
            else if (NewEndDate < NewStartDate)
            {
                MessageBox.Show("The new end date must not be earlier than the new start date.", "Reschedule Request Error", MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private bool NumberOfDaysValid(AccommodationReservation selectedReservation)
        {
            NewStartDate = NewStartDatePicker.SelectedDate.GetValueOrDefault();
            NewEndDate = NewEndDatePicker.SelectedDate.GetValueOrDefault();
            int numberOfDays = (NewEndDate - NewStartDate).Days;
            int expectedNumberOfDays = (selectedReservation.EndDate - selectedReservation.StartDate).Days;

            if (numberOfDays != expectedNumberOfDays)
            {
                 MessageBox.Show("The number of days must be the same as in the existing reservation.", "Reschedule Request Error", MessageBoxButton.OK);
                return false;
            }
           return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
