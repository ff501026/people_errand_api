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
    public class EmployeeWorkRecordsController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeWorkRecordsController(people_errandContext context)
        {
            _context = context;
        }


        // GET: api/EmployeeWorkRecords/hash_account
        [HttpGet("{hash_account}")] //(用來看最後一筆是上班還下班)
        public async Task<ActionResult<EmployeeWorkRecord>> GetEmployeeWorkRecords(string hash_account)
        {
            //去employee_work_record資料表比對hash_account，並回傳資料行
            //找到使用者最後一筆資料
            var employee_work_record = await _context.EmployeeWorkRecords
                .Where(db_employee_work_record => db_employee_work_record.HashAccount == hash_account
                && db_employee_work_record.Enabled.Equals(true))
                .OrderBy(db_employee_work_record => db_employee_work_record.WorkRecordsId)
                .Select(db_employee_work_record => db_employee_work_record).LastOrDefaultAsync();

            if (hash_account == null)
            {
                return NotFound();
            }

            return employee_work_record;
        }

        // GET: api/EmployeeWorkRecords/GetEmployeeAllWorkRecords/hash_account
        [HttpGet("GetEmployeeAllWorkRecords/{hash_account}")] //(用來看全部的上班下班)
        public async Task<ActionResult<IEnumerable<EmployeeWorkRecord>>> GetEmployeeAllWorkRecords(string hash_account)
        {
            //去employee_work_record資料表比對hash_account，並回傳資料行
            var employee_work_record = await _context.EmployeeWorkRecords
                .Where(db_employee_work_record =>db_employee_work_record.HashAccount == hash_account
                && db_employee_work_record.Enabled.Equals(true))
                .OrderBy(db_employee_work_record => db_employee_work_record.WorkRecordsId)
                .Select(db_employee_work_record => db_employee_work_record).ToListAsync();

            if (hash_account == null)
            {
                return NotFound();
            }
            if (employee_work_record.Count == 0)
            {
                return NotFound();
            }

            return employee_work_record;
        }

      

        // PUT: api/EmployeeWorkRecords/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeWorkRecord(int id, EmployeeWorkRecord employeeWorkRecord)
        {
            if (id != employeeWorkRecord.WorkRecordsId)
            {
                return BadRequest();
            }

            _context.Entry(employeeWorkRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeWorkRecordExists(id))
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



        // POST: api/EmployeeWorkRecords/add_workRecord
        [HttpPost("add_workRecord")]
        public ActionResult<bool> add_workRecord([FromBody]List<EmployeeWorkRecord> workRecords)
        {
            bool result = true;
            try
            {
                foreach (EmployeeWorkRecord workRecord in workRecords)
                {
                    string location = AttendanceManagement.Models.GoogleMapApiModel.latLngToChineseAddress((double)employeeTrip2Record.CoordinateX, (double)employeeTrip2Record.CoordinateY);

                    //設定放入查詢的值
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = workRecord.HashAccount
                        },
                        new SqlParameter("@work_type_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = workRecord.WorkTypeId
                        },
                        new SqlParameter("@coordinate_X",System.Data.SqlDbType.Float)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = workRecord.CoordinateX
                        },
                        new SqlParameter("@coordinate_Y",System.Data.SqlDbType.Float)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = workRecord.CoordinateY
                        },
                        new SqlParameter("@address",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = location
                        },
                         new SqlParameter("enabled",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = workRecord.Enabled
                        }
                    };

                    result = _context.Database.ExecuteSqlRaw("exec add_workRecord @hash_account,@work_type_id,@coordinate_X,@coordinate_Y,@address,@enabled", parameters: parameters) != 0 ? true : false;
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

        // DELETE: api/EmployeeWorkRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeWorkRecord(int id)
        {
            var employeeWorkRecord = await _context.EmployeeWorkRecords.FindAsync(id);
            if (employeeWorkRecord == null)
            {
                return NotFound();
            }

            _context.EmployeeWorkRecords.Remove(employeeWorkRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeWorkRecordExists(int id)
        {
            return _context.EmployeeWorkRecords.Any(e => e.WorkRecordsId == id);
        }
    }
}
