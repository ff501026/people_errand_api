using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeGeneralWorktime
    {
        public EmployeeGeneralWorktime()
        {
            Employees = new HashSet<Employee>();
        }

        public string GeneralWorktimeId { get; set; }
        public string CompanyHash { get; set; }
        public string Name { get; set; }
        public TimeSpan WorkTime { get; set; }
        public TimeSpan RestTime { get; set; }
        public int BreakTime { get; set; }

        public virtual Company CompanyHashNavigation { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
