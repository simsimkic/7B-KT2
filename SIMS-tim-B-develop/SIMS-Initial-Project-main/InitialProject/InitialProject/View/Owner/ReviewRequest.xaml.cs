using InitialProject.Model;
using InitialProject.Repository;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace InitialProject.View.Owner
{
    /// <summary>
    /// Interaction logic for ReviewRequest.xaml
    /// </summary>
    public partial class ReviewRequest : Window, INotifyPropertyChanged
    {
        private readonly AccommodationReservationRepository _reservationRepository;
        private readonly RescheduleRequest SelectedRequest;
        private readonly RescheduleRequestRepository _requestRepository;

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

        public ReviewRequest(AccommodationReservationRepository accommodationReservationRepository, RescheduleRequest selectedRescheduleRequest, RescheduleRequestRepository rescheduleRequestRepository)
        {
            InitializeComponent();
            DataContext = this;

            _reservationRepository = accommodationReservationRepository;
            _requestRepository = rescheduleRequestRepository;
            SelectedRequest = selectedRescheduleRequest;

            CheckAvailability();
        }

        public void CheckAvailability()
        {
            
            foreach (AccommodationReservation reservation in _reservationRepository.GetAll())
            {
                if ((SelectedRequest.NewStartDate > reservation.StartDate && SelectedRequest.NewStartDate < reservation.EndDate) || (SelectedRequest.NewEndDate > reservation.StartDate && SelectedRequest.NewEndDate < reservation.EndDate))
                {
                    textBlock.Text = "Period isn't available";
                    return;
                }
            }
            textBlock.Text = "Period is available";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnDecline_Click(object sender, RoutedEventArgs e)
        {
            SelectedRequest.Status = InitialProject.Resources.Enums.RescheduleRequestStatus.rejected;
            SelectedRequest.IsNotified = false;
            if (Comment != null)
            {
                SelectedRequest.Comment = Comment;
            }
            _requestRepository.Update(SelectedRequest);
            Close();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            SelectedRequest.Status = InitialProject.Resources.Enums.RescheduleRequestStatus.approved;
            SelectedRequest.IsNotified = false;
            if (Comment != null)
            {
                SelectedRequest.Comment = Comment;
            }
            _requestRepository.Update(SelectedRequest);
            UpdateAcceptedDates();
            Close();
        }

        private void UpdateAcceptedDates()
        {
            AccommodationReservation reservation = _reservationRepository.GetById(SelectedRequest.ReservationId);
            reservation.StartDate = SelectedRequest.NewStartDate;
            reservation.EndDate = SelectedRequest.NewEndDate;
            _reservationRepository.Update(reservation);
        }
    }
}
