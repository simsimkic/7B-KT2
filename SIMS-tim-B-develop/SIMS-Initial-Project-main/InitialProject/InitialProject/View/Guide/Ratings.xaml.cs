using InitialProject.Model;
using InitialProject.Model.DTO;
using InitialProject.Repository;
using InitialProject.Resources.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace InitialProject.View.Guide
{
    /// <summary>
    /// Interaction logic for Ratings.xaml
    /// </summary>
    public partial class Ratings : Window, INotifyPropertyChanged, IObserver
    {
        private readonly UserRepository _userRepository;
        private readonly TourRatingRepository _tourRatingRepository;
        private readonly TourReservationRepository _tourReservationRepository;
        private readonly CheckpointRepository _checkpointRepository;

        public Tour FinishedTour { get; set; }

        private ObservableCollection<GuideRatingDTO> _guestRatings;
        public ObservableCollection<GuideRatingDTO> GuestRatings
        {
            get => _guestRatings;
            set
            {
                _guestRatings = value;
                OnPropertyChanged();
            }
        }

        public GuideRatingDTO SelectedRatingDTO { get; set; }
        public Ratings(Tour finishedTour, UserRepository userRepository, TourRatingRepository tourRatingRepository, TourReservationRepository tourReservationRepository, CheckpointRepository checkpointRepository)
        {
            InitializeComponent();
            DataContext = this;
            FinishedTour = finishedTour;

            _userRepository = userRepository;
            _tourRatingRepository = tourRatingRepository;
            _tourReservationRepository = tourReservationRepository;
            _checkpointRepository = checkpointRepository;

                        _tourRatingRepository.Subscribe(this);

            GuestRatings = new ObservableCollection<GuideRatingDTO>(ConvertToDTO(_tourRatingRepository.GetTourRatings(finishedTour)));

        }
        public GuideRatingDTO ConvertToDTO(TourRating rating)
        {

            if (rating == null)
                return null;

            string checkpointName;

            if(_tourReservationRepository.GetReservationByGuestIdAndTourId(rating.UserId, rating.TourId).CheckpointArrivalId == -1)
            {
                checkpointName = "Did not arrive";
            }
            else 
            {
                checkpointName = _checkpointRepository.GetById(_tourReservationRepository.GetReservationByGuestIdAndTourId(rating.UserId, rating.TourId).CheckpointArrivalId).Order.ToString() + ". " +
                    _checkpointRepository.GetById(_tourReservationRepository.GetReservationByGuestIdAndTourId(rating.UserId, rating.TourId).CheckpointArrivalId).Name;
            }



            return new GuideRatingDTO(
                    rating.Id,
                    _userRepository.GetById(rating.UserId).Username,
                    checkpointName,
                    rating.Comment,
                    rating.GuideKnowledge,
                    rating.GuideLanguage,
                    rating.Interestingness,
                    rating.isValid,
                    _tourRatingRepository.FindRatingUrls(rating));             
        }
        public List<GuideRatingDTO> ConvertToDTO(List<TourRating> ratings)
        {
            List<GuideRatingDTO> dtos = new List<GuideRatingDTO>();

            foreach (TourRating rating in ratings)
            {
                dtos.Add(ConvertToDTO(rating));
            }
            return dtos;
        }
        public TourRating ConvertToRating(GuideRatingDTO dto)
        {
            if (dto != null)
                return _tourRatingRepository.GetById(dto.Id);
            return null;
        }
        private void RatingsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RatingsOverview overview = new RatingsOverview(SelectedRatingDTO);
            overview.Show();           
        }
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            TourRating rating = ConvertToRating(SelectedRatingDTO);
            rating.isValid = false;
            _tourRatingRepository.Update(rating);
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update()
        {
            GuestRatings.Clear();
            GuestRatings = new ObservableCollection<GuideRatingDTO>(ConvertToDTO(_tourRatingRepository.GetTourRatings(FinishedTour)));
        }
    }
}
