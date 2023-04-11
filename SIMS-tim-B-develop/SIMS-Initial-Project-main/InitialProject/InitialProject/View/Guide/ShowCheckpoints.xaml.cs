using Accessibility;
using InitialProject.Model;
using InitialProject.Model.DTO;
using InitialProject.Repository;
using InitialProject.Resources.Observer;
using InitialProject.Resources.UIHelper;
using InitialProject.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace InitialProject.View.Guide
{
    public partial class ShowCheckpoints : Window, INotifyPropertyChanged, IObserver
    {
        private readonly Tour SelectedTour;

        private readonly CheckpointRepository _repository;
        private readonly TourService _tourService;
        private readonly TourReservationRepository _tourReservationRepository;
        private readonly UserRepository _userRepository;
        public ObservableCollection<Checkpoint> Checkpoints { get; set; }

        private ObservableCollection<UserDTO> unmarkedGuests;
        public ObservableCollection<UserDTO> UnmarkedGuests
        {
            get { return unmarkedGuests; }
            set
            {
                if (unmarkedGuests != value)
                {
                    unmarkedGuests = value;
                    OnPropertyChanged(nameof(UnmarkedGuests));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ShowCheckpoints(Tour tour, CheckpointRepository checkpointRepository, TourService tourService, TourReservationRepository tourReservationRepository, UserRepository userRepository, TourRatingRepository tourRatingRepository)
        {
            InitializeComponent();
            DataContext = this;

            SelectedTour = tour;
            _repository = checkpointRepository;
            _tourService = tourService;
            _tourReservationRepository = tourReservationRepository;
            _userRepository = userRepository;


            InitializeCollections();
            LoadUnmarkedGuests();
            LoadTourCheckpoints();

            // Attach the LOADED EVENT handler to the ListBox, this is becaue of the visuals bg color, border, checked
            listBox.Loaded += ListBox_Loaded;
        }
        private void InitializeCollections()
        {
            Checkpoints = new ObservableCollection<Checkpoint>();
            UnmarkedGuests = new ObservableCollection<UserDTO>();
        }
        private void LoadUnmarkedGuests()
        {
            List<int> unmarkedGuestsId = _tourReservationRepository.GetUserIdsByTour(SelectedTour);

            foreach (int id in unmarkedGuestsId)
            {
                User user = _userRepository.GetById(id);
                UserDTO userDto = ConvertUserToDTO(user);

                if(!HasUser(userDto.UserId))
                    UnmarkedGuests.Add(userDto);
            }
        }

        private bool HasUser(int userId) 
        {
            foreach(UserDTO user in UnmarkedGuests) 
            {
                if(user.UserId == userId) 
                {
                    return true;
                }
            }
            return false;
        }
        private void LoadTourCheckpoints()
        {
            foreach (int id in SelectedTour.CheckpointIds)
            {
                Checkpoint checkpoint = _repository.GetById(id);
                Checkpoints.Add(checkpoint);
            }
            Checkpoints.OrderBy(c => c.Order);
        }
        public UserDTO ConvertUserToDTO(User user) 
        {
            int checkpointId = _tourReservationRepository.GetReservationByGuestIdAndTourId(user.Id, SelectedTour.Id).CheckpointArrivalId;
            String checkpointName;
            if(checkpointId == -1) 
            {
                checkpointName = "Not Arrived Yet";
            }
            else 
            {
                checkpointName = _repository.GetById(_tourReservationRepository.GetReservationByGuestIdAndTourId(user.Id, SelectedTour.Id).CheckpointArrivalId).Name;
            }
            return new UserDTO(
                user.Id,
                user.Username,
                checkpointName
                );
        }
        // This method is called when the ListBox is loaded into the user interface. It sets the VISUAL APPEARANCE of each ListBox item
        // based on the current state of the application, including the currently selected checkpoint and the first checkpoint in the list.
        // The method iterates through each ListBox item, gets the corresponding Checkpoint, CheckBox, and ListBoxItem controls, and calls 
        // the SetCheckpointCheckboxAndBackground method to set the state of the CheckBox and the background color of the ListBoxItem.
        private void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            int currentCheckpointId = SelectedTour.CurrentCheckpointId;
            int firstCheckpointId = Checkpoints.Min(c => c.Id);
            ListBoxItem previousListBoxItem = null;

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                Checkpoint checkpoint = (Checkpoint)listBox.Items[i];
                CheckBox checkbox = UIHelper.FindVisualChild<CheckBox>(listBox.ItemContainerGenerator.ContainerFromIndex(i));
                ListBoxItem listBoxItem = listBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

                SetCheckpointCheckboxAndBackground(checkpoint, checkbox, listBoxItem, currentCheckpointId, firstCheckpointId, ref previousListBoxItem);
            }
        }
        private void SetCheckpointCheckboxAndBackground(Checkpoint checkpoint, CheckBox checkbox, ListBoxItem listBoxItem, int currentCheckpointId, int firstCheckpointId, ref ListBoxItem previousListBoxItem)
        {
            if (checkpoint.Id <= currentCheckpointId)
            {
                DisableCheckboxAndSetBackgroundGray(checkbox, listBoxItem);
            }
            else if (checkpoint.Id == currentCheckpointId + 1)
            {
                EnableCheckboxAndSetBackgroundWhite(checkbox, listBoxItem);
            }
            else
            {
                SetCheckboxAndSetBackgroundWhite(checkbox, listBoxItem, checkpoint.Id == firstCheckpointId + 1);
            }
            if (checkpoint.Id == SelectedTour.CurrentCheckpointId)
            {
                SetListBoxItemBorder(listBoxItem, previousListBoxItem);
                previousListBoxItem = listBoxItem;
            }
        }
        private void DisableCheckboxAndSetBackgroundGray(CheckBox checkbox, ListBoxItem listBoxItem)
        {
            DisableCheckbox(checkbox, listBoxItem);
            SetBackground(listBoxItem, Brushes.LightGray);
        }
        private void EnableCheckboxAndSetBackgroundWhite(CheckBox checkbox, ListBoxItem listBoxItem)
        {
            EnableCheckbox(checkbox, listBoxItem);
            SetBackground(listBoxItem, Brushes.White);
        }
        private void SetCheckboxAndSetBackgroundWhite(CheckBox checkbox, ListBoxItem listBoxItem, bool isEnabled)
        {
            SetCheckbox(checkbox, isEnabled, listBoxItem);
            SetBackground(listBoxItem, Brushes.White);
        }
        private void DisableCheckbox(CheckBox checkbox, ListBoxItem listBoxItem)
        {
            checkbox.IsChecked = true;
            checkbox.IsEnabled = false;
            SetBackground(listBoxItem, Brushes.LightGray);
        }
        private void EnableCheckbox(CheckBox checkbox, ListBoxItem listBoxItem)
        {
            checkbox.IsChecked = false;
            checkbox.IsEnabled = true;
            SetBackground(listBoxItem, Brushes.White);
        }
        private void SetCheckbox(CheckBox checkbox, bool isEnabled, ListBoxItem listBoxItem)
        {
            checkbox.IsChecked = false;
            checkbox.IsEnabled = isEnabled;
            SetBackground(listBoxItem, Brushes.White);
        }
        private void SetBackground(ListBoxItem listBoxItem, Brush brush)
        {
            if (listBoxItem != null)
            {
                listBoxItem.Background = brush;
            }
        }
        private void SetListBoxItemBorder(ListBoxItem listBoxItem, ListBoxItem previousListBoxItem)
        {
            if (listBoxItem != null)
            {
                listBoxItem.BorderThickness = new Thickness(3);
                listBoxItem.BorderBrush = Brushes.Blue;

                if (previousListBoxItem != null)
                {
                    previousListBoxItem.BorderThickness = new Thickness(0);
                    previousListBoxItem.BorderBrush = null;
                }
            }
        }
        private void checkpointCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckbox = (CheckBox)sender;
            currentCheckbox.IsEnabled = false;

            int currentIndex = GetCurrentIndex(currentCheckbox);
            int nextIndex = currentIndex + 1;

            if (nextIndex >= Checkpoints.Count)
            {
                EndTour();
                return;
            }

            UpdateNextCheckpoint(nextIndex);

            DisablePreviousCheckpoints(currentIndex);
            DisableFollowingCheckpoints(nextIndex);

            UpdateCurrentCheckpoint(currentIndex);

            UpdateListBoxItemBackgrounds(currentIndex);
            RemovePreviousListBoxItemBorder(currentIndex);
        }
        private int GetCurrentIndex(CheckBox currentCheckbox)
        {
            return listBox.ItemContainerGenerator.IndexFromContainer(
                listBox.ItemContainerGenerator.ContainerFromItem(currentCheckbox.DataContext));
        }
        private void EndTour()
        {
            UpdateTourStatus();
            Close();
        }
        private void UpdateTourStatus()
        {
            SelectedTour.IsActive = false;
            SelectedTour.IsFinished = true;
            _tourService.Update(SelectedTour);
        }
        private void UpdateNextCheckpoint(int nextIndex)
        {
            CheckBox nextCheckbox = UIHelper.FindVisualChild<CheckBox>(listBox.ItemContainerGenerator.ContainerFromIndex(nextIndex));
            nextCheckbox.IsEnabled = true;

            SelectedTour.CurrentCheckpointId = Checkpoints[nextIndex].Id;
            _tourService.Update(SelectedTour);
        }

        private void DisablePreviousCheckpoints(int currentIndex)
        {
            for (int i = 0; i < currentIndex; i++)
            {
                CheckBox checkbox = UIHelper.FindVisualChild<CheckBox>(listBox.ItemContainerGenerator.ContainerFromIndex(i));
                checkbox.IsChecked = true;
                checkbox.IsEnabled = false;
            }
        }
        private void DisableFollowingCheckpoints(int nextIndex)
        {
            for (int i = nextIndex + 1; i < Checkpoints.Count; i++)
            {
                CheckBox checkbox = UIHelper.FindVisualChild<CheckBox>(listBox.ItemContainerGenerator.ContainerFromIndex(i));
                checkbox.IsChecked = false;
                checkbox.IsEnabled = false;
            }
        }
        private void UpdateCurrentCheckpoint(int currentIndex)
        {
            SelectedTour.CurrentCheckpointId = Checkpoints[currentIndex].Id;
            _tourService.Update(SelectedTour);
        }
        private void UpdateListBoxItemBackgrounds(int currentIndex)
        {
            ListBox_Loaded(listBox, null);
        }
        private void RemovePreviousListBoxItemBorder(int currentIndex)
        {
            if (currentIndex > 0)
            {
                ListBoxItem previousListBoxItem = listBox.ItemContainerGenerator.ContainerFromIndex(currentIndex - 1) as ListBoxItem;
                ListBoxItem currentListBoxItem = listBox.ItemContainerGenerator.ContainerFromIndex(currentIndex) as ListBoxItem;

                if (previousListBoxItem != null && currentListBoxItem != null && previousListBoxItem != currentListBoxItem)
                {
                    previousListBoxItem.BorderThickness = new Thickness(0);
                    previousListBoxItem.BorderBrush = null;
                }
            }
        }
        private void endTourButton_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = ShowEndTourConfirmationMessage();

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                EndTour();
            }
        }
        private void presentButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateReservationCheckpointArrivalId();
            UpdateGuestCheckpointArrivalNameInUnmarkedGuests();
            UpdateGuestsGrid();
        }
        private void UpdateReservationCheckpointArrivalId()
        {
            var selectedGuest = guestsGrid.SelectedItem as UserDTO;
            if (selectedGuest != null)
            {
                int currentCheckpointId = SelectedTour.CurrentCheckpointId;
                TourReservation reservation = GetReservationByGuestIdAndTourId(selectedGuest.UserId, SelectedTour.Id);
                UpdateReservationCheckpointArrival(reservation, currentCheckpointId);
            }
        }
        private TourReservation GetReservationByGuestIdAndTourId(int guestId, int tourId)
        {
            return _tourReservationRepository.GetReservationByGuestIdAndTourId(guestId, tourId);
        }
        private void UpdateReservationCheckpointArrival(TourReservation reservation, int checkpointId)
        {
            reservation.CheckpointArrivalId = checkpointId;
            _tourReservationRepository.Update(reservation);
        }
        private void UpdateGuestCheckpointArrivalNameInUnmarkedGuests()
        {
            var selectedGuest = guestsGrid.SelectedItem as UserDTO;
            if (selectedGuest != null)
            {
                string checkpointName = GetCheckpointNameById(SelectedTour.CurrentCheckpointId);
                UpdateGuestCheckpointArrivalName(selectedGuest.UserId, checkpointName);
            }
        }
        private string GetCheckpointNameById(int checkpointId)
        {
            return _repository.GetById(checkpointId).Name;
        }
        private void UpdateGuestCheckpointArrivalName(int guestId, string checkpointName)
        {
            foreach (UserDTO guest in UnmarkedGuests)
            {
                if (guest.UserId == guestId)
                {
                    guest.CheckpointArrivalName = checkpointName;
                    TourReservation reservation = _tourReservationRepository.GetReservationByGuestIdAndTourId(guestId, SelectedTour.Id);
                    reservation.GuestChecked = true;
                    _tourReservationRepository.Update(reservation);
                }
            }
        }
        private void UpdateGuestsGrid()
        {
            Update();
        }
        public void Update()
        {
            UnmarkedGuests.Clear();

            foreach (int id in _tourReservationRepository.GetUserIdsByTour(SelectedTour))
            {
                if (!UnmarkedGuests.Contains(ConvertUserToDTO(_userRepository.GetById(id))))
                    UnmarkedGuests.Add(ConvertUserToDTO(_userRepository.GetById(id)));
            }
        }
        private MessageBoxResult ShowEndTourConfirmationMessage()
        {
            return MessageBox.Show($"Are you sure you want to finish the {SelectedTour.Name} tour?", "Finish Tour Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}
