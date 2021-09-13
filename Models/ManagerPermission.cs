using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class ManagerPermission
    {
        public ManagerPermission()
        {
            ManagerAccounts = new HashSet<ManagerAccount>();
        }

        public int PermissionsId { get; set; }
        public string CompanyHash { get; set; }
        public string Name { get; set; }
        public int EmployeeDisplay { get; set; }
        public int? CustomizationDisplay { get; set; }
        public int EmployeeReview { get; set; }
        public int? CustomizationReview { get; set; }
        public bool SettingWorktime { get; set; }
        public bool SettingDepartmentJobtitle { get; set; }
        public bool SettingLocation { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Company CompanyHashNavigation { get; set; }
        public virtual ManagerPermissionsType EmployeeDisplayNavigation { get; set; }
        public virtual ManagerPermissionsType EmployeeReviewNavigation { get; set; }
        public virtual ICollection<ManagerAccount> ManagerAccounts { get; set; }
    }
}
