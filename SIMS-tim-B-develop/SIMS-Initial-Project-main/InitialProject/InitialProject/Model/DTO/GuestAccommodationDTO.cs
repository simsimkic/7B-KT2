using InitialProject.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Model.DTO
{
    public  class GuestAccommodationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public AccommodationType Type { get; set; }
        public int MaxGuests { get; set; }
        public int MinReservationDays { get; set; }
        public int CancellationPeriod { get; set; }

        public GuestAccommodationDTO()
        {

        }
        public GuestAccommodationDTO(int id, string name, string country, string city, AccommodationType type, int maxGuests, int minReservationDays, int cancellationPeriod)
        {
            Id = id;
            Name = name;
            Country = country;
            City = city;
            Type = type;
            MaxGuests = maxGuests;
            MinReservationDays = minReservationDays;
            CancellationPeriod = cancellationPeriod;
        }
    }
}
