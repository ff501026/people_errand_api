using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeWorkType
    {
        public EmployeeWorkType()
        {
            EmployeeWorkRecords = new HashSet<EmployeeWorkRecord>();
        }

        public int WorkTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<EmployeeWorkRecord> EmployeeWorkRecords { get; set; }
    }
}
