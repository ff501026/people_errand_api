using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class ManagerAccount
    {
        public int ManagerId { get; set; }
        public string HashAccount { get; set; }
        public string Password { get; set; }
        public int? PermissionsId { get; set; }
        public bool Enabled { get; set; }

        public virtual Employee HashAccountNavigation { get; set; }
        public virtual ManagerPermission Permissions { get; set; }
    }
}
