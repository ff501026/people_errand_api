using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class Company
    {
        public Company()
        {
            EmployeeDepartmentTypes = new HashSet<EmployeeDepartmentType>();
            EmployeeJobtitleTypes = new HashSet<EmployeeJobtitleType>();
            Employees = new HashSet<Employee>();
        }

        public string CompanyId { get; set; }
        public string CompanyHash { get; set; }
        public string ManagerHash { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal? CoordinateX { get; set; }
        public decimal? CoordinateY { get; set; }
        public TimeSpan? WorkTime { get; set; }
        public TimeSpan? RestTime { get; set; }
        public string ManagerPassword { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual Employee ManagerHashNavigation { get; set; }
        public virtual ICollection<EmployeeDepartmentType> EmployeeDepartmentTypes { get; set; }
        public virtual ICollection<EmployeeJobtitleType> EmployeeJobtitleTypes { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
