using InitialProject.Resources.Enums;
using InitialProject.Serializer;
using System;

namespace InitialProject.Model
{
    public class RescheduleRequest : ISerializable
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int GuestId { get; set; }
        public int AccommodationId { get; set; }
        public int OwnerId { get; set; }
        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public RescheduleRequestStatus Status { get; set; }
        public string Comment { get; set; }
        public bool IsNotified { get; set; }

        public RescheduleRequest() { }

        public RescheduleRequest(AccommodationReservation reservation, DateTime newStartDate, DateTime newEndDate)
        {
            ReservationId = reservation.Id;
            GuestId = reservation.GuestId;
            AccommodationId = reservation.AccommodationId;
            OwnerId = reservation.OwnerId;
            NewStartDate = newStartDate;
            NewEndDate = newEndDate;
            Status = RescheduleRequestStatus.onhold;
            Comment = "";
            IsNotified = true;
        }

        public string[] ToCSV()
        {
            string[] csvValues = { Id.ToString(), ReservationId.ToString(), GuestId.ToString(), AccommodationId.ToString(), OwnerId.ToString(), NewStartDate.ToString(), NewEndDate.ToString(), Status.ToString(), Comment, IsNotified.ToString() };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            ReservationId = Convert.ToInt32(values[1]);
            GuestId = Convert.ToInt32(values[2]);
            AccommodationId = Convert.ToInt32(values[3]);
            OwnerId = Convert.ToInt32(values[4]);
            NewStartDate = DateTime.Parse(values[5]);
            NewEndDate = DateTime.Parse(values[6]);
            Status = Enum.Parse<RescheduleRequestStatus>(values[7]);
            Comment = values[8];
            IsNotified = Convert.ToBoolean(values[9]);
        }
    }
}
