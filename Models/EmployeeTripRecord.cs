using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeTripRecord
    {
        public int TripRecordsId { get; set; }
        public string HashAccount { get; set; }
        public int TripTypeId { get; set; }
        public string Location { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Employee HashAccountNavigation { get; set; }
        public virtual EmployeeTripType TripType { get; set; }
    }
}
