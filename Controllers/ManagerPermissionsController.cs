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

        // PUT: api/ManagerPermissions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManagerPermission(int id, ManagerPermission managerPermission)
        {
            if (id != managerPermission.PermissionsId)
            {
                return BadRequest();
            }

            _context.Entry(managerPermission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManagerPermissionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/add_information
        [HttpPost("add_manager_permissions")]//新增員工資料
        public ActionResult<bool> add_manager_permissions([FromBody] List<ManagerPermission> managerPermissions)
        {
            bool result = true;
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
                    result = _context.Database.ExecuteSqlRaw("exec add_manager_permissions @company_hash,@name,@employee_display,@employee_review,@setting_worktime,@setting_department_jobtitle,@setting_location", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }
        // DELETE: api/EmployeeInformations/DeleteInformation/5
        [HttpDelete("DeleteInformation/{hash_account}")]
        public async Task<bool> DeleteInformation(string hash_account)
        {
            bool result = true;
            try
            {
                var parameters = new[]
                {
                            new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = hash_account
                            }
                        };
                result = _context.Database.ExecuteSqlRaw("exec delete_employee @hash_account", parameters: parameters) != 0 ? true : false;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // DELETE: api/ManagerPermissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManagerPermission(int id)
        {
            var managerPermission = await _context.ManagerPermissions.FindAsync(id);
            if (managerPermission == null)
            {
                return NotFound();
            }

            _context.ManagerPermissions.Remove(managerPermission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ManagerPermissionExists(int id)
        {
            return _context.ManagerPermissions.Any(e => e.PermissionsId == id);
        }
    }
}
