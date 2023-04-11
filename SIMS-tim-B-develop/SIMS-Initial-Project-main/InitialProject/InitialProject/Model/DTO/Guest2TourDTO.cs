using InitialProject.Serializer;
using System;
using System.Collections.Generic;

namespace InitialProject.Model.DTO
{
    public class Guest2TourDTO 
    {
        public int TourId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int MaxGuests { get; set; }
        public int CurrentGuestCount { get; set; }
        public DateTime StartTime { get; set; }
        public double Duration { get; set; }
        public string Guide { get; set; }

        public Guest2TourDTO()
        {
        }

        public Guest2TourDTO(int tourId, string name, string country, string city, string description, string language, int maxGuests, int currentGuestCount, DateTime startTime, double duration, string guide)
        {
            TourId = tourId;
            Name = name;
            Country = country;
            City = city;
            Description = description;
            Language = language;
            MaxGuests = maxGuests;
            CurrentGuestCount = currentGuestCount;
            StartTime = startTime;
            Duration = duration;
            Guide = guide;
        }
    }
}
