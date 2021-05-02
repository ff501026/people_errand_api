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
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Employee HashAccountNavigation { get; set; }
        public virtual EmployeeWorkType WorkType { get; set; }
    }
}
