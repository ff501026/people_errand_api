using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeTrip2Record
    {
        public int Trip2RecordsId { get; set; }
        public int GroupId { get; set; }
        public string HashAccount { get; set; }
        public int Trip2TypeId { get; set; }
        public decimal CoordinateX { get; set; }
        public decimal CoordinateY { get; set; }
        public string Address { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Employee HashAccountNavigation { get; set; }
        public virtual EmployeeTrip2Type Trip2Type { get; set; }
    }
}
