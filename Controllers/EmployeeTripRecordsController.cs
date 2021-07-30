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
    public class EmployeeTripRecordsController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeTripRecordsController(people_errandContext context)
        {
            _context = context;
        }



        // GET: api/EmployeeTripRecords/hash_account
        [HttpGet("{hash_account}")]
        public async Task<ActionResult<IEnumerable<EmployeeTripRecord>>> GetEmployeeTripRecord(string hash_account)
        {
            var employeeTripRecord = await _context.EmployeeTripRecords
                .Where(db_employeeTripRecord => db_employeeTripRecord.HashAccount == hash_account)
                .Select(db_employeeTripRecord => db_employeeTripRecord).ToListAsync();

            if (hash_account == null)
            {
                return NotFound();
            }
            if (employeeTripRecord.Count == 0)
            {
                return NotFound();
            }

            return employeeTripRecord;
        }

        [HttpPut("review_tripRecord")]
        public ActionResult<bool> review_tripRecord([FromBody] List<EmployeeTripRecord> tripRecords)
        {
            bool result = true;
            try
            {
                foreach (EmployeeTripRecord tripRecord in tripRecords)
                //foreach用來讀取多筆資料，假設一個JSON有很多{}
                {
                    var parameters = new[]
                    {
                    
                        new SqlParameter("@tripRecord_Id", System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.TripRecordsId
                        },
                        new SqlParameter("@review", System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.Review
                        }
                    };

                result = _context.Database.ExecuteSqlRaw("exec review_employee @tripRecord_Id @review", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // Put: api/EmployeeTripRecords/update_tripRecord
        [HttpPut("update_tripRecord")]
        public ActionResult<bool> update_tripRecord([FromBody] List<EmployeeTripRecord> tripRecords)
        {
            //[FromBody] List<EmployeeTripRecord> tripRecords => JSON
            bool result = true;
            try
            {
                foreach (EmployeeTripRecord tripRecord in tripRecords)
                //foreach用來讀取多筆資料，假設一個JSON有很多{}
                {
                    //設定放入查詢的值
                    var parameters = new[]
                    {
                        new SqlParameter("@tripRecord_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.TripRecordsId
                        },
                        new SqlParameter("@start_date",System.Data.SqlDbType.DateTime)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.StartDate
                        },
                        new SqlParameter("@end_date",System.Data.SqlDbType.DateTime)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.EndDate
                        },
                        new SqlParameter("@location",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.Location
                        },
                         new SqlParameter("reason",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.Reason
                        },
                         new SqlParameter("review",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.Review
                        }
                    };

                    result = _context.Database.ExecuteSqlRaw("exec update_tripRecord @tripRecord_id,@start_date,@end_date,@location,@reason,@review", parameters: parameters) != 0 ? true : false;
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

        // POST: api/EmployeeTripRecords/add_tripRecord
        [HttpPost("add_tripRecord")]
        public ActionResult<bool> add_tripRecord([FromBody] List<EmployeeTripRecord> tripRecords)
        {
            //[FromBody] List<EmployeeTripRecord> tripRecords => JSON
            bool result = true;
            try
            {
                foreach (EmployeeTripRecord tripRecord in tripRecords)
                    //foreach用來讀取多筆資料，假設一個JSON有很多{}
                {
                    //設定放入查詢的值
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.HashAccount
                        },
                        new SqlParameter("@start_date",System.Data.SqlDbType.DateTime)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.StartDate
                        },
                        new SqlParameter("@end_date",System.Data.SqlDbType.DateTime)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.EndDate
                        },
                        new SqlParameter("@location",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.Location
                        },
                         new SqlParameter("reason",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = tripRecord.Reason
                        }
                    };

                    result = _context.Database.ExecuteSqlRaw("exec add_tripRecord @hash_account,@start_date,@end_date,@location,@reason", parameters: parameters) != 0 ? true : false;
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

        // DELETE: api/EmployeeTripRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeTripRecord(int id)
        {
            var employeeTripRecord = await _context.EmployeeTripRecords.FindAsync(id);
            if (employeeTripRecord == null)
            {
                return NotFound();
            }

            _context.EmployeeTripRecords.Remove(employeeTripRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeTripRecordExists(int id)
        {
            return _context.EmployeeTripRecords.Any(e => e.TripRecordsId == id);
        }
    }
}
