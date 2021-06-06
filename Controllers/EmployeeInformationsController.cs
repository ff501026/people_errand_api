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
    public class EmployeeInformationsController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeInformationsController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeInformations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeInformation>>> GetEmployeeInformations()
        {
            return await _context.EmployeeInformations.ToListAsync();
        }


        //// GET: api/EmployeeInformations/hash_account
        //[HttpGet("{hash_account}")]
        //public IEnumerable<Employee> GetEmployeeInformation(string hash_account)
        //{

        //    List<Employee> employee;

        //    var parameters = new[]
        //    {
        //        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
        //        {
        //            Direction = System.Data.ParameterDirection.Input,
        //            Value = hash_account,
        //        }
        //    };

        //    employee = _context.Employees.FromSqlRaw("exec get_employeeInformaion @hash_account", parameters).ToList();

        //    return employee;
        //}


        [HttpGet("{hash_account}")]
        public async Task<IEnumerable> GetEmployeeInformation(string hash_account)
        {

            var Employee_information = await (from t in _context.Employees
                                              join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                              join b in _context.EmployeeDepartmentTypes on a.DepartmentId equals b.DepartmentId
                                              join c in _context.EmployeeJobtitleTypes on a.JobtitleId equals c.JobtitleId
                                              where t.HashAccount == hash_account
                                              select new
                                              {
                                                  name = a.Name,
                                                  department = b.Name,
                                                  jobtitle = c.Name,
                                                  phone = a.Phone,
                                                  email = a.Email,
                                              }).ToListAsync();
            return Employee_information;
        }



        // PUT: api/update_information
        [HttpPut("update_information")]
        public ActionResult<bool> update_information([FromBody] List<EmployeeInformation> employeeInformations)
        {
            bool result = true;
            try
            {
                foreach (EmployeeInformation employeeInformation in employeeInformations)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.HashAccount
                        },
                        new SqlParameter("@phone",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Phone
                        },
                        new SqlParameter("@email",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Email
                        }
                    };

                    result = _context.Database.ExecuteSqlRaw("exec update_information @hash_account,@phone,@email", parameters: parameters) != 0 ? true : false;
                }

            }
            catch (Exception)
            {
                result = false;
                throw;
            }

            return result;
        }

      

        // POST: api/add_information
        [HttpPost("add_information")]
        public ActionResult<bool> add_information([FromBody]List<EmployeeInformation> employeeInformations)
        {
            bool result = true;
            try
            {
                foreach (EmployeeInformation employeeInformation in employeeInformations)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.HashAccount
                        },
                        new SqlParameter("@department_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.DepartmentId
                        },
                        new SqlParameter("@jobtitle_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.JobtitleId
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec add_information @hash_account,@department_id,@jobtitle_id",parameters:parameters) != 0 ? true : false;
                }
            }
            catch (Exception) {
                result = false;
                throw;
            }
            return result;
        }

        // DELETE: api/EmployeeInformations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeInformation(int id)
        {
            var employeeInformation = await _context.EmployeeInformations.FindAsync(id);
            if (employeeInformation == null)
            {
                return NotFound();
            }

            _context.EmployeeInformations.Remove(employeeInformation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeInformationExists(int id)
        {
            return _context.EmployeeInformations.Any(e => e.InformationId == id);
        }
    }
}
