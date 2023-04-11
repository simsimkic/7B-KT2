using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InitialProject.Resources.Enums;
using InitialProject.Serializer;

namespace InitialProject.Model
{
    public class TourReservation : ISerializable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TourId { get; set; }
        public int PersonCount { get; set; }
        public int CheckpointArrivalId { get; set; }
        public bool GuestChecked { get; set; }

        public bool MessageBoxShown { get; set; }
        public bool GuestArrived { get; set; }
        public bool IsRated { get; set; }
        public double AverageAge { get; set; }
        public int UsedVoucherId { get; set; }
        public TourReservation() { }

        public TourReservation(int userId, int tourId, int personCount, double averageAge, int usedVoucher)
        {
            UserId = userId;
            TourId = tourId;
            PersonCount = personCount;
            CheckpointArrivalId = -1;
            GuestChecked = false;
            MessageBoxShown = false;
            GuestArrived = false;
            IsRated = false;
            AverageAge = averageAge;
            UsedVoucherId = usedVoucher;
        }

        public string[] ToCSV()
        {
            string[] csvValues = { Id.ToString(), UserId.ToString(), TourId.ToString(), PersonCount.ToString(), CheckpointArrivalId.ToString(), GuestChecked.ToString(), MessageBoxShown.ToString(), GuestArrived.ToString(), IsRated.ToString(), AverageAge.ToString(), UsedVoucherId.ToString() };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            UserId = Convert.ToInt32(values[1]);
            TourId = Convert.ToInt32(values[2]);
            PersonCount = Convert.ToInt32(values[3]);
            CheckpointArrivalId = Convert.ToInt32(values[4]);
            GuestChecked = Convert.ToBoolean(values[5]);
            MessageBoxShown = Convert.ToBoolean(values[6]);
            GuestArrived = Convert.ToBoolean(values[7]);
            IsRated = Convert.ToBoolean(values[8]);
            AverageAge = Convert.ToDouble (values[9]);
            UsedVoucherId = Convert.ToInt32(values[10]);
        }
    }
}
