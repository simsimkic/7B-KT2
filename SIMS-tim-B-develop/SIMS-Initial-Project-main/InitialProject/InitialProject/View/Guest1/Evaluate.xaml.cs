using InitialProject.Model;
using InitialProject.Model.DTO;
using InitialProject.Repository;
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

namespace InitialProject.View.Guest1
{
    /// <summary>
    /// Interaction logic for Evaluate.xaml
    /// </summary>
    public partial class Evaluate : Window
    {
        private readonly AccommodationRatingsRepository _accommodationRatingsRepository;
        private readonly AccommodationReservationRepository _accommodationReservationRepository;
        private readonly AccommodationRatingsDTO SelectedUnratedAccommodation;
        private readonly ImageRepository _imageRepository;
        private readonly UserRepository _userRepository;

        public AccommodationReservation Reservation { get; set; }

        private int _cleanliness;
        public int Cleanliness
        {
            get => _cleanliness;
            set
            {
                if (value != _cleanliness)
                {
                    _cleanliness = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _correctness;
        public int Correctness
        {
            get => _correctness;
            set
            {
                if (value != _correctness)
                {
                    _correctness = value;
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

        public Evaluate(AccommodationRatingsDTO selectedUnratedAccommodation, AccommodationRatingsRepository accommodationRatingsRepository, AccommodationReservationRepository accommodationReservationRepository, ImageRepository imageRepository)
        {
            InitializeComponent();
            DataContext = this;

            SelectedUnratedAccommodation = selectedUnratedAccommodation;
            _accommodationRatingsRepository = accommodationRatingsRepository;
            _accommodationReservationRepository = accommodationReservationRepository;
            _imageRepository = imageRepository;
            _userRepository = new UserRepository();

            Reservation = _accommodationReservationRepository.GetById(SelectedUnratedAccommodation.ReservationId);
        }

        private void SetRatingsForCleanlinees()
        {
            if (cleanliness1.IsChecked == true)
            {
                Cleanliness = 1;
            }
            else if (cleanliness2.IsChecked == true)
            {
                Cleanliness = 2;
            }
            else if (cleanliness3.IsChecked == true)
            {
                Cleanliness = 3;
            }
            else if (cleanliness4.IsChecked == true)
            {
                Cleanliness = 4;
            }
            else if (cleanliness5.IsChecked == true)
            {
                Cleanliness = 5;
            }
        }

        private void SetRatingsForCorrectness()
        {
            if (correctness1.IsChecked == true)
            {
                Correctness = 1;
            }
            else if (correctness2.IsChecked == true)
            {
                Correctness = 2;
            }
            else if (correctness3.IsChecked == true)
            {
                Correctness = 3;
            }
            else if (correctness4.IsChecked == true)
            {
                Correctness = 4;
            }
            else if (correctness5.IsChecked == true)
            {
                Correctness = 5;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateComment()) return;
            var messageBoxResult = MessageBox.Show($"Would you like to save your rating?", "Rating Accommodation Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
              if (messageBoxResult == MessageBoxResult.Yes)
              {
                    SetRatingsForCleanlinees();
                    SetRatingsForCorrectness();
                    AccommodationRatings accommodationRatings = new AccommodationRatings(Reservation.Id, Reservation.AccommodationId, Reservation.OwnerId, Reservation.GuestId, Cleanliness, Correctness, Comment, _imageIds);
                    AccommodationReservation reservation = _accommodationReservationRepository.GetById(Reservation.Id);
                    reservation.IsRated = true;
                    _accommodationReservationRepository.Update(reservation);
                    _accommodationRatingsRepository.Save(accommodationRatings);
                    MessageBox.Show("Rating saved successfully.");
                    CheckForSuperOwnerPrivileges(Reservation.OwnerId);
                    Close();
            }
        }

        private void CheckForSuperOwnerPrivileges(int id)
        {
            int numberOfRatings = _accommodationRatingsRepository.GetAll().Where(x => x.OwnerId == id).Count();
            int cleanlinessSum = _accommodationRatingsRepository.GetAll().Where(x => x.OwnerId == id).Sum(x => x.Cleanliness);
            int correctnessSum = _accommodationRatingsRepository.GetAll().Where(x => x.OwnerId == id).Sum(x => x.Correctness);

            double averageRating = (cleanlinessSum + correctnessSum) / (numberOfRatings * 2);

            SetSuperOwnerPrivileges(id, numberOfRatings, averageRating);
        }

        private void SetSuperOwnerPrivileges(int id, int numberOfRatings, double averageRating)
        {
            User owner = _userRepository.GetById(id);
            if (numberOfRatings >= 50 && averageRating > 4.5)
            {
                owner.Type = InitialProject.Resources.Enums.UserType.superowner;
            }
            else
            {
                owner.Type = InitialProject.Resources.Enums.UserType.owner;
            }
            _userRepository.Update(owner);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void AddPicture_Click(object sender, RoutedEventArgs e)
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
        private bool ValidateComment()
        {
            if(string.IsNullOrEmpty(commentTextBox.Text))
            {
                ShowNoCommentWarning();
                return false;
            }
            return true;
        }
        private void ShowNoCommentWarning()
        {
            MessageBox.Show("Please enter a comment.", "Comment warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
