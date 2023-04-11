using InitialProject.Model.DTO;
using InitialProject.Model;
using InitialProject.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InitialProject.View.Guest1
{
    /// <summary>
    /// Interaction logic for ReserveAccommodation.xaml
    /// </summary>
    public partial class ReserveAccommodation : Window
    {
        private readonly AccommodationReservationRepository _accommodationReservationRepository;
        private readonly AccommodationRepository _accommodationRepository;

        private AccommodationReservation _reservation;
        public AccommodationReservation Reservation
        {
            get { return _reservation; }
            set
            {
                if (_reservation != value)
                {
                    _reservation = value;
                    OnPropertyChanged(nameof(Reservation));
                }
            }
        }

        private GuestAccommodationDTO _selectedAccommodation;
        public GuestAccommodationDTO SelectedAccommodation
        {
            get { return _selectedAccommodation; }
            set
            {
                if (_selectedAccommodation != value)
                {
                    _selectedAccommodation = value;
                    OnPropertyChanged(nameof(SelectedAccommodation));
                }
            }
        }

        private ObservableCollection<DatesDTO> _dateIntervals;
        public ObservableCollection<DatesDTO> DateIntervals
        {
            get { return _dateIntervals; }
            set
            {
                _dateIntervals = value;
                OnPropertyChanged("DateIntervals");
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        private int _numberOfDays;
        public int NumberOfDays
        {
            get => _numberOfDays;
            set
            {
                if (value != _numberOfDays)
                {
                    _numberOfDays = value;
                    OnPropertyChanged();
                }
            }
        }
        public User LoggedInUser { get; set; }

        private bool _isAvailable;
        public bool Available
        {
            get => _isAvailable;
            set
            {
                if (value != _isAvailable)
                {
                    _isAvailable = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _maxGuests;
        public int MaxGuests
        {
            get => _maxGuests;
            set
            {
                if (value != _maxGuests)
                {
                    _maxGuests = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _ownerId;
        public int OwnerId
        {
            get => _ownerId;
            set
            {
                if (value != _ownerId)
                {
                    _ownerId = value;
                    OnPropertyChanged();
                }
            }
        }


        private int _cancellationPeriod;
        public int CancellationPeriod
        {
            get => _cancellationPeriod;
            set
            {
                if (value != _cancellationPeriod)
                {
                    _cancellationPeriod = value;
                    OnPropertyChanged();
                }
            }
        }
        public ReserveAccommodation(GuestAccommodationDTO selectedAccommodation, User user, AccommodationRepository accommodationRepository, AccommodationReservationRepository accommodationReservationRepository)
        {
            InitializeComponent();
            DataContext = this;

            SelectedAccommodation = selectedAccommodation;
            LoggedInUser = user;

            Reservation = new AccommodationReservation();
            Reservation.AccommodationId = selectedAccommodation.Id;


            _accommodationRepository = accommodationRepository;
            _accommodationReservationRepository = accommodationReservationRepository;

            DateIntervals = new ObservableCollection<DatesDTO>();
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DateIntervals.Clear();

            if (!ValidateDates()) return;
            if (!ValidateNumberOfDays()) return;
            if (!ValidateNumberOfGuests()) return;

            ObservableCollection<DateTime> allFreeDates = GetAllFreeDates();

            AddDateRanges(FindDateRanges(allFreeDates));

            if (DateIntervals.Count == 0)
            {
                var messageBoxResult = MessageBox.Show($"There are no available dates to reserve right now, would you like to see suggested dates?", "Suggested Accomodation Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    int DaysToAdd = 25;

                    if (DaysToAdd > 0)
                    {
                        DateTime newStartDate = StartDate.AddDays(-DaysToAdd);
                        if (newStartDate >= DateTime.Today)
                        {
                            StartDate = newStartDate.Date;
                        }
                        else
                        {
                            StartDate = DateTime.Today.Date;
                        }
                    }

                    EndDate = EndDate.Date.AddDays(DaysToAdd);

                    allFreeDates = GetAllFreeDates();

                    AddDateRanges(FindDateRanges(allFreeDates));
                }

                return;

            }

        }
        private ObservableCollection<DateTime> GetAllFreeDates()
        {
            int accommodationId = Reservation.AccommodationId;
            DateTime startDate = StartDate;
            DateTime endDate = EndDate;

            return new ObservableCollection<DateTime>(_accommodationReservationRepository.GetAvailableDates(accommodationId, startDate, endDate));
        }

        private void AddDateRanges(List<DatesDTO> dateRanges)
        {
            foreach (var dateRange in dateRanges)
            {
                DateIntervals.Add(dateRange);
            }
        }

        private bool ValidateDates()
        {
            StartDate = startDatePicker.SelectedDate.GetValueOrDefault();  
            EndDate = endDatePicker.SelectedDate.GetValueOrDefault();

            if (StartDate == default || EndDate == default)
            {
                ShowNoDateTimeWarning();
                return false;
            }

            if (StartDate.Date < DateTime.Today || EndDate.Date < DateTime.Today || StartDate.Date > EndDate.Date)
            {
                ShowInvalidDateTimeWarning();
                return false;
            }

            return true;
        }

        private bool ValidateNumberOfDays()
        {
            if (!int.TryParse(numDaysTextBox.Text, out int numberOfDays))
            {
                ShowInvalidNumberWarning();
                return false;
            }

            if (numberOfDays <= 0)
            {
                ShowNoNumberWarning();
                return false;
            }

            if (numberOfDays < SelectedAccommodation.MinReservationDays)
            {
                ShowMinimumReservationDaysWarning();
                return false;
            }

            NumberOfDays = numberOfDays;
            return true;
        }

        private bool ValidateNumberOfGuests()
        {
            if (!int.TryParse(maxGuestsTextBox.Text, out int numberGuests))
            {
                ShowInvalidInputWarning();
                return false;
            }

            if (numberGuests <= 0)
            {
                ShowNoInputWarning();
                return false;
            }

            if (numberGuests > SelectedAccommodation.MaxGuests)
            {
                ShowMaxGuestsWarning();
                return false;
            }

            MaxGuests = numberGuests;
            return true;
        }

        private List<DatesDTO> FindDateRanges(ObservableCollection<DateTime> dates)
        {
            var dateRanges = new List<DatesDTO>();

            for (int i = 0; i < dates.Count - NumberOfDays + 1; i++)  
            {
                DateTime startDate = dates[i];
                DateTime endDate = dates[i + NumberOfDays - 1];

                if (IsValidDateRange(dates, i))
                {
                    dateRanges.Add(new DatesDTO { StartDate = startDate, EndDate = endDate });
                }
            }

            return dateRanges;
        }

        private bool IsValidDateRange(ObservableCollection<DateTime> dates, int startIndex)
        {
            for (int i = startIndex + 1; i <= startIndex + NumberOfDays - 2; i++)  
            {
                if (!IsDateFollowsPreviousDate(dates, i))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsDateFollowsPreviousDate(ObservableCollection<DateTime> dates, int index)
        {
            return dates[index].Subtract(dates[index - 1]).Days == 1; 
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (sender as DataGrid)?.SelectedItem as DatesDTO;

            if (selectedItem != null)
            {
                var messageBoxResult = MessageBox.Show($"Are you sure you want to reserve the date: {selectedItem.StartDate:d} - {selectedItem.EndDate:d}", "Reserve Accomodation Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var reservation = new AccommodationReservation(LoggedInUser.Id, SelectedAccommodation.Id, selectedItem.StartDate, selectedItem.EndDate, NumberOfDays, MaxGuests, OwnerId, false, CancellationPeriod);
                    _accommodationReservationRepository.Save(reservation);

                    MessageBox.Show("Reservation created successfully.");
                    Close();
                }
                return;
            }
        }

        private void ShowNoDateTimeWarning()
        {
            MessageBox.Show("Please enter at least one date and time.", "Date and time warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowInvalidDateTimeWarning()
        {
            MessageBox.Show("Please choose a valid date and time.", "Date and time warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowNoNumberWarning()
        {
            MessageBox.Show("Please choose a number of days.", "Number of days warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowInvalidNumberWarning()
        {
            MessageBox.Show("Please choose a valid number.", "Number of days warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowMinimumReservationDaysWarning()
        {
            MessageBox.Show($"The number of days needs to be at least {SelectedAccommodation.MinReservationDays}, the selected accommodation's minimum reservation days.", "Number of days warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowNoInputWarning()
        {
            MessageBox.Show("Please choose a max number of guests.", "Max number of guests warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowInvalidInputWarning()
        {
            MessageBox.Show("Please choose a valid number.", "Max number of guests warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowMaxGuestsWarning()
        {
            MessageBox.Show($"The max number of guests needs to be at least {SelectedAccommodation.MaxGuests}, the selected accommodation's max guests.", "Max number of guests warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
