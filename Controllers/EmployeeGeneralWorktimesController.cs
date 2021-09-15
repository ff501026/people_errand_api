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
    public class EmployeeGeneralWorktimesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeGeneralWorktimesController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeGeneralWorktimes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeGeneralWorktime>>> GetEmployeeGeneralWorktimes()
        {
            return await _context.EmployeeGeneralWorktimes.ToListAsync();
        }

        // GET: api/EmployeeGeneralWorktimes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeGeneralWorktime>> GetEmployeeGeneralWorktime(string id)
        {
            var employeeGeneralWorktime = await _context.EmployeeGeneralWorktimes.FindAsync(id);

            if (employeeGeneralWorktime == null)
            {
                return NotFound();
            }

            return employeeGeneralWorktime;
        }

        // PUT: api/update_general_worktime
        [HttpPut("update_general_worktime")]//新增一般上下班
        public ActionResult<bool> update_general_worktime([FromBody] List<GeneralWorkTime> GeneralWorktimes)
        {
            bool result = true;
            try
            {
                foreach (GeneralWorkTime GeneralWorktime in GeneralWorktimes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@general_worktime_id",System.Data.SqlDbType.Char)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = GeneralWorktime.GeneralWorktimeId
                        },
                        new SqlParameter("@name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = GeneralWorktime.Name
                        },
                        new SqlParameter("@work_time",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(GeneralWorktime.WorkTime).TimeOfDay
                        },
                        new SqlParameter("@rest_time",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(GeneralWorktime.RestTime).TimeOfDay
                        },
                        new SqlParameter("@break_time",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = GeneralWorktime.BreakTime
                        },
                        new SqlParameter("@color",System.Data.SqlDbType.NChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = GeneralWorktime.Color
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_employee_general_worktime @general_worktime_id,@name,@work_time,@rest_time,@break_time,@color", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        public class GeneralWorkTime 
        {
            public string GeneralWorktimeId { get; set; }
            public string CompanyHash { get; set; }
            public string Name { get; set; }
            public string WorkTime { get; set; }
            public string RestTime { get; set; }
            public int? BreakTime { get; set; }
            public string Color { get; set; }
        }

        // POST: api/add_general_worktime
        [HttpPost("add_general_worktime")]//新增一般上下班
        public async Task<string> add_general_worktime([FromBody] List<GeneralWorkTime> GeneralWorktimes)
        {
            string id = "";
            try
            {
                foreach (GeneralWorkTime GeneralWorktime in GeneralWorktimes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@company_hash",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = GeneralWorktime.CompanyHash
                        },
                        new SqlParameter("@name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = GeneralWorktime.Name
                        },
                        new SqlParameter("@work_time",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(GeneralWorktime.WorkTime).TimeOfDay
                        },
                        new SqlParameter("@rest_time",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(GeneralWorktime.RestTime).TimeOfDay
                        },
                        new SqlParameter("@break_time",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = GeneralWorktime.BreakTime
                        },
                        new SqlParameter("@color",System.Data.SqlDbType.NChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = GeneralWorktime.Color
                        }
                    };
                    var get_company = await(_context.EmployeeGeneralWorktimes
                        .FromSqlRaw("EXECUTE dbo.add_employee_general_worktime @company_hash,@name,@work_time,@rest_time,@break_time,@color", parameters: parameters)
                        ).ToListAsync();
                    id = get_company[0].GeneralWorktimeId;
                }
            }
            catch (Exception)
            {
                id = "";
                throw;
            }
            return id;
        }

        // DELETE: api/EmployeeGeneralWorktimes/DeleteGeneralWorktime/5
        [HttpDelete("DeleteGeneralWorktime/{general_worktime_id}")]
        public async Task<bool> DeleteGeneralWorktime(string general_worktime_id)
        {
            bool result = true;
            try
            {
                var parameters = new[]
                {
                            new SqlParameter("@general_worktime_id",System.Data.SqlDbType.Char)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = general_worktime_id
                            }
                        };
                result = _context.Database.ExecuteSqlRaw("exec delete_employee_general_worktime @general_worktime_id", parameters: parameters) != 0 ? true : false;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        private bool EmployeeGeneralWorktimeExists(string id)
        {
            return _context.EmployeeGeneralWorktimes.Any(e => e.GeneralWorktimeId == id);
        }
    }
}
