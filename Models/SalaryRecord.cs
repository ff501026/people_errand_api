using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class SalaryRecord
    {
        public int SalaryRecordsId { get; set; }
        public string ManagerHash { get; set; }
        public string EmployeeHash { get; set; }
        public double Money { get; set; }
        public int PayMonth { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Employee EmployeeHashNavigation { get; set; }
        public virtual Employee ManagerHashNavigation { get; set; }
    }
}
