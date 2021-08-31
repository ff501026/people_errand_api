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
