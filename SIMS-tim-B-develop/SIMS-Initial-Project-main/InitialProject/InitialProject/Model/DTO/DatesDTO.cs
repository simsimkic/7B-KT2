using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Model.DTO
{
    public class DatesDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DatesDTO()  {}
        public DatesDTO(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
