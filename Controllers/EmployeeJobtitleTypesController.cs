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
    public class EmployeeJobtitleTypesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeJobtitleTypesController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeJobtitleTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeJobtitleType>>> GetEmployeeJobtitleTypes()
        {
            return await _context.EmployeeJobtitleTypes.ToListAsync();
        }

        // GET: api/EmployeeJobtitleTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeJobtitleType>> GetEmployeeJobtitleType(int id)
        {
            var employeeJobtitleType = await _context.EmployeeJobtitleTypes.FindAsync(id);

            if (employeeJobtitleType == null)
            {
                return NotFound();
            }

            return employeeJobtitleType;
        }

        // PUT: api/EmployeeJobtitleTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("UpdateJobtitle")]
        public ActionResult<bool> update_jobtitle([FromBody] List<EmployeeJobtitleType> employeeJobtitleTypes)
        {
            bool result = true;
            try
            {
                foreach (EmployeeJobtitleType employeeJobtitleType in employeeJobtitleTypes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@jobtitle_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeJobtitleType.JobtitleId
                        },
                        new SqlParameter("@jobtitle_name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeJobtitleType.Name
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_jobtitle @jobtitle_id,@jobtitle_name", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                return result;
            }
            return result;
        }


        // POST: api/EmployeeJobtitleTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("add_jobtitle")]
        public ActionResult<bool> add_jobtitle([FromBody]List<EmployeeJobtitleType> employeeJobtitleTypes)
        {
            bool result = true;
            try
            {
                foreach (EmployeeJobtitleType employeeJobtitleType in employeeJobtitleTypes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@jobtitle_name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeJobtitleType.Name
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec add_jobtitle @jobtitle_name", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                return result;
            }
            return result;
        }

        // DELETE: api/EmployeeJobtitleTypes/5
        [HttpDelete("DeleteJobtitle/{jobtitle_id}")]
        public async Task<bool> DeleteJobtitle(int jobtitle_id)
        {
            bool result = true;
            try
            {
                var parameters = new[]
                {
                            new SqlParameter("@jobtitle_id",System.Data.SqlDbType.Int)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = jobtitle_id
                            }
                        };
                result = _context.Database.ExecuteSqlRaw("exec delete_jobtitle @jobtitle_id", parameters: parameters) != 0 ? true : false;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        private bool EmployeeJobtitleTypeExists(int id)
        {
            return _context.EmployeeJobtitleTypes.Any(e => e.JobtitleId == id);
        }
    }
}
