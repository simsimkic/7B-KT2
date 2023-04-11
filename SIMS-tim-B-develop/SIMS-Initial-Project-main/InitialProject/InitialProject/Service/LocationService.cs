using InitialProject.Model;
using InitialProject.Repository;
using InitialProject.Resources.Observer;
using InitialProject.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Service
{
    public class LocationService: ISubject
    {
        private readonly LocationRepository _locationRepository;

        public LocationService()
        {
           _locationRepository = new LocationRepository();
        }
        public List<string> GetCountries()
        {
            List<string> countries = new List<string>();
            foreach (Location location in _locationRepository.GetAll())
            {
                if (!countries.Contains(location.Country))
                    countries.Add(location.Country);
            }
            return countries;
        }
        public List<string> GetCities(String country)
        {
            List<string> cities = new List<string>();
            foreach (Location location in _locationRepository.GetAll())
            {
                if (location.Country == country)
                {
                    cities.Add(location.City);
                }
            }
            return cities;
        }
        public Location GetLocation(String country, string city)
        {
            foreach (Location location in _locationRepository.GetAll())
            {
                if (location.Country == country && location.City == city)
                {
                    return location;
                }
            }
            return null;
        }
        public Location GetById(int id)
        {
            return _locationRepository.GetById(id);
        }

        public void NotifyObservers()
        {
            _locationRepository.NotifyObservers();
        }

        public void Subscribe(IObserver observer)
        {
            _locationRepository.Subscribe(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            _locationRepository.Unsubscribe(observer);
        }
    }
}
