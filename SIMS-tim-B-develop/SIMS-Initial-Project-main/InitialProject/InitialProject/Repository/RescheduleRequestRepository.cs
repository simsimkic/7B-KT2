using InitialProject.Model;
using InitialProject.Resources.Observer;
using InitialProject.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace InitialProject.Repository
{
    public class RescheduleRequestRepository : ISubject
    {
        private const string FilePath = "../../../Resources/Data/rescheduleRequests.csv";

        private readonly Serializer<RescheduleRequest> _serializer;

        private List<RescheduleRequest> _requests;

        private readonly List<IObserver> _observers;

        public RescheduleRequestRepository()
        {
            _serializer = new Serializer<RescheduleRequest>();
            _requests = _serializer.FromCSV(FilePath);
            _observers = new List<IObserver>();
        }

        public int NextId()
        {
            _requests = _serializer.FromCSV(FilePath);
            if (_requests.Count < 1)
            {
                return 1;
            }
            return _requests.Max(c => c.Id) + 1;
        }

        public RescheduleRequest Save(RescheduleRequest request)
        {
            request.Id = NextId();

            _requests = _serializer.FromCSV(FilePath);
            _requests.Add(request);
            _serializer.ToCSV(FilePath, _requests);
            NotifyObservers();

            return request;
        }

        public List<RescheduleRequest> GetAll()
        {
            return _requests;
        }

        public RescheduleRequest GetById(int id)
        {
            _requests = _serializer.FromCSV(FilePath);
            return _requests.FirstOrDefault(u => u.Id == id);
        }

        public RescheduleRequest Update(RescheduleRequest request)
        {
            _requests = _serializer.FromCSV(FilePath);
            RescheduleRequest current = _requests.Find(c => c.Id == request.Id);
            int index = _requests.IndexOf(current);
            _requests.Remove(current);
            _requests.Insert(index, request);       // keep ascending order of ids in file 
            _serializer.ToCSV(FilePath, _requests);
            NotifyObservers();
            return request;
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
