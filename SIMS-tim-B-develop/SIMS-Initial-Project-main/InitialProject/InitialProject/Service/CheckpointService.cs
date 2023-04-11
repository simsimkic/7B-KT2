using InitialProject.Model;
using InitialProject.Repository;
using InitialProject.Resources.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Service
{
    public class CheckpointService : ISubject
    {
        private readonly CheckpointRepository _checkpointRepository;

        public CheckpointService() 
        {
            _checkpointRepository = new CheckpointRepository();
        }
        public void NotifyObservers()
        {
            _checkpointRepository.NotifyObservers();
        }

        public void Subscribe(IObserver observer)
        {
            _checkpointRepository.Subscribe(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            _checkpointRepository.Unsubscribe(observer);
        }
    }
}
