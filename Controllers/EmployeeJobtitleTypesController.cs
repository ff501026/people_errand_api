using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        [HttpGet("{company_hash}")]
        public async Task<IEnumerable> Get_Jobtitle(string company_hash)
        {
            var jobtitle = await (from t in _context.EmployeeJobtitleTypes
                                    where t.CompanyHash.Equals(company_hash)
                                    select new
                                    {
                                        JobtitleId = t.JobtitleId,
                                        Name = t.Name
                                    }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(jobtitle);
            return jsonData;
        }

        // PUT: api/EmployeeJobtitleTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateJobtitle")]
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
                        },
                        new SqlParameter("@company_hash",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeJobtitleType.CompanyHash
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec add_jobtitle @jobtitle_name,@company_hash", parameters: parameters) != 0 ? true : false;
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
