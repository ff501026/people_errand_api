using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class EmployeeJobtitleType
    {
        public EmployeeJobtitleType()
        {
            EmployeeInformations = new HashSet<EmployeeInformation>();
        }

        public int JobtitleId { get; set; }
        public string Name { get; set; }
        public string CompanyHash { get; set; }

        public virtual Company CompanyHashNavigation { get; set; }
        public virtual ICollection<EmployeeInformation> EmployeeInformations { get; set; }
    }
}
