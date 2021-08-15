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
    public class EmployeeTrip2TypeController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeTrip2TypeController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeTrip2Type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeTrip2Type>>> GetEmployeeTrip2Types()
        {
            return await _context.EmployeeTrip2Types.ToListAsync();
        }

        // GET: api/EmployeeTrip2Type/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeTrip2Type>> GetEmployeeTrip2Type(int id)
        {
            var employeeTrip2Type = await _context.EmployeeTrip2Types.FindAsync(id);

            if (employeeTrip2Type == null)
            {
                return NotFound();
            }

            return employeeTrip2Type;
        }

        // PUT: api/EmployeeTrip2Type/update_trip2type_name
        [HttpPut("update_trip2type_name")]//
        public ActionResult<bool> PutEmployeeTrip2Type([FromBody]List<EmployeeTrip2Type> employeeTrip2Types)
        {
            bool result = false;
            foreach (var employeeTrip2Type in employeeTrip2Types) 
            {
                var parameters = new[]
                    {

                        new SqlParameter("@tripType_Id", System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeTrip2Type.Trip2TypeId
                        },
                        new SqlParameter("@name", System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeTrip2Type.Name
                        }
                    };

                result = _context.Database.ExecuteSqlRaw("exec update_trip2type @tripType_Id,@name", parameters: parameters) != 0 ? true : false;
            }
            return result;
        }

        // POST: api/EmployeeTrip2Type
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeTrip2Type>> PostEmployeeTrip2Type(EmployeeTrip2Type employeeTrip2Type)
        {
            _context.EmployeeTrip2Types.Add(employeeTrip2Type);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployeeTrip2Type", new { id = employeeTrip2Type.Trip2TypeId }, employeeTrip2Type);
        }

        // DELETE: api/EmployeeTrip2Type/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeTrip2Type(int id)
        {
            var employeeTrip2Type = await _context.EmployeeTrip2Types.FindAsync(id);
            if (employeeTrip2Type == null)
            {
                return NotFound();
            }

            _context.EmployeeTrip2Types.Remove(employeeTrip2Type);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeTrip2TypeExists(int id)
        {
            return _context.EmployeeTrip2Types.Any(e => e.Trip2TypeId == id);
        }
    }
}
