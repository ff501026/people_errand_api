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
    public class EmployeeLeaveRecordsController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeLeaveRecordsController(people_errandContext context)
        {
            _context = context;
        }

        [HttpGet("Review_LeaveRecord")]//取得未審核請假資料
        public async Task<IEnumerable> Manager_GetReviewLeaveRecord(string hash_company, string hash_account)
        {
            var review_leaverecord = await (from t in _context.EmployeeLeaveRecords
                                            join a in _context.Employees on t.HashAccount equals a.HashAccount
                                            join b in _context.EmployeeLeaveTypes on t.LeaveTypeId equals b.LeaveTypeId
                                            join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                            where a.CompanyHash == hash_company && t.Review == null
                                            orderby t.CreatedTime
                                            select new
                                            {
                                                LeaveRecordId = t.LeaveRecordsId,
                                                HashAccount = t.HashAccount,
                                                Name = c.Name,
                                                LeaveType = b.Name,
                                                StartDate = t.StartDate,
                                                EndDate = t.EndDate,
                                                Reason = t.Reason,
                                                Review = t.Review,
                                                CreatedTime = t.CreatedTime,
                                            }).ToListAsync();

            var isManager = await _context.ManagerAccounts
                            .Where(db => db.HashAccount == hash_account)
                            .Select(db => db.Enabled).FirstOrDefaultAsync();

            var result = await (from t in _context.ManagerAccounts
                                join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                where t.HashAccount.Equals(hash_account)
                                select new
                                {
                                    PermissionsId = t.PermissionsId,
                                    EmployeeDisplay = a.EmployeeDisplay,
                                    CustomizationDisplay = a.CustomizationDisplay,
                                    EmployeeReview = a.EmployeeReview,
                                    CustomizationReview = a.CustomizationReview,
                                    SettingWorktime = a.SettingWorktime,
                                    SettingDepartmentJobtitle = a.SettingDepartmentJobtitle,
                                    SettingLocation = a.SettingLocation
                                }).ToListAsync();

            if (isManager == false)
            {
                return null;
            }
            else if (result.Count == 0 || result[0].EmployeeReview == 1)
            {
                return review_leaverecord;
            }
            else if (result[0].EmployeeReview == 2)
            {
                review_leaverecord = await (from t in _context.EmployeeLeaveRecords
                                            join a in _context.Employees on t.HashAccount equals a.HashAccount
                                            join b in _context.EmployeeLeaveTypes on t.LeaveTypeId equals b.LeaveTypeId
                                            join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                            where a.ManagerHash == hash_account && t.Review == null
                                            orderby t.CreatedTime
                                            select new
                                            {
                                                LeaveRecordId = t.LeaveRecordsId,
                                                HashAccount = t.HashAccount,
                                                Name = c.Name,
                                                LeaveType = b.Name,
                                                StartDate = t.StartDate,
                                                EndDate = t.EndDate,
                                                Reason = t.Reason,
                                                Review = t.Review,
                                                CreatedTime = t.CreatedTime,
                                            }).ToListAsync();
                return review_leaverecord;
            }
            else 
            {
                var customizationsReview =await( from t in _context.ManagerPermissionsCustomizations
                                           where t.PermissionsId == result[0].CustomizationReview
                                           select new
                                           {
                                               DepartmentId = t.DepartmentId,
                                               JobtitleId = t.JobtitleId
                                           }).ToListAsync();

                List<string> passEmployee = new List<string>();
                foreach (var department_jobtitle in customizationsReview) 
                {
                    var one = await (from t in _context.EmployeeInformations
                                     where t.DepartmentId == department_jobtitle.DepartmentId && t.JobtitleId == department_jobtitle.JobtitleId
                                     select new
                                              {
                                                 HashAccount = t.HashAccount
                                              }).ToListAsync();
                    foreach (var i in one) 
                    {
                        passEmployee.Add(i.HashAccount);
                    }
                }
                List<LeaveRecord> leaveRecords = new List<LeaveRecord>();
                foreach (var i in passEmployee)
                {
                    review_leaverecord = await (from t in _context.EmployeeLeaveRecords
                                                join a in _context.Employees on t.HashAccount equals a.HashAccount
                                                join b in _context.EmployeeLeaveTypes on t.LeaveTypeId equals b.LeaveTypeId
                                                join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                                where a.HashAccount == i && t.Review == null
                                                orderby t.CreatedTime
                                                select new
                                                {
                                                    LeaveRecordId = t.LeaveRecordsId,
                                                    HashAccount = t.HashAccount,
                                                    Name = c.Name,
                                                    LeaveType = b.Name,
                                                    StartDate = t.StartDate,
                                                    EndDate = t.EndDate,
                                                    Reason = t.Reason,
                                                    Review = t.Review,
                                                    CreatedTime = t.CreatedTime,
                                                }).ToListAsync();

                    string jsonData = JsonConvert.SerializeObject(review_leaverecord);
                    List<LeaveRecord> leaverecord = JsonConvert.DeserializeObject<List<LeaveRecord>>(jsonData);
                    foreach (var record in leaverecord)
                    {
                        LeaveRecord search = new LeaveRecord()
                        {
                            LeaveRecordId = record.LeaveRecordId,//請假編號
                            HashAccount = record.HashAccount,//員工編號
                            Name = record.Name,//員工姓名
                            LeaveType = record.LeaveType,//假別
                            StartDate = record.StartDate,//開始時間
                            EndDate = record.EndDate,//結束時間
                            Reason = record.Reason,//備註(事由)
                            Review = record.Review,//審核狀態
                            CreatedTime = record.CreatedTime//申請時間
                        };
                        leaveRecords.Add(search);
                    }
                }
                return leaveRecords;

            }

            //string jsonData = JsonConvert.SerializeObject(isManager);
            //return jsonData;
        }

        public class LeaveRecord
        {
            public int LeaveRecordId { get; set; }//請假編號
            public string HashAccount { get; set; }//員工編號
            public string Name { get; set; }//員工姓名
            public string LeaveType { get; set; }//假別
            public DateTime StartDate { get; set; }//開始時間
            public DateTime EndDate { get; set; }//結束時間
            public string Reason { get; set; }//備註(事由)
            public bool? Review { get; set; }//審核狀態
            public DateTime CreatedTime { get; set; }//申請時間
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

                    result = _context.Database.ExecuteSqlRaw("exec review_leaveRecord @leaveRecord_Id,@review", parameters: parameters) != 0 ? true : false;
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
