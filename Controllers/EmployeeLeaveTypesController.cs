using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using People_errand_api.Models;

namespace People_errand_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeLeaveTypesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeLeaveTypesController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeLeaveTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeLeaveType>>> GetEmployeeLeaveTypes()
        {
            return await _context.EmployeeLeaveTypes.ToListAsync();
        }

        // GET: api/EmployeeLeaveTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeLeaveType>> GetEmployeeLeaveType(int id)
        {
            var employeeLeaveType = await _context.EmployeeLeaveTypes.FindAsync(id);

            if (employeeLeaveType == null)
            {
                return NotFound();
            }

            return employeeLeaveType;
        }

        // PUT: api/EmployeeLeaveTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeLeaveType(int id, EmployeeLeaveType employeeLeaveType)
        {
            if (id != employeeLeaveType.LeaveTypeId)
            {
                return BadRequest();
            }

            _context.Entry(employeeLeaveType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeLeaveTypeExists(id))
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

        // POST: api/EmployeeLeaveTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeLeaveType>> PostEmployeeLeaveType(EmployeeLeaveType employeeLeaveType)
        {
            _context.EmployeeLeaveTypes.Add(employeeLeaveType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployeeLeaveType", new { id = employeeLeaveType.LeaveTypeId }, employeeLeaveType);
        }

        // DELETE: api/EmployeeLeaveTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeLeaveType(int id)
        {
            var employeeLeaveType = await _context.EmployeeLeaveTypes.FindAsync(id);
            if (employeeLeaveType == null)
            {
                return NotFound();
            }

            _context.EmployeeLeaveTypes.Remove(employeeLeaveType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeLeaveTypeExists(int id)
        {
            return _context.EmployeeLeaveTypes.Any(e => e.LeaveTypeId == id);
        }
    }
}
