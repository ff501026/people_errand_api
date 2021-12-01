using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeWorkRecord
    {
        public int WorkRecordsId { get; set; }
        public string HashAccount { get; set; }
        public int WorkTypeId { get; set; }
        public decimal CoordinateX { get; set; }
        public decimal CoordinateY { get; set; }
        public string Address { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Employee HashAccountNavigation { get; set; }
        public virtual EmployeeWorkType WorkType { get; set; }
    }
}
