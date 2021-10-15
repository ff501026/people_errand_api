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
    public class EmployeeTrip2RecordController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeTrip2RecordController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeTrip2Record
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeTrip2Record>>> GetEmployeeTrip2Records()
        {
            return await _context.EmployeeTrip2Records.ToListAsync();
        }

        // GET: api/EmployeeTrip2Record/GetEmployeeTrip2Records/hash_account
        [HttpGet("GetEmployeeTrip2Records/{hash_account}")]
        public async Task<ActionResult<IEnumerable<EmployeeTrip2Record>>> GetEmployeeTrip2Record(string hash_account)
        {
            var employeeTrip2Record = await _context.EmployeeTrip2Records
                .Where(db_employee_trip2_record => db_employee_trip2_record.HashAccount == hash_account)
                .OrderBy(db_employee_trip2_record => db_employee_trip2_record.Trip2RecordsId)
                .Select(db_employee_trip2_record => db_employee_trip2_record).ToListAsync();

            if (hash_account == null)
            {
                return NotFound();
            }
            if (employeeTrip2Record.Count == 0)
            {
                return NotFound();
            }
            return employeeTrip2Record;
        }

        // PUT: api/EmployeeTrip2Record/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeTrip2Record(int id, EmployeeTrip2Record employeeTrip2Record)
        {
            if (id != employeeTrip2Record.Trip2RecordsId)
            {
                return BadRequest();
            }

            _context.Entry(employeeTrip2Record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeTrip2RecordExists(id))
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

        // POST: api/EmployeeTrip2Record/add_trip2Record
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("add_trip2Record")]
        public ActionResult<bool> add_trip2Record([FromBody] List<EmployeeTrip2Record> employeeTrip2Records)
        {
            bool result = true;
            try
            {
                foreach (EmployeeTrip2Record employeeTrip2Record in employeeTrip2Records)
                {
                    string location = AttendanceManagement.Models.GoogleMapApiModel.latLngToChineseAddress((double)employeeTrip2Record.CoordinateX, (double)employeeTrip2Record.CoordinateY);

                    //設定放入查詢的值
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeTrip2Record.HashAccount
                        },
                        new SqlParameter("@trip2_type_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeTrip2Record.Trip2TypeId
                        },
                        new SqlParameter("@coordinate_X",System.Data.SqlDbType.Float)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeTrip2Record.CoordinateX
                        },
                        new SqlParameter("@coordinate_Y",System.Data.SqlDbType.Float)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeTrip2Record.CoordinateY
                        },
                        new SqlParameter("@address",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = location
                        }
                    };
                    //執行預存程序
                    result = _context.Database.ExecuteSqlRaw("exec add_trip2Record @hash_account,@trip2_type_id,@coordinate_X,@coordinate_Y,@address", parameters: parameters) != 0 ? true : false;
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

        // DELETE: api/EmployeeTrip2Record/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeTrip2Record(int id)
        {
            var employeeTrip2Record = await _context.EmployeeTrip2Records.FindAsync(id);
            if (employeeTrip2Record == null)
            {
                return NotFound();
            }

            _context.EmployeeTrip2Records.Remove(employeeTrip2Record);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeTrip2RecordExists(int id)
        {
            return _context.EmployeeTrip2Records.Any(e => e.Trip2RecordsId == id);
        }
    }
}
