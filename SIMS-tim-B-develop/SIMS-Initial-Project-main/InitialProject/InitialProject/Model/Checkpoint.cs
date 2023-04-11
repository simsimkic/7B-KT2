using InitialProject.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Model
{
    public class Checkpoint: ISerializable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TourId { get; set; }

        public int Order { get; set; }

        public bool IsActive { get; set; }

        public Checkpoint() { }
        public Checkpoint(string name, int order, bool isActive, int tourId)
        {
            Name = name;
            Order = order;
            IsActive = isActive;
            TourId = tourId;
        }

        public string[] ToCSV()
        {

            string[] csvValues = { Id.ToString(), Name, Order.ToString(), IsActive.ToString(), TourId.ToString() };
            return csvValues;
        }
        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Name = values[1];
            Order = int.Parse(values[2]);
            IsActive = bool.Parse(values[3]);
            TourId = int.Parse(values[4]);
        }

    }
}
