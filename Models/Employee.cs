﻿using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Companies = new HashSet<Company>();
            EmployeeInformations = new HashSet<EmployeeInformation>();
            EmployeeLeaveRecords = new HashSet<EmployeeLeaveRecord>();
            EmployeeScheduleEmployeeHashNavigations = new HashSet<EmployeeSchedule>();
            EmployeeScheduleManagerHashNavigations = new HashSet<EmployeeSchedule>();
            EmployeeTripRecords = new HashSet<EmployeeTripRecord>();
            EmployeeWorkRecords = new HashSet<EmployeeWorkRecord>();
            SalaryRecordEmployeeHashNavigations = new HashSet<SalaryRecord>();
            SalaryRecordManagerHashNavigations = new HashSet<SalaryRecord>();
        }

        public string EmployeeId { get; set; }
        public string HashAccount { get; set; }
        public string PhoneCode { get; set; }
        public int RoleId { get; set; }
        public string CompanyHash { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Company CompanyHashNavigation { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<EmployeeInformation> EmployeeInformations { get; set; }
        public virtual ICollection<EmployeeLeaveRecord> EmployeeLeaveRecords { get; set; }
        public virtual ICollection<EmployeeSchedule> EmployeeScheduleEmployeeHashNavigations { get; set; }
        public virtual ICollection<EmployeeSchedule> EmployeeScheduleManagerHashNavigations { get; set; }
        public virtual ICollection<EmployeeTripRecord> EmployeeTripRecords { get; set; }
        public virtual ICollection<EmployeeWorkRecord> EmployeeWorkRecords { get; set; }
        public virtual ICollection<SalaryRecord> SalaryRecordEmployeeHashNavigations { get; set; }
        public virtual ICollection<SalaryRecord> SalaryRecordManagerHashNavigations { get; set; }


    }
}
