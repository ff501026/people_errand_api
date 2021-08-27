using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class ManagerPermissionsType
    {
        public ManagerPermissionsType()
        {
            ManagerPermissionEmployeeDisplayNavigations = new HashSet<ManagerPermission>();
            ManagerPermissionEmployeeReviewNavigations = new HashSet<ManagerPermission>();
        }

        public int PermissionsTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ManagerPermission> ManagerPermissionEmployeeDisplayNavigations { get; set; }
        public virtual ICollection<ManagerPermission> ManagerPermissionEmployeeReviewNavigations { get; set; }
    }
}
