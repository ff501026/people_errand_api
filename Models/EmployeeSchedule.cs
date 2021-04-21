using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeSchedule
    {
        public int ScheduleId { get; set; }
        public string ManagerHash { get; set; }
        public string EmployeeHash { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Employee EmployeeHashNavigation { get; set; }
        public virtual Employee ManagerHashNavigation { get; set; }
    }
}
