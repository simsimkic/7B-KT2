using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InitialProject.Model;
using InitialProject.Resources.Observer;
using InitialProject.Serializer;
using InitialProject.View.Guest2;

namespace InitialProject.Repository
{
    public class ImageRepository: ISubject
    {
        private const string FilePath = "../../../Resources/Data/images.csv";

        private readonly Serializer<Image> _serializer;

        private List<Image> _images;

        private readonly List<IObserver> _observers;
        public ImageRepository()
        {
            _serializer = new Serializer<Image>();
            _images = _serializer.FromCSV(FilePath);
            _observers = new List<IObserver>();
        }

        public int NextId()
        {
            _images = _serializer.FromCSV(FilePath);
            if (_images.Count < 1)
            {
                return 1;
            }
            return _images.Max(c => c.Id) + 1;
        }

        public Image Save(Image image)
        {
            image.Id = NextId();

            _images = _serializer.FromCSV(FilePath);
            _images.Add(image);
            _serializer.ToCSV(FilePath, _images);
            NotifyObservers();

            return image;
        }
        public Image GetById(int id)
        {
            _images = _serializer.FromCSV(FilePath);
            return _images.Find(c => c.Id == id);
        }

        public List<Image> GetAll()
        {
            return _images;
        }

        public void Subscribe(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.Update();
            }
        }
    }
}
