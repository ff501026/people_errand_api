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
            EmployeeFlexibleWorktimes = new HashSet<EmployeeFlexibleWorktime>();
            EmployeeGeneralWorktimes = new HashSet<EmployeeGeneralWorktime>();
            EmployeeJobtitleTypes = new HashSet<EmployeeJobtitleType>();
            Employees = new HashSet<Employee>();
            ManagerPermissions = new HashSet<ManagerPermission>();
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
        public int PositionDifference { get; set; }
        public bool SettingTrip2Enabled { get; set; }
        public bool SettingWorkrecordEnabled { get; set; }
        public TimeSpan? WorkTime { get; set; }
        public TimeSpan? RestTime { get; set; }
        public string ManagerPassword { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual ICollection<EmployeeDepartmentType> EmployeeDepartmentTypes { get; set; }
        public virtual ICollection<EmployeeFlexibleWorktime> EmployeeFlexibleWorktimes { get; set; }
        public virtual ICollection<EmployeeGeneralWorktime> EmployeeGeneralWorktimes { get; set; }
        public virtual ICollection<EmployeeJobtitleType> EmployeeJobtitleTypes { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<ManagerPermission> ManagerPermissions { get; set; }
    }
}
