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
    public class EmployeesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeesController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/phone_code
        [HttpGet("{phone_code}")]
        public async Task<ActionResult<Employee>> GetEmployee(string phone_code)
        {
            var employee = await _context.Employees
                .Where(o => o.PhoneCode == phone_code)
                .Select(o => o).FirstOrDefaultAsync();

            if (phone_code == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee)
        {
            if (id != employee.HashAccount)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        //POST: api/Employees/regist_company
        [HttpPost("regist_company")]
        public async Task<ActionResult<bool>> regist_employee(string employee_name , string phone_code)
        {
            var parameters = new[]
            {
                new SqlParameter("@employee_name",System.Data.SqlDbType.NVarChar)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = employee_name
                },
                new SqlParameter("@phone_code",System.Data.SqlDbType.NVarChar)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = phone_code
                }
            };

            var result = _context.Database.ExecuteSqlRaw("exec regist_employee @employee_name , @phone_code", parameters: parameters);
            //輸出成功與否
            return result != 0 ? true : false;
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
