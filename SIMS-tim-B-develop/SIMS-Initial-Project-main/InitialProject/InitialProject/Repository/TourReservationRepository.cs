using InitialProject.Model;
using InitialProject.Resources.Observer;
using InitialProject.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Repository
{
    public class TourReservationRepository : ISubject
    {
        private const string _filePath = "../../../Resources/Data/tourReservations.csv";

        private readonly Serializer<TourReservation> _serializer;
        private readonly List<IObserver> _observers;

        private List<TourReservation> _tourReservations;

        public TourReservationRepository()
        {
            _serializer = new Serializer<TourReservation>();
            _tourReservations = _serializer.FromCSV(_filePath);
            _observers = new List<IObserver>();
        }

        public List<TourReservation> RemoveById(List<TourReservation> tourReservations, int id)
        {
            List<TourReservation> tourReservationsRemoved = tourReservations;
            tourReservationsRemoved.RemoveAll(t => t.Id == id);
            return tourReservationsRemoved;
        }

        public int NextId()
        {
            _tourReservations = _serializer.FromCSV(_filePath);
            if (_tourReservations.Count < 1)
            {
                return 1;
            }
            return _tourReservations.Max(c => c.Id) + 1;
        }

        public TourReservation GetByTourId(int id)
        {
            _tourReservations = _serializer.FromCSV(_filePath);
            return _tourReservations.Find(c => c.TourId == id);
        }

        public TourReservation Save(TourReservation tourReservation)
        {
            tourReservation.Id = NextId();
            _tourReservations = _serializer.FromCSV(_filePath);
            _tourReservations.Add(tourReservation);
            _serializer.ToCSV(_filePath, _tourReservations);
            NotifyObservers();

            return tourReservation;
        }
        public TourReservation Update(TourReservation reservation)
        {
            _tourReservations = _serializer.FromCSV(_filePath);
            TourReservation current = _tourReservations.Find(c => c.Id == reservation.Id);
            int index = _tourReservations.IndexOf(current);
            _tourReservations.Remove(current);
            _tourReservations.Insert(index, reservation);       // keep ascending order of ids in file 
            _serializer.ToCSV(_filePath, _tourReservations);
            NotifyObservers();
            return reservation;
        }
        public List<int> GetUserIdsByTour(Tour tour) 
        {
            List<int> userIds = new List<int>();

            foreach(TourReservation reservation in _tourReservations) 
            {
                if(reservation.TourId == tour.Id && !reservation.GuestChecked) 
                {
                    userIds.Add(reservation.UserId);
                }   
            }
            return userIds;
        }

        public List<int> GetCheckedTourIds(User user)
        {
            List<int> tourIds = new List<int>();

            foreach (TourReservation reservation in _tourReservations)
            {
                if (reservation.UserId == user.Id && reservation.GuestChecked)
                {
                    tourIds.Add(reservation.TourId);
                }
            }

            return tourIds;
        }

        public TourReservation GetReservationByGuestIdAndTourId(int guestId, int tourId)
        {
            foreach(TourReservation tourReservation in _tourReservations) 
            {
                if(guestId == tourReservation.UserId && tourId == tourReservation.TourId) 
                {
                    return tourReservation;
                }
            }
            return null;
        }
        public List<TourReservation> GetReservationsByTourId(int tourId)
        {
            List<TourReservation> reservations = new List<TourReservation>();

            foreach (TourReservation tourReservation in _tourReservations)
            {
                if (tourId == tourReservation.TourId)
                {
                   reservations.Add(tourReservation);
                }
            }
            return reservations;
        }
        
        public int GetUnder18Count(Tour tour) 
        {
            int counter = 0;
            foreach(TourReservation reservation in _tourReservations) 
            {
                if(tour.Id == reservation.TourId && reservation.AverageAge < 18) 
                {
                    counter++;
                }
            }
            return counter;
        }
        public int GetBetween18And50Count(Tour tour)
        {
            int counter = 0;
            foreach (TourReservation reservation in _tourReservations)
            {
                if (tour.Id == reservation.TourId && reservation.AverageAge >= 18 && reservation.AverageAge <= 50)
                {
                    counter++;
                }
            }
            return counter;
        }
        public int GetAbove50Count(Tour tour)
        {
            int counter = 0;
            foreach (TourReservation reservation in _tourReservations)
            {
                if (tour.Id == reservation.TourId && reservation.AverageAge > 50)
                {
                    counter++;
                }
            }
            return counter;
        }
        public int GetUsedVoucherCount(Tour tour)
        {
            int counter = 0;
            foreach (TourReservation reservation in _tourReservations)
            {
                if (tour.Id == reservation.TourId && reservation.UsedVoucherId != -1)
                {
                    counter++;
                }
            }
            return counter;
        }
        public int GetUnusedVoucherCount(Tour tour)
        {
            int counter = 0;
            foreach (TourReservation reservation in _tourReservations)
            {
                if (tour.Id == reservation.TourId && reservation.UsedVoucherId == -1)
                {
                    counter++;
                }
            }
            return counter;
        }
        public List<TourReservation> GetAll()
        {
            return _tourReservations;
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
