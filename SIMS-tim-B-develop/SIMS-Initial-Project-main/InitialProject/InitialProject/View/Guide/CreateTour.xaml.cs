using InitialProject.Model;
using InitialProject.Repository;
using InitialProject.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Image = InitialProject.Model.Image;

namespace InitialProject.View.Guide
{
    public partial class CreateTour : Window,INotifyPropertyChanged,IDataErrorInfo
    {
        const int NO_TOUR_ASSIGNED = -1;

        private readonly TourService _tourService;
        private readonly LocationService _locationService;
        private readonly ImageRepository _imageRepository;
        private readonly CheckpointRepository _checkpointRepository;

        private ObservableCollection<int> ImageIds;
        private ObservableCollection<int> CheckpointIds;
        private User LoggedInUser;
        public ObservableCollection<Checkpoint> MiddleCheckpoints { get; set; }
        public ObservableCollection<string> ImageUrls { get; set; }
        public ObservableCollection<DateTime> DateTimes { get; set; }
        public Location TourLocation { get; set; }

        public int OrderCounter { get; set; }

        private Checkpoint _startCheckpoint;
        public Checkpoint StartCheckpoint

        {
            get { return _startCheckpoint; }
            set
            {
                if (_startCheckpoint != value)
                {
                    _startCheckpoint = value;
                    OnPropertyChanged(nameof(StartCheckpoint));
                }
            }
        }

        private Checkpoint _endCheckpoint;
        public Checkpoint EndCheckpoint
        {
            get { return _endCheckpoint; }
            set
            {
                if (_endCheckpoint != value)
                {
                    _endCheckpoint = value;
                    OnPropertyChanged(nameof(EndCheckpoint));
                }
            }
        }

        private string _name;
        public string TourName
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _language;
        public string TourLanguage
        {
            get => _language;
            set
            {
                if (value != _language)
                {
                    _language = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _maxGuests;
        public string MaxGuests
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

        private string _duration;
        public string Duration
        {
            get => _duration;
            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _country;
        public string Country
        {
            get => _country;
            set
            {
                if (value != _country)
                {
                    _country = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _city;
        public string City
        {
            get => _city;
            set
            {
                if (value != _city)
                {
                    _city = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _hours;
        public string Hours
        {
            get => _hours;
            set
            {
                if (value != _hours)
                {
                    _hours = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _minutes;
        public string Minutes
        {
            get => _minutes;
            set
            {
                if (value != _minutes)
                {
                    _minutes = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public CreateTour(User user, TourService tourService, LocationService locationService, ImageRepository imageRepository, CheckpointRepository checkpointRepository)
        {
            InitializeComponent();
            DataContext = this;

            _tourService = tourService;
            _locationService = locationService;
            _imageRepository = imageRepository;
            _checkpointRepository = checkpointRepository;

            LoggedInUser = user;
            OrderCounter = 1;

            InitializeCollections();
            InitializeComboBoxes();
            InitializeCountryDropdown();
            DisableCheckpointButtons();
        }
        private void InitializeCollections()
        {
            ImageIds = new ObservableCollection<int>();
            CheckpointIds = new ObservableCollection<int>();
            MiddleCheckpoints = new ObservableCollection<Checkpoint>();
            ImageUrls = new ObservableCollection<string>();
            DateTimes = new ObservableCollection<DateTime>();
            StartCheckpoint = null;
            EndCheckpoint = null;
        }
        private void InitializeComboBoxes()
        {
            for (int i = 0; i < 24; i++)
            {
                string hour = i.ToString("D2");
                Hours_cb.Items.Add(hour);
            }
            for (int i = 0; i < 60; i++)
            {
                string minute = i.ToString("D2");
                Minutes_cb.Items.Add(minute);
            }
        }

        private void InitializeCountryDropdown()
        {
            foreach (var country in _locationService.GetCountries().OrderBy(c => c))
            {
                cbCountry.Items.Add(country);
            }
        }
        private void DisableCheckpointButtons()
        {
            AddFinalCheckpointButton.IsEnabled = false;
            AddMiddleCheckpointButton.IsEnabled = false;
        }
        private void btnCreateTour_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid)
            {
                ShowInvalidInfoWarning();
                return;
            }

            if (ImageUrls.Count() < 1)
            {
                ShowImageWarning();
                return;
            }

            if (DateTimes.Count() < 1)
            {
                ShowNoDateTimeWarning();
                return;
            }

            if (StartCheckpoint == null)
            {
                ShowStartCheckpointWarning();
                return;
            }

            if (EndCheckpoint == null)
            {
                ShowEndCheckpointWarning();
                return;
            }

            TourLocation = _locationService.GetLocation(Country, City);

            if (TourLocation == null)
            {
                ShowLocationWarning();
                return;
            }

            SaveTour();

            Close();
        }
        private void SaveTour()
        {
            List<int> imageIds = SaveImages();
            List<int> checkpointIds = SaveCheckpoints();

            foreach (DateTime dateTime in DateTimes)
            {
                Tour tour = new Tour(TourName,
                    TourLocation.Id,
                    Description, 
                    TourLanguage, 
                    int.Parse(MaxGuests), 
                    0, 
                    dateTime,
                    double.Parse(Duration), 
                    LoggedInUser.Id,
                    new ObservableCollection<int>(imageIds),
                    new ObservableCollection<int>(checkpointIds)); 

                tour = _tourService.Save(tour);

                UpdateCheckpointsTourId(tour.Id);
            }
        }
        private List<int> SaveImages()
        {
            List<int> imageIds = new List<int>();
            foreach (string imageUrl in ImageUrls)
            {
                imageIds.Add(_imageRepository.Save(new Image(imageUrl)).Id);
            }
            return imageIds;
        }
        private List<int> SaveCheckpoints()
        {
            List<int> checkpointIds = new List<int>();
            checkpointIds.Add(_checkpointRepository.Save(StartCheckpoint).Id);
            foreach (Checkpoint checkpoint in MiddleCheckpoints)
            {
                checkpointIds.Add(_checkpointRepository.Save(checkpoint).Id);
            }
            checkpointIds.Add(_checkpointRepository.Save(EndCheckpoint).Id);
            return checkpointIds;
        }
        private void UpdateCheckpointsTourId(int tourId)
        {
            StartCheckpoint.TourId = tourId;
            _checkpointRepository.Update(StartCheckpoint);

            foreach (Checkpoint checkpoint in MiddleCheckpoints)
            {
                checkpoint.TourId = tourId;
                _checkpointRepository.Update(checkpoint);
            }

            EndCheckpoint.TourId = tourId;
            _checkpointRepository.Update(EndCheckpoint);
        }
        private void Hours_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Hours = ((ComboBox)sender).SelectedItem.ToString();
        }
        private void Minutes_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Minutes = ((ComboBox)sender).SelectedItem.ToString();
        }
        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            string imageUrl = UrlTextBox.Text;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                ImageUrls.Add(imageUrl);
            }
            UrlTextBox.Text = string.Empty;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void AddStartingCheckpoint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StartingCheckpointName.Text))
                return;

            StartCheckpoint = CreateCheckpoint(StartingCheckpointName.Text, 1);

            SetCheckpointButtonsState(false, true, false);
            SetCheckpointInputsState(false, true);

            StartingCheckpointName.Text = string.Empty;
        }

        private void AddFinalCheckpoint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FinalCheckpointName.Text))
                return;

            EndCheckpoint = CreateCheckpoint(FinalCheckpointName.Text, 2);

            SetCheckpointButtonsState(false, false, true);
            SetCheckpointInputsState(true, true);

            FinalCheckpointName.Text = string.Empty;
        }

        private void AddMiddleCheckpoint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MiddleCheckpointName.Text))
                return;

            Checkpoint checkpoint = CreateCheckpoint(MiddleCheckpointName.Text, ++OrderCounter);
            MiddleCheckpoints.Add(checkpoint);

            EndCheckpoint.Order = MiddleCheckpoints.Count() + 2;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndCheckpoint)));

            MiddleCheckpointName.Text = string.Empty;

            SetCheckpointButtonsState(false, false, true);
        }

        private Checkpoint CreateCheckpoint(string name, int order)
        {
            return new Checkpoint(name, order, false, NO_TOUR_ASSIGNED);
        }
        private void SetCheckpointButtonsState(bool starting, bool final, bool middle)
        {
            AddStartingCheckpointButton.IsEnabled = starting;
            AddFinalCheckpointButton.IsEnabled = final;
            AddMiddleCheckpointButton.IsEnabled = middle;
        }
        private void SetCheckpointInputsState(bool starting, bool middle)
        {
            StartingCheckpointName.IsEnabled = starting;
            FinalCheckpointName.IsEnabled = true;
            MiddleCheckpointName.IsEnabled = middle;
        }

        private void CbCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCountry.SelectedItem != null && _locationService != null)
            {
                ClearCityItems();
                EnableCitySelection();
                LoadCitiesForSelectedCountry();
            }
        }
        private void ClearCityItems()
        {
            if (cbCity.Items != null)
            {
                cbCity.Items.Clear();
            }
        }
        private void EnableCitySelection()
        {
            cbCity.IsEnabled = true;
        }
        private void LoadCitiesForSelectedCountry()
        {
            foreach (string city in _locationService.GetCities(cbCountry.SelectedItem.ToString()).OrderBy(c => c))
            {
                cbCity.Items.Add(city);
            }
        }
        private void CbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCountry.SelectedItem != null && cbCity.SelectedItem != null)
            {
                UpdateCountryAndCity();
            }
        }
        private void UpdateCountryAndCity()
        {
            Country = cbCountry.SelectedItem.ToString();
            City = cbCity.SelectedItem.ToString();
        }
        private void addDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsDateTimeInputValid())
            {
                DateTime selectedDateTime = GetSelectedDateTime();
                DateTimes.Add(selectedDateTime);
                return;
            }             
            
            ShowInvalidDateTimeWarning();
        }
        private bool IsDateTimeInputValid()
        {
            if (dpDate.SelectedDate == null || Hours == null || Minutes == null)
            {
                return false;
            }

            DateTime selectedDate = dpDate.SelectedDate.GetValueOrDefault();
            DateTime selectedDateTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, int.Parse(Hours), int.Parse(Minutes), 0);

            return selectedDateTime >= DateTime.Now;
        }
        private DateTime GetSelectedDateTime()
        {
            DateTime selectedDate = dpDate.SelectedDate.GetValueOrDefault();
            DateTime selectedDateTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, int.Parse(Hours), int.Parse(Minutes), 0);

            return selectedDateTime;
        }
        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                int TryParseNumber;
                double TryParseNumberD;
                if (columnName == "TourName")
                {
                    if (string.IsNullOrEmpty(TourName))
                        return "Name is required";
                }
                else if (columnName == "Description")
                {
                    if (string.IsNullOrEmpty(Description))
                        return "Description is required";
                }
                else if (columnName == "TourLanguage")
                {
                    if (string.IsNullOrEmpty(TourLanguage))
                        return "Language is required";
                }
                else if (columnName == "MaxGuests")
                {
                    if (string.IsNullOrEmpty(MaxGuests))
                        return "Maximum guest number is required";

                    if (!int.TryParse(MaxGuests, out TryParseNumber))
                        return "This field should be a number";
                    else 
                    {
                        if (int.Parse(MaxGuests) <= 0)
                            return "Invalid value of maximum guest number";
                    }
                }
                else if (columnName == "Duration")
                {
                    if (string.IsNullOrEmpty(Duration))
                        return "Duration is required";

                    if (!double.TryParse(Duration, out TryParseNumberD))
                        return "This field should be a number";
                    else
                    {
                        if (double.Parse(Duration) <= 0)
                            return "Invalid duration value";
                    }
                }

                return null;
            }
        }
    
        private readonly string[] _validatedProperties = { "TourName", "Description", "TourLanguage", "MaxGuests", "Duration" };
        public bool IsValid
        {
            get
            {
                foreach (var property in _validatedProperties)
                {
                    if (this[property] != null)
                        return false;
                }

                return true;
            }
        }
        private void ShowInvalidInfoWarning()
        {
            MessageBox.Show("Please enter valid information.", "Info warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowImageWarning()
        {
            MessageBox.Show("Please enter at least one image.", "Image warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowNoDateTimeWarning()
        {
            MessageBox.Show("Please enter at least one date and time.", "Date and time warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowInvalidDateTimeWarning()
        {
            MessageBox.Show("Please choose a valid date and time.", "Date and time warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowStartCheckpointWarning()
        {
            MessageBox.Show("Please enter the starting checkpoint.", "Start checkpoint warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowEndCheckpointWarning()
        {
            MessageBox.Show("Please enter the ending checkpoint.", "End checkpoint warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowLocationWarning()
        {
            MessageBox.Show("Please enter the location.", "Location warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
