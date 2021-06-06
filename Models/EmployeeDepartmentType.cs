using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeDepartmentType
    {
        public EmployeeDepartmentType()
        {
            EmployeeInformations = new HashSet<EmployeeInformation>();
        }

        public int DepartmentId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<EmployeeInformation> EmployeeInformations { get; set; }
    }
}
