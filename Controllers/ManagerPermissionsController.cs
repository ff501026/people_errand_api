using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using People_errand_api.Models;

namespace People_errand_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerPermissionsController : ControllerBase
    {
        private readonly people_errandContext _context;

        public ManagerPermissionsController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/ManagerPermissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagerPermission>>> GetManagerPermissions()
        {
            return await _context.ManagerPermissions.ToListAsync();
        }

        // GET: api/ManagerPermissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ManagerPermission>> GetManagerPermission(int id)
        {
            var managerPermission = await _context.ManagerPermissions.FindAsync(id);

            if (managerPermission == null)
            {
                return NotFound();
            }

            return managerPermission;
        }
        // PUT: api/ManagerPermissions/update_manager_permissions
        [HttpPut("update_manager_permissions")]//編輯
        public ActionResult<bool> update_manager_permissions([FromBody] List<ManagerPermission> managerPermissions)
        {
            bool result = true;
            try
            {
                foreach (ManagerPermission managerPermission in managerPermissions)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@permissions_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.PermissionsId
                        },
                        new SqlParameter("@name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.Name
                        },
                        new SqlParameter("@employee_display",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.EmployeeDisplay
                        },
                        new SqlParameter("@employee_review",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.EmployeeReview
                        },
                        new SqlParameter("@setting_worktime",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.SettingWorktime
                        },
                        new SqlParameter("@setting_department_jobtitle",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.SettingDepartmentJobtitle
                        },
                        new SqlParameter("@setting_location",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.SettingLocation
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("EXECUTE dbo.update_manager_permissions @permissions_id,@name,@employee_display,@employee_review,@setting_worktime,@setting_department_jobtitle,@setting_location", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }
        

        // POST: api/ManagerPermissions/add_manager_permissions
        [HttpPost("add_manager_permissions")]//新增
        public async Task<int> add_manager_permissions([FromBody] List<ManagerPermission> managerPermissions)
        {
            int id = -1;
            try
            {
                foreach (ManagerPermission managerPermission in managerPermissions)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@company_hash",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.CompanyHash
                        },
                        new SqlParameter("@name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.Name
                        },
                        new SqlParameter("@employee_display",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.EmployeeDisplay
                        },
                        new SqlParameter("@employee_review",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.EmployeeReview
                        },
                        new SqlParameter("@setting_worktime",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.SettingWorktime
                        },
                        new SqlParameter("@setting_department_jobtitle",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.SettingDepartmentJobtitle
                        },
                        new SqlParameter("@setting_location",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermission.SettingLocation
                        }
                    };
                    var get_manager_permissions = await(_context.ManagerPermissions
                        .FromSqlRaw("EXECUTE dbo.add_manager_permissions @company_hash,@name,@employee_display,@employee_review,@setting_worktime,@setting_department_jobtitle,@setting_location", parameters: parameters)
                        ).ToListAsync();
                    id = get_manager_permissions[0].PermissionsId;
                }
            }
            catch (Exception)
            {
                id=-1;
                throw;
            }
            return id;
        }
        // DELETE: api/ManagerPermissions/DeleteManagerPermissions/5
        [HttpDelete("DeleteManagerPermissions/{permissions_id}")]
        public async Task<bool> DeleteManagerPermissions(int permissions_id)
        {
            bool result = true;
            try
            {
                var parameters = new[]
                {
                            new SqlParameter("@permissions_id",System.Data.SqlDbType.Int)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = permissions_id
                            }
                        };
                result = _context.Database.ExecuteSqlRaw("exec delete_manager_permissions @permissions_id", parameters: parameters) != 0 ? true : false;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

       
        private bool ManagerPermissionExists(int id)
        {
            return _context.ManagerPermissions.Any(e => e.PermissionsId == id);
        }
    }
}
