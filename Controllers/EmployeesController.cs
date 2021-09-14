using System;
using System.Collections;
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
    public class EmployeesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeesController(people_errandContext context)
        {
            _context = context;
        }

        //// GET: api/Employees
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        //{
        //    return await _context.Employees.ToListAsync();
        //}
        // Get: api/Employees/get_employee_manager_key
        [HttpGet("get_employee_manager_email")]
        public async Task<IEnumerable> employee_manager_key(string company_hash, string hash_account)
        {
            var employee = await (from t in _context.EmployeeInformations
                                  join b in _context.Employees on t.HashAccount equals b.HashAccount
                                  where t.HashAccount == hash_account
                                select new
                                {
                                    ManagerHash = b.ManagerHash,
                                    DepartmentId = t.DepartmentId,
                                    JobtitleId = t.JobtitleId
                                }).ToListAsync();
            var result = await (from t in _context.ManagerAccounts
                                join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                join b in _context.Employees on t.HashAccount equals b.HashAccount
                                where t.PermissionsId == null && b.CompanyHash==company_hash
                                select new
                                {
                                    Email = a.Email
                                }).ToListAsync();

            var permissions = await (from t in _context.ManagerPermissions
                                where t.CompanyHash.Equals(company_hash)
                                select new
                                {
                                    PermissionsId = t.PermissionsId,
                                    Name = t.Name,
                                    EmployeeDisplay = t.EmployeeDisplay,
                                    CustomizationDisplay = t.CustomizationDisplay,
                                    EmployeeReview = t.EmployeeReview,
                                    CustomizationReview = t.CustomizationReview,
                                    SettingWorktime = t.SettingWorktime,
                                    SettingDepartmentJobtitle = t.SettingDepartmentJobtitle,
                                    SettingLocation = t.SettingLocation
                                }).ToListAsync();

            List<string> manager = new List<string>();
            foreach (var i in result)
            {
                string email = i.Email;
                manager.Add(email);
            }
            foreach (var p in permissions) 
            {
                if (p.EmployeeReview == 1)
                {
                    result = await (from t in _context.ManagerAccounts
                                    join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                    where t.PermissionsId == p.PermissionsId
                                    select new
                                    {
                                        Email = a.Email
                                    }).ToListAsync();
                    foreach (var i in result)
                    {
                        string email = i.Email;
                        manager.Add(email);
                    }
                }
                else if (p.EmployeeReview == 2)
                {
                    result = await (from t in _context.ManagerAccounts
                                    join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                    where t.PermissionsId == p.PermissionsId && t.HashAccount.Equals(employee[0].ManagerHash) 
                                    select new
                                    {
                                        Email = a.Email
                                    }).ToListAsync();
                    foreach (var i in result)
                    {
                        string email = i.Email;
                        manager.Add(email);
                    }
                }
                else 
                {
                    var customizationsReview = await (from t in _context.ManagerPermissionsCustomizations
                                                      where t.PermissionsId == p.CustomizationReview
                                                      select new
                                                      {
                                                          DepartmentId = t.DepartmentId,
                                                          JobtitleId = t.JobtitleId
                                                      }).ToListAsync();
                    foreach (var c in customizationsReview) 
                    {
                        if (c.DepartmentId==employee[0].DepartmentId && c.JobtitleId==employee[0].JobtitleId) 
                        {
                            result = await (from t in _context.ManagerAccounts
                                            join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                            where t.PermissionsId == p.PermissionsId 
                                            select new
                                            {
                                                Email = a.Email
                                            }).ToListAsync();

                            foreach (var i in result)
                            {
                                string email = i.Email;
                                manager.Add(email);
                            }
                        }
                    }
                }
                
            }
            return manager;
        }


        // GET: api/Employees/phone_code
        [HttpGet("{phone_code}")]
        public async Task<ActionResult<string>> GetEmployee(string phone_code)
        {
            //去employee資料表比對phone_code，並回傳資料行
            var employee = await _context.Employees
                .Where(db_employee => db_employee.PhoneCode == phone_code)
                .Select(db_employee => db_employee.HashAccount).FirstOrDefaultAsync();

            if (phone_code == null)
            {
                return NotFound();
            }

            return employee;
        }


        // Get: api/Employees/get_employee_manager_key
        [HttpGet("get_employee_manager_key/{hash_account}")]
        public async Task<string> employee_manager_key(string hash_account)
        {
            var get_employee = await(_context.Employees
                       .FromSqlInterpolated($"EXECUTE dbo.update_employee_manager_key {hash_account}")
                       ).ToListAsync();

            var get_manager_key = get_employee.Count != 0 ? get_employee[0].ManagerKey.ToString() : "";
            return get_manager_key;
        }

        // Get: api/Employees/update_employee_manager_key
        [HttpGet("get_employee_data/{manager_key}")]
        public async Task<IEnumerable> get_manager_data(string manager_key)
        {
            var get_employee = await (from t in _context.Employees
                                      join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                      join b in _context.Companies on t.CompanyHash equals b.CompanyHash
                                      where t.ManagerKey.Equals(manager_key)
                                      select new
                                      {
                                          HashAccount = t.HashAccount,
                                          Code = b.Code,
                                          Name = a.Name,
                                          ManagerKeyOverDate = t.ManagerKeyOverDate
                                      }).ToListAsync();

            return get_employee;
        }

        // PUT: api/Employees/enabled_employee
        [HttpPut("enabled_employee")]
        public ActionResult<bool> enabled_employee([FromBody] List<Employee> employees)
        {
            bool result = true;
            try
            {
                foreach (Employee employee in employees)
                //foreach用來讀取多筆資料，假設一個JSON有很多{}
                {
                    var parameters = new[]
                    {

                        new SqlParameter("@hashaccount", System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employee.HashAccount
                        },
                        new SqlParameter("@enabled", System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employee.Enabled
                        }
                    };

                    result = _context.Database.ExecuteSqlRaw("exec enabled_employee @hashaccount,@enabled", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }


        // POST: api/Employees/regist_employees
        [HttpPost("regist_employee")]
        public ActionResult<bool> regist_employee([FromBody] List<Employee> employees)
        {
            bool result = true;
            try
            {
                foreach (Employee employee in employees)
                {
                    //設定放入查詢的值
                    var parameters = new[]
                    {
                        new SqlParameter("@phone_code",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employee.PhoneCode
                        },
                        new SqlParameter("@company_hash",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employee.CompanyHash
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec regist_employee @phone_code,@company_hash", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }

            //輸出成功與否
            return result;
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(string id)
        {
            return _context.Employees.Any(e => e.HashAccount == id);
        }
    }
}
