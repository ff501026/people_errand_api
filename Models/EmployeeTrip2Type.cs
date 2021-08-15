using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeTrip2Type
    {
        public EmployeeTrip2Type()
        {
            EmployeeTrip2Records = new HashSet<EmployeeTrip2Record>();
        }

        public int Trip2TypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<EmployeeTrip2Record> EmployeeTrip2Records { get; set; }
    }
}
