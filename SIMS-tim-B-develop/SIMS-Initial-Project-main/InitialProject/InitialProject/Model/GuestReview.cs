using InitialProject.Serializer;
using System;

namespace InitialProject.Model
{
    public class GuestReview : ISerializable
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int AccommodationId { get; set; }
        public int GuestId { get; set; }
        public string Comment { get; set; }
        public int Cleanness { get; set; }
        public int Behavior { get; set; }

        public GuestReview() { }

        public GuestReview(int reservationId, int accommodationId, int guestId, string comment, int cleanness, int behavior)
        {
            ReservationId = reservationId;
            AccommodationId = accommodationId;
            GuestId = guestId;
            Comment = comment;
            Cleanness = cleanness;
            Behavior = behavior;
        }

        public string[] ToCSV()
        {
            string[] csvValues = { Id.ToString(), ReservationId.ToString(), AccommodationId.ToString(), GuestId.ToString(), Comment, Cleanness.ToString(), Behavior.ToString() };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            ReservationId = Convert.ToInt32(values[1]);
            AccommodationId = Convert.ToInt32(values[2]);
            GuestId = Convert.ToInt32(values[3]);
            Comment = values[4];
            Cleanness = Convert.ToInt32(values[5]);
            Behavior = Convert.ToInt32(values[6]);
        }
    }
}
