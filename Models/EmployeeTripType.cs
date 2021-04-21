using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeTripType
    {
        public EmployeeTripType()
        {
            EmployeeTripRecords = new HashSet<EmployeeTripRecord>();
        }

        public int TripTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<EmployeeTripRecord> EmployeeTripRecords { get; set; }
    }
}
