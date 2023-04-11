using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Model.DTO
{
    public class GuideTourDTO
    {
        public int Id { get; set; }  
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string NumberOfGuestsMessage { get; set; }

        public GuideTourDTO()
        {

        }
        public GuideTourDTO(int id, string name, string country, string city, DateTime startTime, int numberOfGuests)
        {
            Id = id;
            Name = name;
            Country = country;
            City = city;
            StartTime = startTime;
            Location = city + ", " + country;
            NumberOfGuests = numberOfGuests;
            NumberOfGuestsMessage = numberOfGuests.ToString() + " people visited";
        }
    }
}
