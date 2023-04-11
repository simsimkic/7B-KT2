using InitialProject.Model;
using InitialProject.Model.DTO;
using InitialProject.Repository;
using InitialProject.Resources.Observer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
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
using System.Windows.Threading;
using Image = System.Windows.Controls.Image;

namespace InitialProject.View.Guide
{
    /// <summary>
    /// Interaction logic for RatingsOverview.xaml
    /// </summary>
    public partial class RatingsOverview : Window
    {
        public GuideRatingDTO SelectedDTO { get; set; }
        public int imageIndex { get; set; }

        public List<string> Urls { get; set; }
        public RatingsOverview(GuideRatingDTO guideRatingDTO)
        {
            InitializeComponent();
            DataContext = this;
            SelectedDTO = guideRatingDTO;

            imageIndex = 0;
            Urls = guideRatingDTO.Urls;

            SetImage(imageIndex);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            imageIndex--;
            if (imageIndex < 0)
            {
                imageIndex = Urls.Count - 1;
            }

            SetImage(imageIndex);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            imageIndex++;
            if (imageIndex > Urls.Count - 1)
            {
                imageIndex = 0;
            }

            SetImage(imageIndex);
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void SetImage(int index)
        {
            var image = new Image();
            try
            {
                var fullFilePath = Urls[index];

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                bitmap.EndInit();

                image.Source = bitmap;
            }
            catch (Exception ex)
            {
                // Handle the exception by setting the Source to a default image
                BitmapImage defaultImage = new BitmapImage(new Uri("/Resources/Images/image_unavailable.png", UriKind.Relative));
                image.Source = defaultImage;
            }

            picHolder.Source = image.Source;
        }

    }
}
