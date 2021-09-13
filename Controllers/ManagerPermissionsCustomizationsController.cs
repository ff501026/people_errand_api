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
    public class ManagerPermissionsCustomizationsController : ControllerBase
    {
        private readonly people_errandContext _context;

        public ManagerPermissionsCustomizationsController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/ManagerPermissionsCustomizations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagerPermissionsCustomization>>> GetManagerPermissionsCustomizations()
        {
            return await _context.ManagerPermissionsCustomizations.ToListAsync();
        }

        // GET: api/ManagerPermissionsCustomizations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ManagerPermissionsCustomization>> GetManagerPermissionsCustomization(int id)
        {
            var managerPermissionsCustomization = await _context.ManagerPermissionsCustomizations.FindAsync(id);

            if (managerPermissionsCustomization == null)
            {
                return NotFound();
            }

            return managerPermissionsCustomization;
        }

        //// PUT: api/ManagerPermissions/update_manager_permissions_customization
        //[HttpPut("update_manager_permissions_customization")]//編輯
        //public ActionResult<bool> update_manager_permissions_customization([FromBody] List<ManagerPermissionsCustomization> managerPermissionsCustomizations)
        //{
        //    bool result = true;
        //    try
        //    {
        //        foreach (ManagerPermissionsCustomization managerPermissionsCustomization in managerPermissionsCustomizations)
        //        {
        //            var parameters = new[]
        //            {
        //                new SqlParameter("@permissions_id",System.Data.SqlDbType.Int)
        //                {
        //                    Direction = System.Data.ParameterDirection.Input,
        //                    Value = managerPermissionsCustomization.PermissionsId
        //                },
        //                new SqlParameter("@departmnet_id",System.Data.SqlDbType.Int)
        //                {
        //                    Direction = System.Data.ParameterDirection.Input,
        //                    Value = managerPermissionsCustomization.DepartmentId
        //                },
        //                new SqlParameter("@jobtitle_id",System.Data.SqlDbType.Int)
        //                {
        //                    Direction = System.Data.ParameterDirection.Input,
        //                    Value = managerPermissionsCustomization.JobtitleId
        //                }
        //            };
        //            result = _context.Database.ExecuteSqlRaw("EXECUTE dbo.update_manager_permissions @permissions_id,@name,@employee_display,@employee_review,@setting_worktime,@setting_department_jobtitle,@setting_location", parameters: parameters) != 0 ? true : false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = false;
        //        throw;
        //    }
        //    return result;
        //}

        // POST: api/ManagerPermissionsCustomization/add_manager_permissions_customization
        [HttpPost("add_manager_permissions_customization")]//新增
        public ActionResult<bool> add_manager_permissions_customization([FromBody] List<ManagerPermissionsCustomization> managerPermissionsCustomizations)
        {
            bool result = true;
            try
            {
                foreach (ManagerPermissionsCustomization managerPermissionsCustomization in managerPermissionsCustomizations)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@permissions_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermissionsCustomization.PermissionsId
                        },
                        new SqlParameter("@department_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermissionsCustomization.DepartmentId
                        },
                        new SqlParameter("@jobtitle_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerPermissionsCustomization.JobtitleId
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("EXECUTE dbo.add_manager_permissions_customization @permissions_id,@department_id,@jobtitle_id", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // DELETE: api/ManagerPermissionsCustomization/DeleteManagerPermissionsCustomization/5
        [HttpDelete("DeleteManagerPermissionsCustomization/{permissions_id}")]
        public async Task<bool> DeleteManagerPermissionsCustomization(int permissions_id)
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
                result = _context.Database.ExecuteSqlRaw("exec delete_manager_permissions_customization @permissions_id", parameters: parameters) != 0 ? true : false;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        private bool ManagerPermissionsCustomizationExists(int id)
        {
            return _context.ManagerPermissionsCustomizations.Any(e => e.CustomizationId == id);
        }
    }
}
