using InitialProject.Model;
using InitialProject.Resources.Observer;
using InitialProject.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InitialProject.Repository
{
    public class CheckpointRepository: ISubject
    {
        private const string _filepath = "../../../Resources/Data/checkpoints.csv";

        private readonly Serializer<Checkpoint> _serializer;

        private readonly List<IObserver> _observers;
        private readonly TourRepository _tourRepository;

        private List<Checkpoint> _checkpoints;
        public CheckpointRepository()
        {
            _serializer = new Serializer<Checkpoint>();
            _checkpoints = _serializer.FromCSV(_filepath);
            _tourRepository = new TourRepository();
            _observers = new List<IObserver>();
        }

        public int NextId()
        {
            _checkpoints = _serializer.FromCSV(_filepath);
            if (_checkpoints.Count < 1)
            {
                return 1;
            }
            return _checkpoints.Max(c => c.Id) + 1;
        }

        public Checkpoint Save(Checkpoint checkpoint)
        {
            checkpoint.Id = NextId();
            _checkpoints = _serializer.FromCSV(_filepath);
            _checkpoints.Add(checkpoint);
            _serializer.ToCSV(_filepath, _checkpoints);
            NotifyObservers();

            return checkpoint;
        }
        public Checkpoint Update(Checkpoint checkpoint)
        {
            _checkpoints = _serializer.FromCSV(_filepath);
            Checkpoint current = _checkpoints.Find(c => c.Id == checkpoint.Id);
            int index = _checkpoints.IndexOf(current);
            _checkpoints.Remove(current);
            _checkpoints.Insert(index, checkpoint);       // keep ascending order of ids in file 
            _serializer.ToCSV(_filepath, _checkpoints);
            NotifyObservers();
            return checkpoint;
        }

        public Checkpoint GetById(int id)
        {
            _checkpoints = _serializer.FromCSV(_filepath);
            return _checkpoints.Find(c => c.Id == id);
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
