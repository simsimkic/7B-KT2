using InitialProject.Model;
using InitialProject.Repository;
using InitialProject.Resources.Enums;
using InitialProject.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Image = InitialProject.Model.Image;

namespace InitialProject.View.Owner
{
    public partial class RegisterAccommodation : Window, INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly AccommodationRepository _repository;

        private readonly LocationService _locationService;

        private readonly ImageRepository _imageRepository;

        public User LoggedInUser { get; set; }

        public int LastImageId { get; set; }

        private string _accommodationName;
        public string AccommodationName
        {
            get => _accommodationName;
            set
            {
                if (value != _accommodationName)
                {
                    _accommodationName = value;
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

        private string _type;
        public string Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value;
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

        private string _minReservationDays;
        public string MinReservationDays
        {
            get => _minReservationDays;
            set
            {
                if (value != _minReservationDays)
                {
                    _minReservationDays = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _cancellationPeriod;
        public string CancellationPeriod
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

        private ObservableCollection<string> _imageUrls = new ObservableCollection<string>();
        public ObservableCollection<string> ImageUrls
        {
            get => _imageUrls;
            set
            {
                if (_imageUrls != value)
                {
                    _imageUrls = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<int> _imageIds = new ObservableCollection<int>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RegisterAccommodation(User user, AccommodationRepository repository, LocationService locationService, ImageRepository imageRepository)
        {
            InitializeComponent();
            DataContext = this;

            _repository = repository;
            _locationService = locationService;
            _imageRepository = imageRepository;

            LoggedInUser = user;

            InitializeCountries();

            InitializeAccommodationType_cb();
        }

        private void InitializeAccommodationType_cb()
        {
            AccommodationType_cb.Items.Add("Apartment");
            AccommodationType_cb.Items.Add("House");
            AccommodationType_cb.Items.Add("Hut");
            AccommodationType_cb.SelectedIndex = 0;
        }

        private void InitializeCountries()
        {
            foreach (var country in _locationService.GetCountries())
            {
                cbCountry.Items.Add(country);
            }
        }

        private void btnRegisterAccommodation_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid)
            {
                Location AccommodationLocation = _locationService.GetLocation(Country, City);
                Accommodation Accommodation = new Accommodation(AccommodationName, LoggedInUser.Id, AccommodationLocation.Id, Enum.Parse<AccommodationType>(Type), int.Parse(MaxGuests), int.Parse(CancellationPeriod), int.Parse(CancellationPeriod), _imageIds);
                _repository.Save(Accommodation);
                Close();
            }
            else
            {
                MessageBox.Show("Cannot Register Accommodation", "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AccommodationType_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Type = ((ComboBox)sender).SelectedItem.ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            string imageUrl = UrlTextBox.Text;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                ImageUrls.Add(imageUrl);
                Image image = new Image(imageUrl);
                _imageRepository.Save(image);
                _imageIds.Add(image.Id);
            }
            UrlTextBox.Text = string.Empty;
        }

        private void CbCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCountry.SelectedItem != null && _locationService != null)
            {
                if (cbCity.Items != null)
                {
                    cbCity.Items.Clear();
                }

                cbCity.IsEnabled = true;
                foreach (String city in _locationService.GetCities(cbCountry.SelectedItem.ToString()))
                {
                    cbCity.Items.Add(city);
                }
            }
        }

        private void CbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCountry.SelectedItem != null && cbCity.SelectedItem != null)
            {
                Country = cbCountry.SelectedItem.ToString();
                City = cbCity.SelectedItem.ToString();
            }
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                int TryParseNumber;
                if (columnName == "AccommodationName")
                {
                    if (string.IsNullOrEmpty(AccommodationName))
                        return "This field is required";
                }
                else if (columnName == "MaxGuests")
                {
                    if (string.IsNullOrEmpty(MaxGuests))
                        return "This field is required";

                    if (!int.TryParse(MaxGuests, out TryParseNumber))
                        return "This field should be a number";
                }
                else if (columnName == "MinReservationDays")
                {
                    if (string.IsNullOrEmpty(MinReservationDays))
                        return "This field is required";

                    if (!int.TryParse(MinReservationDays, out TryParseNumber))
                        return "This field should be a number";
                }
                else if (columnName == "CancellationPeriod")
                {
                    if (string.IsNullOrEmpty(CancellationPeriod))
                        return "This field is required";

                    if (!int.TryParse(CancellationPeriod, out TryParseNumber))
                        return "This field should be a number";
                }

                return null;
            }
        }

        private readonly string[] _validatedProperties = { "AccommodationName", "MaxGuests", "MinReservationDays", "CancellationPeriod" };

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
    }
}
