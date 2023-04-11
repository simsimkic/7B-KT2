using System;
using System.Collections.Generic;
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

namespace InitialProject.View.Guest1
{
    /// <summary>
    /// Interaction logic for AccommodationImages.xaml
    /// </summary>
    public partial class AccommodationImages : Window
    {
        public int imageIndex { get; set; }

        public List<string> Urls { get; set; }
        public AccommodationImages(List<string> urls)
        {
            InitializeComponent();
            DataContext = this;

            imageIndex = 0;
            Urls = urls;

            var image = new Image();
            var fullFilePath = urls[imageIndex];

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            image.Source = bitmap;
            picHolder.Source = image.Source;

        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            imageIndex--;

            if (imageIndex < 0)
            {
                imageIndex = Urls.Count - 1;
            }

            var image = new Image();
            var fullFilePath = Urls[imageIndex];

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            image.Source = bitmap;
            picHolder.Source = image.Source;

        }

        private void nextButton_Click_1(object sender, RoutedEventArgs e)
        {
            imageIndex++;
            if (imageIndex > Urls.Count - 1)
            {
                imageIndex = 0;
            }

            var image = new Image();
            var fullFilePath = Urls[imageIndex];

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            image.Source = bitmap;
            picHolder.Source = image.Source;
        }
    }
}
