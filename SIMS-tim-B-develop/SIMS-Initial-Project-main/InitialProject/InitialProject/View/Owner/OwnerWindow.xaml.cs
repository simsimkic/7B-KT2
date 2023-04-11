using InitialProject.Model;
using InitialProject.Model.DTO;
using InitialProject.Repository;
using InitialProject.Resources.Observer;
using InitialProject.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace InitialProject.View.Owner
{
    /// <summary>
    /// Interaction logic for OwnerWindow.xaml
    /// </summary>
    public partial class OwnerWindow : Window, INotifyPropertyChanged, IObserver
    {
        public User LoggedInUser { get; set; }
        public ObservableCollection<Accommodation> Accommodations { get; set; }
        public Accommodation SelectedAccommodation { get; set; }
        public ObservableCollection<GuestReview> GuestReviews { get; set; }
        public GuestReview SelectedGuestReview { get; set; }
        public ObservableCollection<GuestReviewDTO> UnreviewedGuests { get; set; }
        public GuestReviewDTO SelectedUnreviewedGuest { get; set; }
        public ObservableCollection<AccommodationReservation> UnreviewedReservations { get; set; }
        public ObservableCollection<AccommodationRatings> Ratings { get; set; }
        public ObservableCollection<RatingsDTO> RatingsDTO { get; set; }
        public ObservableCollection<RescheduleRequest> Requests { get; set; }
        public RescheduleRequest SelectedRequest { get; set; }

        private AccommodationRepository _repository;
        private readonly LocationService _locationService;
        private readonly ImageRepository _imageRepository;
        private readonly AccommodationReservationRepository _reservationRepository;
        private readonly GuestReviewRepository _guestReviewRepository;
        private readonly UserRepository _userRepository;
        private readonly AccommodationRatingsRepository _ratingRepository;
        private readonly RescheduleRequestRepository _rescheduleRequestRepository;

        public OwnerWindow(User user)
        {
            InitializeComponent();
            DataContext = this;
            LoggedInUser = user;

            _repository = new AccommodationRepository();
            _repository.Subscribe(this);
            _locationService = new LocationService();
            _locationService.Subscribe(this);
            _imageRepository = new ImageRepository();
            _imageRepository.Subscribe(this);
            _reservationRepository = new AccommodationReservationRepository();
            _reservationRepository.Subscribe(this);
            _guestReviewRepository = new GuestReviewRepository();
            _guestReviewRepository.Subscribe(this);
            _userRepository = new UserRepository();
            _userRepository.Subscribe(this);
            _ratingRepository = new AccommodationRatingsRepository();
            _ratingRepository.Subscribe(this);
            _rescheduleRequestRepository = new RescheduleRequestRepository();
            _rescheduleRequestRepository.Subscribe(this);

            InitializeAccommodations();
            InitializeRatings();
            InitializeRescheduleRequests();
            CheckForEligibleReviews();
        }

        public void FormRescheduleRequests()
        {
            foreach (RescheduleRequest request in _rescheduleRequestRepository.GetAll())
            {
                if (request.OwnerId == LoggedInUser.Id && request.Status == InitialProject.Resources.Enums.RescheduleRequestStatus.onhold)
                {
                    Requests.Add(request);
                }
            }
        }

        public void InitializeRescheduleRequests()
        {
            Requests = new ObservableCollection<RescheduleRequest>();
            FormRescheduleRequests();
        }

        public void FormRatings()
        {
            /*foreach (AccommodationRatings rating in _ratingRepository.GetAll())
            {
                if (rating.OwnerId == LoggedInUser.Id && _guestReviewRepository.GetAll().Any(t => t.ReservationId == rating.ReservationId))
                {
                    Ratings.Add(rating);
                }
            }*/

            foreach (AccommodationRatings rating in _ratingRepository.GetAll())
            {
                if (rating.OwnerId == LoggedInUser.Id && _guestReviewRepository.GetAll().Any(t => t.ReservationId == rating.ReservationId))
                {
                    RatingsDTO dto = new RatingsDTO(_userRepository.GetById(_reservationRepository.GetById(rating.ReservationId).GuestId).Username, _repository.GetById(rating.AccommodationId).Name, rating.Cleanliness, rating.Correctness, rating.Comment);
                    RatingsDTO.Add(dto);
                }
            }
        }

        public void InitializeRatings()
        {
            Ratings = new ObservableCollection<AccommodationRatings>();
            RatingsDTO = new ObservableCollection<RatingsDTO>();
            FormRatings();
        }

        public void FormAccommodations()
        {
            foreach (Accommodation accommodation in _repository.GetAll())
            {
                if (accommodation.OwnerId == LoggedInUser.Id)
                {
                    Accommodations.Add(accommodation);
                }
            }
        }

        public void FormUnreviewedGuests()
        {
            foreach (AccommodationReservation reservation in _reservationRepository.GetAll())
            {
                if (LessThanFiveDaysPassed(reservation) && !GuestReviewed(reservation))
                {
                    if (Accommodations.Any(item => item.Id == reservation.AccommodationId))
                    {
                        UnreviewedGuests.Add(new GuestReviewDTO(reservation.Id, _userRepository.GetById(reservation.GuestId).Username, _repository.GetById(reservation.AccommodationId).Name));
                        UnreviewedReservations.Add(reservation);
                    }
                }
            }
        }

        public void InitializeAccommodations()
        {
            Accommodations = new ObservableCollection<Accommodation>();
            FormAccommodations();
        }

        
        public void CheckForEligibleReviews()
        {
            UnreviewedGuests = new ObservableCollection<GuestReviewDTO>();
            UnreviewedReservations = new ObservableCollection<AccommodationReservation>();
            FormUnreviewedGuests();

            if (UnreviewedGuests.Count != 0)
            {
                MessageBox.Show("You have unreviewed guests!", "Guest Review", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public bool GuestReviewed(AccommodationReservation reservation)
        {
            return _guestReviewRepository.GetAll().Any(item => item.ReservationId == reservation.Id);
        }

        public bool LessThanFiveDaysPassed(AccommodationReservation reservation)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(reservation.EndDate);
            return timeSpan.Days >= 0 && timeSpan.Days < 5;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterAccommodation registerAccommodation = new RegisterAccommodation(LoggedInUser, _repository, _locationService, _imageRepository);
            registerAccommodation.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAccommodation != null)
            {
                string MessageBoxText = "Are You Sure You Want To Delete Accommodation?";
                string Caption = "Delete Accommodation";
                MessageBoxButton Button = MessageBoxButton.YesNo;
                MessageBoxImage Icon = MessageBoxImage.Warning;
                MessageBox.Show(MessageBoxText, Caption, Button, Icon, MessageBoxResult.Yes);
            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            SignInForm signInForm = new SignInForm();
            signInForm.Show();
            Close();
        }
        
        private void ReviewGuestButton_Guest(object sender, RoutedEventArgs e)
        {
            if(SelectedUnreviewedGuest != null)
            {
                ReviewGuest reviewGuest = new ReviewGuest(_guestReviewRepository, SelectedUnreviewedGuest, _reservationRepository, _ratingRepository);
                reviewGuest.Show();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void IObserver.Update()
        {
            Accommodations.Clear();
            FormAccommodations();

            UnreviewedGuests.Clear();
            FormUnreviewedGuests();

            Ratings.Clear();
            FormRatings();

            Requests.Clear();
            FormRescheduleRequests();
        }

        private void DecideButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRequest != null)
            {
                ReviewRequest reviewRequest = new ReviewRequest(_reservationRepository, SelectedRequest, _rescheduleRequestRepository);
                reviewRequest.Show();
            }
        }
    }
}
