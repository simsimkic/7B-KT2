using InitialProject.Model;
using InitialProject.Model.DTO;
using InitialProject.Resources.Observer;
using InitialProject.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace InitialProject.Repository
{
    public class TourRepository : ISubject
    {
        private const string _filePath = "../../../Resources/Data/tours.csv";

        private readonly Serializer<Tour> _serializer;
        private readonly List<IObserver> _observers;

        private List<Tour> _tours;

        public TourRepository()
        {
            _serializer = new Serializer<Tour>();
            _tours = _serializer.FromCSV(_filePath);
            _observers = new List<IObserver>();
        }

        public int NextId()
        {
            _tours = _serializer.FromCSV(_filePath);
            if (_tours.Count < 1)
            {
                return 1;
            }
            return _tours.Max(c => c.Id) + 1;
        }

        public Tour Save(Tour tour)
        {
            tour.Id = NextId();
            _tours = _serializer.FromCSV(_filePath);
            _tours.Add(tour);
            _serializer.ToCSV(_filePath, _tours);
            NotifyObservers();

            return tour;
        }
        public Tour Update(Tour tour)
        {
            _tours = _serializer.FromCSV(_filePath);
            Tour current = _tours.Find(c => c.Id == tour.Id);
            int index = _tours.IndexOf(current);
            _tours.Remove(current);
            _tours.Insert(index, tour);       // keep ascending order of ids in file 
            _serializer.ToCSV(_filePath, _tours);
            NotifyObservers();
            return tour;
        }
        public Tour GetById(int id)
        {
            _tours = _serializer.FromCSV(_filePath);
            return _tours.Find(c => c.Id == id);
        }

        public List<Tour> GetAll()
        {
            return _tours;
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
