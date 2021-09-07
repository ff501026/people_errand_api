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
    public class EmployeeFlexibleWorktimesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeFlexibleWorktimesController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeFlexibleWorktimes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeFlexibleWorktime>>> GetEmployeeFlexibleWorktimes()
        {
            return await _context.EmployeeFlexibleWorktimes.ToListAsync();
        }

        // GET: api/EmployeeFlexibleWorktimes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeFlexibleWorktime>> GetEmployeeFlexibleWorktime(string id)
        {
            var employeeFlexibleWorktime = await _context.EmployeeFlexibleWorktimes.FindAsync(id);

            if (employeeFlexibleWorktime == null)
            {
                return NotFound();
            }

            return employeeFlexibleWorktime;
        }

        // PUT: api/update_flexible_worktime
        [HttpPut("update_flexible_worktime")]//新增一般上下班
        public ActionResult<bool> update_general_worktime([FromBody] List<FlexibleWorkTime> FlexibleWorktimes)
        {
            bool result = true;
            try
            {
                foreach (FlexibleWorkTime FlexibleWorktime in FlexibleWorktimes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@flexible_worktime_id",System.Data.SqlDbType.Char)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = FlexibleWorktime.FlexibleWorktimeId
                        },
                        new SqlParameter("@name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = FlexibleWorktime.Name
                        },
                        new SqlParameter("@work_time_start",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(FlexibleWorktime.WorkTimeStart).TimeOfDay
                        },
                        new SqlParameter("@work_time_end",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(FlexibleWorktime.WorkTimeEnd).TimeOfDay
                        },
                        new SqlParameter("@rest_time_start",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(FlexibleWorktime.RestTimeStart).TimeOfDay
                        },
                        new SqlParameter("@rest_time_end",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(FlexibleWorktime.RestTimeEnd).TimeOfDay
                        },
                        new SqlParameter("@break_time",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = FlexibleWorktime.BreakTime
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_employee_flexible_worktime @flexible_worktime_id,@name,@work_time_start,@work_time_end,@rest_time_start,@rest_time_end,@break_time", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        public class FlexibleWorkTime
        {
            public string FlexibleWorktimeId { get; set; }
            public string CompanyHash { get; set; }
            public string Name { get; set; }
            public string WorkTimeStart { get; set; }
            public string WorkTimeEnd { get; set; }
            public string RestTimeStart { get; set; }
            public string RestTimeEnd { get; set; }
            public int BreakTime { get; set; }
        }

        // POST: api/add_flexible_worktime
        [HttpPost("add_flexible_worktime")]//新增彈性上下班
        public ActionResult<bool> add_flexible_worktime([FromBody] List<FlexibleWorkTime> flexibleWorktimes)
        {
            bool result = true;
            try
            {
                foreach (FlexibleWorkTime flexibleWorkTime in flexibleWorktimes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@company_hash",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = flexibleWorkTime.CompanyHash
                        },
                        new SqlParameter("@name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = flexibleWorkTime.Name
                        },
                        new SqlParameter("@work_time_start",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(flexibleWorkTime.WorkTimeStart).TimeOfDay
                        },
                        new SqlParameter("@work_time_end",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(flexibleWorkTime.WorkTimeEnd).TimeOfDay
                        },
                        new SqlParameter("@rest_time_start",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(flexibleWorkTime.RestTimeStart).TimeOfDay
                        },
                        new SqlParameter("@rest_time_end",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(flexibleWorkTime.RestTimeEnd).TimeOfDay
                        },
                        new SqlParameter("@break_time",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = flexibleWorkTime.BreakTime
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec add_employee_flexible_worktime @company_hash,@name,@work_time_start,@work_time_end,@rest_time_start,@rest_time_end,@break_time", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // DELETE: api/EmployeeFlexibleWorktimes/DeleteFlexibleWorktime/5
        [HttpDelete("DeleteFlexibleWorktime/{flexible_worktime_id}")]
        public async Task<bool> DeleteFlexibleWorktime(string flexible_worktime_id)
        {
            bool result = true;
            try
            {
                var parameters = new[]
                {
                            new SqlParameter("@flexible_worktime_id",System.Data.SqlDbType.Char)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = flexible_worktime_id
                            }
                        };
                result = _context.Database.ExecuteSqlRaw("exec delete_employee_flexible_worktime @flexible_worktime_id", parameters: parameters) != 0 ? true : false;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        private bool EmployeeFlexibleWorktimeExists(string id)
        {
            return _context.EmployeeFlexibleWorktimes.Any(e => e.FlexibleWorktimeId == id);
        }
    }
}
