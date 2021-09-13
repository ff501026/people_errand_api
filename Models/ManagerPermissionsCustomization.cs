using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class ManagerPermissionsCustomization
    {
        public int CustomizationId { get; set; }
        public int PermissionsId { get; set; }
        public int DepartmentId { get; set; }
        public int JobtitleId { get; set; }
    }
}
