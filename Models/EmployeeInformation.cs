using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeInformation
    {
        public int InformationId { get; set; }
        public string HashAccount { get; set; }
        public string Name { get; set; }
        public int? DepartmentId { get; set; }
        public int? JobtitleId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Img { get; set; }

        public virtual EmployeeDepartmentType Department { get; set; }
        public virtual Employee HashAccountNavigation { get; set; }
        public virtual EmployeeJobtitleType Jobtitle { get; set; }
    }
}
