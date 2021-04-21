using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeInformation
    {
        public int InformationId { get; set; }
        public string HashAccount { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public virtual Employee HashAccountNavigation { get; set; }
    }
}
