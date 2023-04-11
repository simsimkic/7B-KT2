using InitialProject.Model;
using InitialProject.Model.DTO;
using InitialProject.Repository;
using InitialProject.Service;
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
using Image = InitialProject.Model.Image;

namespace InitialProject.View.Guest2
{
    public partial class RateTour : Window
    {
        public User LoggedInUser { get; set; }
        public Guest2TourDTO SelectedTour { get; set; }

        private readonly TourRatingRepository _tourRatingRepository;
        private readonly TourReservationRepository _tourReservationRepository;
        private readonly TourService _tourService;
        private readonly ImageRepository _imageRepository;

        private int _guideKnowledge;
        public int GuideKnowledge
        {
            get => _guideKnowledge;
            set
            {
                if (_guideKnowledge != value)
                {
                    _guideKnowledge = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _guideLanguage;
        public int GuideLanguage
        {
            get => _guideLanguage;
            set
            {
                if (_guideLanguage != value)
                {
                    _guideLanguage = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _interestingness;
        public int Interestingness
        {
            get => _interestingness;
            set
            {
                if (_interestingness != value)
                {
                    _interestingness = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set
            {
                if (value != _comment)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }

        public RateTour(Guest2TourDTO selectedTour, User user, TourRatingRepository tourRatingRepository, TourReservationRepository tourReservationRepository, TourService tourService, ImageRepository imageRepository)
        {
            InitializeComponent();
            DataContext = this;

            SelectedTour = selectedTour;
            LoggedInUser = user;

            _tourRatingRepository = tourRatingRepository;
            _tourReservationRepository = tourReservationRepository;
            _tourService = tourService;
            _imageRepository = imageRepository;
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetRatingForGuideKnowledge()
        {
            if (knowledge1.IsChecked == true)
                GuideKnowledge = 1;
            else if (knowledge2.IsChecked == true)
                GuideKnowledge = 2;
            else if (knowledge3.IsChecked == true)
                GuideKnowledge = 3;
            else if (knowledge4.IsChecked == true)
                GuideKnowledge = 4;
            else if (knowledge5.IsChecked == true)
                GuideKnowledge = 5;
        }

        private void SetRatingForGuideLanguage()
        {
            if (language1.IsChecked == true)
                GuideLanguage = 1;
            else if (language2.IsChecked == true)
                GuideLanguage = 2;
            else if (language3.IsChecked == true)
                GuideLanguage = 3;
            else if (language4.IsChecked == true)
                GuideLanguage = 4;
            else if (language5.IsChecked == true)
                GuideLanguage = 5;
        }

        private void SetRatingForInterestingness()
        {
            if (interestingness1.IsChecked == true)
                Interestingness = 1;
            else if (interestingness2.IsChecked == true)
                Interestingness = 2;
            else if (interestingness3.IsChecked == true)
                Interestingness = 3;
            else if (interestingness4.IsChecked == true)
                Interestingness = 4;
            else if (interestingness5.IsChecked == true)
                Interestingness = 5;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Would you like to save your rating?", "Rating Tour Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SetRatingForGuideKnowledge();
                SetRatingForGuideLanguage();
                SetRatingForInterestingness();

                TourRating tourRating = new TourRating(
                        SelectedTour.TourId,
                        GuideKnowledge,
                        GuideLanguage,
                        Interestingness,
                        Comment,
                        _imageIds,
                        LoggedInUser.Id);

                Tour tour = _tourService.GetById(tourRating.TourId);
                tour.IsRated = true;
                _tourService.Update(tour);

                TourReservation tourReservation = _tourReservationRepository.GetReservationByGuestIdAndTourId(LoggedInUser.Id, SelectedTour.TourId);
                tourReservation.IsRated = true;
                _tourReservationRepository.Update(tourReservation);

                _tourRatingRepository.Save(tourRating);

                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
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
    }
}
