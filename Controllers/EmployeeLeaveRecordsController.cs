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
    public class EmployeeLeaveRecordsController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeLeaveRecordsController(people_errandContext context)
        {
            _context = context;
        }

       

        //// GET: api/EmployeeLeaveRecords/hash_account
        //[HttpGet("{hash_account}")] //(用來看最後一筆)
        //public async Task<ActionResult<EmployeeLeaveRecord>> GetEmployeeLeaveRecords(string hash_account)
        //{
        //    //去employee_leave_record資料表比對hash_account，並回傳資料行
        //    //找到使用者最後一筆資料
        //    var employee_leave_record = await _context.EmployeeLeaveRecords
        //        .Where(db_employee_leave_record => db_employee_leave_record.HashAccount == hash_account)
        //        .OrderBy(db_employee_leave_record => db_employee_leave_record.LeaveRecordsId)
        //        .Select(db_employee_leave_record => db_employee_leave_record).LastOrDefaultAsync();

        //    if (hash_account == null)
        //    {
        //        return NotFound();
        //    }

        //    return employee_leave_record;
        //}

        // GET: api/EmployeeLeaveRecords/GetEmployeeAllLeaveRecords/hash_account
        [HttpGet("GetEmployeeAllLeaveRecords/{hash_account}")] //(用來看全部的請假)
        public async Task<ActionResult<IEnumerable<EmployeeLeaveRecord>>> GetEmployeeAllLeaveRecords(string hash_account)
        {
            //去employee_leave_record資料表比對hash_account，並回傳資料行
            var employee_leave_record = await _context.EmployeeLeaveRecords
                .Where(db_employee_leave_record => db_employee_leave_record.HashAccount == hash_account)
                .OrderBy(db_employee_leave_record => db_employee_leave_record.LeaveRecordsId)
                .Select(db_employee_leave_record => db_employee_leave_record).ToListAsync();

            if (hash_account == null)
            {
                return NotFound();
            }
            if (employee_leave_record.Count == 0)
            {
                return NotFound();
            }

            return employee_leave_record;
        }
        [HttpPut("review_leaveRecord")]
        public ActionResult<bool> review_leaveRecord([FromBody] List<EmployeeLeaveRecord> leaveRecords)
        {
            bool result = true;
            try
            {
                foreach (EmployeeLeaveRecord leaveRecord in leaveRecords)
                //foreach用來讀取多筆資料，假設一個JSON有很多{}
                {
                    var parameters = new[]
                    {

                        new SqlParameter("@leaveRecord_Id", System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.LeaveRecordsId
                        },
                        new SqlParameter("@review", System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.Review
                        }
                    };

                    result = _context.Database.ExecuteSqlRaw("exec review_leaveRecord @leaveRecord_Id @review", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // PUT: api/EmployeeLeaveRecords/update_leaveRecord
        [HttpPut("update_leaveRecord")]
        public ActionResult<bool> update_leaveRecord([FromBody] List<EmployeeLeaveRecord> leaveRecords)
        {
            bool result = true;
            try
            {
                foreach (EmployeeLeaveRecord leaveRecord in leaveRecords)
                {
                    //設定放入查詢的值
                    var parameters = new[]
                    {
                        new SqlParameter("@leaveRecord_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.LeaveRecordsId
                        },
                        new SqlParameter("@start_date",System.Data.SqlDbType.DateTime)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.StartDate
                        },
                        new SqlParameter("@end_date",System.Data.SqlDbType.DateTime)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.EndDate
                        },
                        new SqlParameter("@leave_type_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.LeaveTypeId
                        },
                        new SqlParameter("@reason",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.Reason
                        },
                        new SqlParameter("@review",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.Review
                        }
                    };
                    //執行預存程序
                    result = _context.Database.ExecuteSqlRaw("exec update_leaveRecord @leaveRecord_id,@start_date,@end_date,@leave_type_id,@reason,@review", parameters: parameters) != 0 ? true : false;
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


        // POST: api/EmployeeLeaveRecords/add_leaveRecord
        [HttpPost("add_leaveRecord")]
        public ActionResult<bool> add_leaveRecord([FromBody] List<EmployeeLeaveRecord> leaveRecords)
        {
            bool result = true;
            try
            {
                foreach (EmployeeLeaveRecord leaveRecord in leaveRecords)
                {
                    //設定放入查詢的值
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.HashAccount
                        },
                        new SqlParameter("@start_date",System.Data.SqlDbType.DateTime)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.StartDate
                        },
                        new SqlParameter("@end_date",System.Data.SqlDbType.DateTime)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.EndDate
                        },
                        new SqlParameter("@leave_type_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.LeaveTypeId
                        },
                        new SqlParameter("@reason",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = leaveRecord.Reason
                        }
                    };
                    //執行預存程序
                    result = _context.Database.ExecuteSqlRaw("exec add_leaveRecord @hash_account,@start_date,@end_date,@leave_type_id,@reason", parameters: parameters) != 0 ? true : false;
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

        // DELETE: api/EmployeeLeaveRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeLeaveRecord(int id)
        {
            var employeeLeaveRecord = await _context.EmployeeLeaveRecords.FindAsync(id);
            if (employeeLeaveRecord == null)
            {
                return NotFound();
            }

            _context.EmployeeLeaveRecords.Remove(employeeLeaveRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeLeaveRecordExists(int id)
        {
            return _context.EmployeeLeaveRecords.Any(e => e.LeaveRecordsId == id);
        }
    }
}
