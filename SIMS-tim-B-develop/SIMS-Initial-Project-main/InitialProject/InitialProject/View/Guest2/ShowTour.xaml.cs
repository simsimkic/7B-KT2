using InitialProject.Model;
using InitialProject.Model.DTO;
using InitialProject.Repository;
using InitialProject.Resources.Observer;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace InitialProject.View.Guest2
{
    public partial class ShowTour : Window, INotifyPropertyChanged, IObserver
    {
        public Guest2TourDTO tourDTO { get; set; }

        private readonly TourRepository _tourRepository;
        private readonly ImageRepository _imageRepository;

        public ShowTour(Guest2TourDTO tourDTO)
        {
            InitializeComponent();
            DataContext = this;

            _tourRepository = new TourRepository();
            _tourRepository.Subscribe(this);

            _imageRepository = new ImageRepository();
            _imageRepository.Subscribe(this);

            this.tourDTO = tourDTO;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ImagesButton_Click(object sender, RoutedEventArgs e)
        {
            Tour selectedTour = ConvertToTour(tourDTO);

            List<string> imageUrls = new List<string>();

            foreach (int imageId in selectedTour.ImageIds)
            {
                if (_imageRepository.GetById(imageId) != null)
                    imageUrls.Add(_imageRepository.GetById(imageId).Url);
            }

            TourImages tourImages = new TourImages(imageUrls);
            tourImages.Show();
        }

        public Tour ConvertToTour(Guest2TourDTO dto)
        {
            return _tourRepository.GetById(dto.TourId);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update()
        {
            //
        }
    }
}
