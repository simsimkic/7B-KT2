using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using InitialProject.Model;
using InitialProject.Repository;
using InitialProject.Resources.Observer;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using InitialProject.Model.DTO;
using InitialProject.Service;

namespace InitialProject.View.Guest2
{
    public partial class Guest2Window : Window, INotifyPropertyChanged, IObserver
    {
        public User LoggedInUser { get; set; }

        public Guest2TourDTO SelectedGuest2TourDTO { get; set; }
        public ObservableCollection<Guest2TourDTO> TourDTOs { get; set; }
        public ObservableCollection<Tour> Tours { get; set; }

        public ObservableCollection<Guest2TourDTO> FinishedTourDTOs { get; set; }
        public List<Tour> FinishedTours { get; set; }

        public List<Tour> CheckedTours { get; set; }
        public Tour CurrentlyActiveTour { get; set; }
        public Checkpoint CurrentlyActiveCheckpoint { get; set; }

        public ObservableCollection<Location> Locations;

        public ObservableCollection<Guest2TourDTO> NonReservedTours { get; set; }

        private readonly TourService _tourService;
        private readonly LocationRepository _locationRepository;
        private readonly ImageRepository _imageRepository;
        private readonly CheckpointRepository _checkpointRepository;
        private readonly UserRepository _userRepository;
        private readonly TourReservationRepository _tourReservationRepository;
        private readonly TourRatingRepository _tourRatingRepository;


        private string country;
        public string Country
        {
            get => country;
            set
            {
                if (value != country)
                {
                    country = value;
                    OnPropertyChanged();
                }
            }
        }

        private string city;
        public string City
        {
            get => city;
            set
            {
                if (value != city)
                {
                    city = value;
                    OnPropertyChanged();
                }
            }
        }

        private string tourLanguage;
        public string TourLanguage
        {
            get => tourLanguage;
            set
            {
                if (value != tourLanguage)
                {
                    tourLanguage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string personCount;
        public string PersonCount
        {
            get => personCount;
            set
            {
                if (value != personCount)
                {
                    personCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public Guest2Window(User user)
        {
            InitializeComponent();
            DataContext = this;
            LoggedInUser = user;

            _tourService = new TourService();
            _tourService.Subscribe(this);

            _imageRepository = new ImageRepository();
            _imageRepository.Subscribe(this);

            _locationRepository = new LocationRepository();
            _locationRepository.Subscribe(this);

            _checkpointRepository = new CheckpointRepository();
            _checkpointRepository.Subscribe(this);

            _userRepository = new UserRepository();
            _userRepository.Subscribe(this);

            _tourReservationRepository = new TourReservationRepository();
            _tourReservationRepository.Subscribe(this);

            _tourRatingRepository = new TourRatingRepository();
            _tourRatingRepository.Subscribe(this);

            Tours = new ObservableCollection<Tour>(_tourService.GetReservableTours());

            TourDTOs = ConvertToDTO(new List<Tour>(Tours));

            CheckedTours = new List<Tour>();
            foreach (int id in _tourReservationRepository.GetCheckedTourIds(LoggedInUser))
            {
                CheckedTours.Add(_tourService.GetById(id));
            }

            if(CheckedTours.Count != 0 && !CheckedTours[0].IsFinished)
            {
                CurrentlyActiveTour = CheckedTours[0];
                CurrentlyActiveCheckpoint = _checkpointRepository.GetById(CurrentlyActiveTour.CurrentCheckpointId);
            }

            List<Tour> UserTours = new List<Tour>(_tourService.GetUserTours(LoggedInUser));
            FinishedTours = _tourService.GetFinishedTours(UserTours);
            FinishedTourDTOs = ConvertToDTO(FinishedTours);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Update()
        {
            TourDTOs.Clear();
            FormTours();

            FinishedTourDTOs.Clear();
            FormFinishedTours();
        }

        public void FormTours()
        {
            foreach (Tour tour in _tourService.GetReservableTours())
            {
                TourDTOs.Add(ConvertToDTO(tour));
            }
        }

        public void FormFinishedTours()
        {
            foreach (Tour userTour in _tourService.GetUserTours(LoggedInUser))
            {
                if (userTour.IsFinished && !_tourReservationRepository.GetByTourId(userTour.Id).IsRated)
                {
                    FinishedTourDTOs.Add(ConvertToDTO(userTour));
                }
            }
        }

        public ObservableCollection<Guest2TourDTO> ConvertToDTO(List<Tour> tours)
        {
            ObservableCollection<Guest2TourDTO> dto = new ObservableCollection<Guest2TourDTO>();

            foreach (Tour tour in tours)
            {
                dto.Add(new Guest2TourDTO(
                    tour.Id,
                    tour.Name,
                    _locationRepository.GetById(tour.LocationId).Country,
                    _locationRepository.GetById(tour.LocationId).City,
                    tour.Description,
                    tour.Language,
                    tour.MaxGuests,
                    tour.CurrentGuestCount,
                    tour.StartTime,
                    tour.Duration,
                    _userRepository.GetById(tour.GuideId).Username));
            }

            return dto;
        }
        public Guest2TourDTO ConvertToDTO(Tour tour)
        {
            return new Guest2TourDTO(
                tour.Id,
                tour.Name,
                _locationRepository.GetById(tour.LocationId).Country,
                _locationRepository.GetById(tour.LocationId).City,
                tour.Description,
                tour.Language,
                tour.MaxGuests,
                tour.CurrentGuestCount,
                tour.StartTime,
                tour.Duration,
                _userRepository.GetById(tour.GuideId).Username);
        }

        private void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGuest2TourDTO != null)
            {
                ReserveTour reserveTourForm = new ReserveTour(SelectedGuest2TourDTO, LoggedInUser, _tourService, _tourReservationRepository);
                reserveTourForm.ShowDialog();
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            CheckIfAllEmpty();

            TourDTOs.Clear();

            List<Tour> result = new List<Tour>();

            if (!string.IsNullOrEmpty(Country))
            {
                result = _tourService.GetByCountryName(Country);
            }
            else
            {
                result = _tourService.GetReservableTours();
            }

            if (!string.IsNullOrEmpty(City))
            {
                result = result.Intersect(_tourService.GetByCityName(City)).ToList();
            }
            if (!string.IsNullOrEmpty(TourLanguage))
            {
                result = result.Intersect(_tourService.GetByLanguage(TourLanguage)).ToList();
            }
            if (!string.IsNullOrEmpty(PersonCount))
            {
                result = result.Intersect(_tourService.GetByGuests(int.Parse(PersonCount))).ToList();
            }

            ObservableCollection<Guest2TourDTO> searchResults = ConvertToDTO(result);
            foreach (Guest2TourDTO dto in searchResults)
            {
                TourDTOs.Add(dto);
            }
        }

        public void CheckIfAllEmpty()
        {
            if (string.IsNullOrEmpty(Country) && string.IsNullOrEmpty(City) && string.IsNullOrEmpty(TourLanguage) && string.IsNullOrEmpty(PersonCount))
            {
                foreach (Tour tour in _tourService.GetReservableTours())
                {
                    TourDTOs.Add(ConvertToDTO(tour));
                }
            }
        }
        private void ShowMoreButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTour showTour = new ShowTour(SelectedGuest2TourDTO);
            showTour.Show();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            SignInForm signInForm = new SignInForm();
            signInForm.Show();
            Close();
        }

        private void RateButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGuest2TourDTO != null)
            {
                RateTour rateTour = new RateTour(SelectedGuest2TourDTO, LoggedInUser, _tourRatingRepository, _tourReservationRepository, _tourService, _imageRepository);
                rateTour.Show();
            }
        }
    }
}