using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeLeaveType
    {
        public EmployeeLeaveType()
        {
            EmployeeLeaveRecords = new HashSet<EmployeeLeaveRecord>();
        }

        public int LeaveTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<EmployeeLeaveRecord> EmployeeLeaveRecords { get; set; }
    }
}
