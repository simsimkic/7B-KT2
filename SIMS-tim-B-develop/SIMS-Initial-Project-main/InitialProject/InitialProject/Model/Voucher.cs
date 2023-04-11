using System;
using System.Collections.Generic;
using System.Linq;
using InitialProject.Serializer;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Model
{
    public class Voucher : ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }

        public Voucher()
        {
        }

        public Voucher(string name, DateTime startDateTime, DateTime expirationDateTime, int userId)
        {
            Name = name;
            StartDateTime = startDateTime;
            ExpirationDateTime = expirationDateTime;
            IsActive = true;
            UserId = userId;
        }

        public string[] ToCSV()
        {
            string[] csvValues = {
                Id.ToString(),
                Name,
                StartDateTime.ToString(),
                ExpirationDateTime.ToString(),
                IsActive.ToString(),
                UserId.ToString()
            };

            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            Name = values[1];
            StartDateTime = DateTime.Parse(values[2]);
            ExpirationDateTime = DateTime.Parse(values[3]);
            IsActive = bool.Parse(values[4]);
            UserId = int.Parse(values[5]);
        }
    }
}
