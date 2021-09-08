using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeFlexibleWorktime
    {
        public EmployeeFlexibleWorktime()
        {
            Employees = new HashSet<Employee>();
        }

        public string FlexibleWorktimeId { get; set; }
        public string CompanyHash { get; set; }
        public string Name { get; set; }
        public TimeSpan WorkTimeStart { get; set; }
        public TimeSpan WorkTimeEnd { get; set; }
        public TimeSpan RestTimeStart { get; set; }
        public TimeSpan RestTimeEnd { get; set; }
        public int? BreakTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Company CompanyHashNavigation { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
