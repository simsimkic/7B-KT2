using InitialProject.Model;
using InitialProject.Repository;
using InitialProject.Resources.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Service
{
    public class TourService : ISubject
    {
        private readonly TourRepository _tourRepository;
        private readonly LocationRepository _locationRepository;
        private readonly TourReservationRepository _tourReservationRepository;

        public TourService()
        {
            _tourRepository = new TourRepository();
            _locationRepository = new LocationRepository();
            _tourReservationRepository = new TourReservationRepository();
        }

        public List<Tour> GetUserTours(User user)
        {
            List<Tour> tours = new List<Tour>();
            List<TourReservation> tourReservations = _tourReservationRepository.GetAll();

            foreach (TourReservation tourReservation in tourReservations)
            {
                if (tourReservation.UserId == user.Id && !tourReservation.IsRated)
                {
                    tours.Add(GetById(tourReservation.TourId));
                }
            }

            return tours;
        }

        public List<Tour> GetFinishedTours(List<Tour> tours)
        {
            return tours.FindAll(tour => tour.IsFinished);
        }

        public bool CheckIfTourCanBeAborted(Tour tour) 
        {
            return tour.StartTime.AddDays(-2) >= DateTime.Now;
        }

        public List<Tour> RemoveFromListById(List<Tour> tours, int id)
        {
            List<Tour> toursRemoved = tours;
            toursRemoved.RemoveAll(t => t.Id == id);
            return toursRemoved;
        }

        public List<Tour> GetTodaysTours()
        {
            var currentDateTime = DateTime.Now;
            var currentTours = new List<Tour>();

            foreach (var tour in _tourRepository.GetAll())
            {
                if (tour.StartTime.Date == currentDateTime.Date && tour.StartTime >= currentDateTime && !tour.IsFinished)
                {
                    currentTours.Add(tour);
                }
            }

            return currentTours;
        }
        public List<Tour> GetFinishedTours()
        {
            var finishedTours = new List<Tour>();

            foreach (var tour in _tourRepository.GetAll())
            {
                if (tour.IsFinished)
                {
                    finishedTours.Add(tour);
                }
            }

            return finishedTours;
        }
        public List<Tour> GetRatedTours()
        {
            var ratedTours = new List<Tour>();

            foreach (var tour in GetFinishedTours())
            {
                if (tour.IsRated)
                {
                    ratedTours.Add(tour);
                }
            }

            return ratedTours;
        }
        public List<Tour> GetUpcomingTours()
        {
            var upcomingTours = new List<Tour>();
            var currentDate = DateTime.Now.Date;

            foreach (var tour in _tourRepository.GetAll())
            {
                var tourStartDate = tour.StartTime.Date;
                if (tourStartDate > currentDate && !tour.IsAborted)
                {
                    upcomingTours.Add(tour);
                }
            }
            return upcomingTours;
        }
        public List<Tour> GetReservableTours()
        {
            var upcomingTours = new List<Tour>();
            var currentDate = DateTime.Now.Date;

            foreach (var tour in _tourRepository.GetAll())
            {
                var tourStartDate = tour.StartTime.Date;
                if (tourStartDate >= currentDate && !tour.IsAborted && !tour.IsFinished)
                {
                    upcomingTours.Add(tour);
                }
            }
            return upcomingTours;
        }
        public List<Tour> GetByCityName(string city)
        {
            var toursByCityName = new List<Tour>();

            foreach (var tour in GetReservableTours())
            {
                if (_locationRepository.GetById(tour.LocationId).City == city)
                {
                    toursByCityName.Add(tour);
                }
            }

            return toursByCityName;
        }
        public List<Tour> GetByCountryName(string country)
        {
            var tours = new List<Tour>();

            foreach (var tour in GetReservableTours())
            {
                if (_locationRepository.GetById(tour.LocationId).Country == country)
                {
                    tours.Add(tour);
                }
            }

            return tours;
        }
        public List<Tour> GetByLanguage(string language)
        {
            var tours = new List<Tour>();

            foreach (var tour in GetReservableTours())
            {
                if (tour.Language == language)
                {
                    tours.Add(tour);
                }
            }

            return tours;
        }
        public List<Tour> GetByDuration(double duration)
        {
            var tours = new List<Tour>();

            foreach (var tour in GetReservableTours())
            {
                if (tour.Duration == duration)
                {
                    tours.Add(tour);
                }
            }

            return tours;
        }
        public List<Tour> GetByGuests(int guestCount)
        {
            var tours = new List<Tour>();

            foreach (var tour in GetReservableTours())
            {
                if (guestCount <= tour.MaxGuests - tour.CurrentGuestCount)
                {
                    tours.Add(tour);
                }
            }

            return tours;
        }
        public List<Tour> GetToursByYear(int year) 
        {
            List<Tour> toursThisYear = new List<Tour>();

            DateTime LeftBoundary = new DateTime(year,1,1);
            DateTime RightBoundary = new DateTime(year, 12, 31);

            foreach (Tour tour in GetFinishedTours()) 
            {
                if(tour.StartTime >= LeftBoundary && tour.StartTime <= RightBoundary) 
                {
                    toursThisYear.Add(tour);
                }
            }
            return toursThisYear;
        }
        public Tour GetMostVisitedTour(List<Tour> toursThisYear) 
        {
            if (toursThisYear.Count == 0) 
            {
                return null;
            }

            Tour mostVisitedTour = toursThisYear[0];

            foreach (Tour tour in toursThisYear) 
            {
                if(tour.CurrentGuestCount > mostVisitedTour.CurrentGuestCount) 
                {
                    mostVisitedTour = tour;
                }
            }
            return mostVisitedTour;
        }
        public Tour GetById(int id)
        {
            return _tourRepository.GetById(id);
        }

        public void Subscribe(IObserver observer)
        {
            _tourRepository.Subscribe(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            _tourRepository.Unsubscribe(observer);
        }

        public void NotifyObservers()
        {
            _tourRepository.NotifyObservers();
        }

        public void Update(Tour tour)
        {
            _tourRepository.Update(tour);
        }

        public Tour Save(Tour tour)
        {
            return _tourRepository.Save(tour);
        }

        internal List<Tour> GetAll()
        {
            return _tourRepository.GetAll();
        }
    }
}
