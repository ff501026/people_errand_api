﻿using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeLeaveRecord
    {
        public int LeaveRecordsId { get; set; }
        public string HashAccount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveTypeId { get; set; }
        public string Reason { get; set; }
        public bool? Review { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Employee HashAccountNavigation { get; set; }
        public virtual EmployeeLeaveType LeaveType { get; set; }
    }
}
