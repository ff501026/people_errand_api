using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    public class CompaniesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public CompaniesController(people_errandContext context)
        {
            _context = context;
        }

        [HttpGet("sendGmailAsync")]//公司登入
        public async Task<ActionResult<bool>> sendGmailAsync(string to_email, string email_subject, string email_body)//寄EMAIL
        {
            try
            {
                MailMessage mail = new MailMessage();
                //前面是發信email後面是顯示的名稱
                mail.From = new MailAddress("C108118221@nkust.edu.tw", "差勤打卡");

                //收信者email
                mail.To.Add(to_email);

                //設定優先權
                mail.Priority = MailPriority.Normal;

                //標題
                mail.Subject = email_subject;

                //內容
                mail.Body = email_body;

                //內容使用html
                mail.IsBodyHtml = true;

                //設定gmail的smtp (這是google的)
                SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

                //您在gmail的帳號密碼
                MySmtp.Credentials = new System.Net.NetworkCredential("like3yy@gmail.com", "nkust.edu.tw");

                //開啟ssl
                MySmtp.EnableSsl = true;

                //發送郵件
                await MySmtp.SendMailAsync(mail);

                //放掉宣告出來的MySmtp
                MySmtp = null;

                //放掉宣告出來的mail
                mail.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpGet("Login_Company")]//公司登入
        public async Task<ActionResult<bool>> CheckCode(string code, string password)
        {
            var get_company = await (_context.Companies
                       .FromSqlInterpolated($"EXECUTE dbo.login_employee {code},{password}")
                       ).ToListAsync();

            bool result = get_company.Count != 0 ? true:false;
            return result;
        }

        [HttpGet("Forget_Manager")]//忘記密碼
        public async Task<ActionResult<string>> ForgetManager(string code, string email)
        {
            var get_hashaccount = await (from t in _context.Employees
                                         join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                         join b in _context.Companies on t.CompanyHash equals b.CompanyHash
                                         where b.Code.Equals(code) && a.Email.Equals(email) 
                                         select new
                                         {
                                             HashAccount = t.HashAccount
                                         }).ToListAsync();
            var managerhash = await _context.ManagerAccounts
                .Where(db_manageraccount => db_manageraccount.HashAccount == get_hashaccount[0].HashAccount)
                .Select(db_manageraccount => db_manageraccount.HashAccount).FirstOrDefaultAsync();

            return managerhash;
        }

        [HttpGet("Login_Manager")]//管理員登入
        public async Task<ActionResult<bool>> LoginManager(string code, string email,string password)
        {
            var get_password = await (_context.ManagerAccounts
                       .FromSqlInterpolated($"EXECUTE dbo.login_manager_password {password}")
                       ).ToListAsync();

            var new_password = get_password.Count != 0 ? get_password[0].Password : "";
            var manager = await (from t in _context.Employees
                                join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                join b in _context.Companies on t.CompanyHash equals b.CompanyHash
                                join c in _context.ManagerAccounts on t.HashAccount equals c.HashAccount
                                where b.Code.Equals(code) && a.Email.Equals(email) && c.Password.Equals(new_password) && c.Enabled==true
                                select new
                                {
                                    CompanyHash = b.CompanyHash,
                                    HashAccount = t.HashAccount,
                                    Name = a.Name
                                }).ToListAsync();
            bool result = manager.Count != 0 ? true : false;
            return result;
        }

        [HttpGet("Get_Manager")]
        public async Task<IEnumerable> GetManager(string code, string email, string password)
        {
            var get_password = await (_context.ManagerAccounts
                       .FromSqlInterpolated($"EXECUTE dbo.login_manager_password {password}")
                       ).ToListAsync();

            var new_password = get_password.Count != 0 ? get_password[0].Password : "";
            var result = await (from t in _context.Employees
                                join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                join b in _context.Companies on t.CompanyHash equals b.CompanyHash
                                join c in _context.ManagerAccounts on t.HashAccount equals c.HashAccount
                                where b.Code.Equals(code) && a.Email.Equals(email) && c.Password.Equals(new_password)
                                select new
                                {
                                    CompanyHash = b.CompanyHash,
                                    HashAccount = t.HashAccount,
                                    Name = a.Name
                                }).ToListAsync();
            return result;
        }

        [HttpGet("BoolManager/{hash_account}")]
        public async Task<bool> BoolManager(string hash_account)
        {
            var manager = await (from t in _context.ManagerAccounts
                                where t.HashAccount.Equals(hash_account)
                                select new
                                {
                                    HashAccount = t.HashAccount
                                }).ToListAsync();
            bool result = manager.Count != 0 ? true : false;
            return result;
        }

        [HttpGet("OrganizationChart/{hash_company}")]
        public async Task<IEnumerable> OrganizationChart(string hash_company)
        {
            var Organization = await (from t in _context.Employees
                                 join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                 join b in _context.EmployeeDepartmentTypes on a.DepartmentId equals b.DepartmentId
                                 join c in _context.EmployeeJobtitleTypes on a.JobtitleId equals c.JobtitleId
                                 where t.CompanyHash.Equals(hash_company)
                                 select new
                                 {
                                     HashAccount = t.HashAccount,
                                     Name = a.Name,
                                     ManagerHash = t.ManagerHash,
                                     Department = b.Name,
                                     Jobtitle = c.Name
                                 }).ToListAsync();
            return Organization;
        }

        [HttpGet("Get_Manager_Permissions_Customizations")]
        public IEnumerable GetManagerPermissionsCustomizations()
        {
            var result = from t in _context.ManagerPermissionsCustomizations
                         select t;
            return result;
        }

        [HttpGet("GetManagerRolePermissions/{hash_account}")]
        public async Task<IEnumerable> GetManagerRolePermissions(string hash_account)
        {
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
            return result;
        }

        [HttpGet("Get_Manager_Permissions")]
        public async Task<IEnumerable> GetManagerPermissions(string company_hash)
        {
            var result = await (from t in _context.ManagerPermissions
                                where t.CompanyHash.Equals(company_hash)
                                select new
                                {
                                    PermissionsId =t.PermissionsId,
                                    Name = t.Name,
                                    EmployeeDisplay = t.EmployeeDisplay,
                                    CustomizationDisplay = t.CustomizationDisplay,
                                    EmployeeReview = t.EmployeeReview,
                                    CustomizationReview = t.CustomizationReview,
                                    SettingWorktime = t.SettingWorktime,
                                    SettingDepartmentJobtitle = t.SettingDepartmentJobtitle,
                                    SettingLocation = t.SettingLocation
                                }).ToListAsync();
            return result;
        }

        [HttpGet("CompanyManagerAccountPermissions/{hash_company}")]
        public async Task<IEnumerable> CompanyManagerAccountPermissions(string hash_company)
        {
            var ManagerAccountPermissions = await (from t in _context.ManagerAccounts
                                          join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                          join b in _context.Employees on t.HashAccount equals b.HashAccount
                                          where b.CompanyHash.Equals(hash_company) 
                                          orderby a.DepartmentId
                                          select new
                                          {
                                              HashAccount = t.HashAccount,
                                              DepartmentId = a.DepartmentId,
                                              JobtitleId = a.JobtitleId,
                                              PermisisonsId = t.PermissionsId
                                          }).ToListAsync();

            List<UpdateManagerAccountPermissions> managerAccountPermissions = new List<UpdateManagerAccountPermissions>();
            foreach (var all in ManagerAccountPermissions)
            {
                bool result = true;
                foreach (var x in managerAccountPermissions)
                {
                    if (all.DepartmentId == x.DepartmentId && all.JobtitleId == x.JobtitleId)
                    {
                        result = false;
                        break;
                    }
                }
                if (result)
                {
                    UpdateManagerAccountPermissions managerAccountPermissions1 = new UpdateManagerAccountPermissions
                    {
                        DepartmentId = (int)all.DepartmentId,
                        JobtitleId = (int)all.JobtitleId,
                        PermissionsId = all.PermisisonsId
                    };
                    managerAccountPermissions.Add(managerAccountPermissions1);
                }
            }
            string jsonData = JsonConvert.SerializeObject(managerAccountPermissions);
            return jsonData;
        }

        [HttpGet("CompanyEmployeeWorkTime/{hash_company}")]
        public async Task<IEnumerable> CompanyEmployeeWorkTime(string hash_company)
        {
            var EmployeeWorkTime = await (from t in _context.Employees
                                          join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                          where t.CompanyHash.Equals(hash_company) && t.Enabled !=null
                                          select new
                                          {
                                              HashAccount = t.HashAccount,
                                              DepartmentId = a.DepartmentId,
                                              JobtitleId = a.JobtitleId,
                                              WorktimeId = t.WorktimeId
                                          }).ToListAsync();
            List<EmployeeWorkTime> employeeWorkTimes = new List<EmployeeWorkTime>();
            foreach (var all in EmployeeWorkTime) 
            {
                bool result = true;
                foreach (var x in employeeWorkTimes) 
                {
                    if (all.DepartmentId == x.DepartmentId && all.JobtitleId == x.JobtitleId)
                    {
                        result = false;
                        break;
                    }
                }
                if (result) 
                {
                    EmployeeWorkTime employeeWorkTime = new EmployeeWorkTime
                    {
                        DepartmentId = (int)all.DepartmentId,
                        JobtitleId = (int)all.JobtitleId,
                        WorktimeId = all.WorktimeId
                    };
                    employeeWorkTimes.Add(employeeWorkTime);
                }
            }
            string jsonData = JsonConvert.SerializeObject(employeeWorkTimes);
            return jsonData;
        }

        public partial class EmployeeWorkTime//變更員工上下班時間
        {
            public int DepartmentId { get; set; }
            public int JobtitleId { get; set; }
            public string WorktimeId { get; set; }
        }

        // PUT: api/Companies/UpdateEmployeeWorkTime
        [HttpPut("UpdateEmployeeWorkTime")]//啟用或停用管理員
        public ActionResult<bool> UpdateEmployeeWorkTime([FromBody] List<EmployeeWorkTime> employeeWorkTimes)
        {
            bool result = true;
            try
            {
                foreach (EmployeeWorkTime employeeWorkTime in employeeWorkTimes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@department_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeWorkTime.DepartmentId
                        },
                        new SqlParameter("@jobtitle_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeWorkTime.JobtitleId
                        },
                        new SqlParameter("@worktime_id",System.Data.SqlDbType.Char)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeWorkTime.WorktimeId
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_employee_worktime @department_id,@jobtitle_id,@worktime_id", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        [HttpGet("GetAllGeneralWorkTime/{hash_company}")]
        public async Task<IEnumerable> GetAllGeneralWorkTime(string hash_company)
        {
            var get_general_worktime = await (from t in _context.EmployeeGeneralWorktimes
                                              where t.CompanyHash.Equals(hash_company)
                                              select new
                                              {
                                                  GeneralWorktimeId = t.GeneralWorktimeId,
                                                  Name = t.Name,
                                                  WorkTime = t.WorkTime, 
                                                  RestTime = t.RestTime,
                                                  BreakTime = t.BreakTime,
                                                  Color = t.Color
                                              }).ToListAsync();
            string jsonData = JsonConvert.SerializeObject(get_general_worktime);
            return jsonData;
        }


        [HttpGet("GetAllFlexibleWorkTime/{hash_company}")]
        public async Task<IEnumerable> GetAllFlexibleWorkTime(string hash_company)
        {
            var get_flexible_worktime = await (from t in _context.EmployeeFlexibleWorktimes
                                               where t.CompanyHash.Equals(hash_company)
                                               select new
                                               {
                                                   FlexibleWorktimeId = t.FlexibleWorktimeId,
                                                   Name = t.Name,
                                                   WorkTimeStart = t.WorkTimeStart,
                                                   WorkTimeEnd = t.WorkTimeEnd,
                                                   RestTimeStart = t.RestTimeStart,
                                                   RestTimeEnd = t.RestTimeEnd,
                                                   BreakTime = t.BreakTime,
                                                   Color = t.Color
                                               }).ToListAsync();
            string jsonData = JsonConvert.SerializeObject(get_flexible_worktime);
            return jsonData;
        }

        // GET: api/Companies/company_hash
        [HttpGet("{company_code}")]
        public async Task<ActionResult<string>> GetCompany(string company_code)
        {
            //去company資料表比對company_code，並回傳資料行
            var company_hash = await _context.Companies
                .Where(db_company => db_company.Code == company_code)
                .Select(db_company => db_company.CompanyHash).FirstOrDefaultAsync();
            var coordinate_X = await _context.Companies
                .Where(db_company => db_company.Code == company_code)
                .Select(db_company => db_company.CoordinateX).FirstOrDefaultAsync();
            var coordinate_Y = await _context.Companies
                .Where(db_company => db_company.Code == company_code)
                .Select(db_company => db_company.CoordinateY).FirstOrDefaultAsync();


            if (company_hash == null)
            {
                return NotFound();
            }

            return company_hash + "\n" + coordinate_X + "\n" + coordinate_Y;
        }

        // GET: api/Companies/GetCompanyPositionDifference/company_hash
        [HttpGet("GetCompanyPositionDifference/{company_hash}")]//定位誤差值
        public async Task<ActionResult<int>> GetCompanyPositionDifference(string company_hash)
        {
            //去company資料表比對company_code，並回傳資料行
            var position_difference = await _context.Companies
                .Where(db_company => db_company.CompanyHash == company_hash)
                .Select(db_company => db_company.PositionDifference).FirstOrDefaultAsync();

            if (company_hash == null)
            {
                return NotFound();
            }

            return position_difference;
        }

        // GET: api/Companies/GetCompanySettingTrip2Enabled/company_hash
        [HttpGet("GetCompanySettingTrip2Enabled/{company_hash}")]//是否開啟到站
        public async Task<ActionResult<bool>> GetCompanySettingTrip2Enabled(string company_hash)
        {
            //去company資料表比對company_code，並回傳資料行
            var setting_trip2_enabled = await _context.Companies
                .Where(db_company => db_company.CompanyHash == company_hash)
                .Select(db_company => db_company.SettingTrip2Enabled).FirstOrDefaultAsync();

            if (company_hash == null)
            {
                return NotFound();
            }

            return setting_trip2_enabled;
        }

        // GET: api/Companies/GetCompanySettingWorkRecordEnabled/company_hash
        [HttpGet("GetCompanySettingWorkRecordEnabled/{company_hash}")]//是否開啟定位打卡
        public async Task<ActionResult<bool>> GetCompanySettingWorkRecordEnabled(string company_hash)
        {
            //去company資料表比對company_code，並回傳資料行
            var setting_workrecord_enabled = await _context.Companies
                .Where(db_company => db_company.CompanyHash == company_hash)
                .Select(db_company => db_company.SettingWorkrecordEnabled).FirstOrDefaultAsync();

            if (company_hash == null)
            {
                return NotFound();
            }

            return setting_workrecord_enabled;
        }

        // PUT: api/Companies/UpdateManagerPassword
        [HttpPut("UpdateManagerPassword")]//更新管理員密碼
        public ActionResult<bool> update_manager_password([FromBody] List<ManagerAccount> managerAccounts)
        {
            bool result = true;
            try
            {
                foreach (ManagerAccount managerAccount in managerAccounts)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.HashAccount
                        },
                        new SqlParameter("@manager_password",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.Password
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_manager_account @hash_account,@manager_password", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        public class UpdateManagerAccountPermissions 
        {
            public int DepartmentId { get; set; }
            public int JobtitleId { get; set; }
            public int? PermissionsId { get; set; }
        }


        // PUT: api/Companies/UpdateManagerAccountPermissions
        [HttpPut("UpdateManagerAccountPermissions")]//變更管理員權限
        public ActionResult<bool> update_manager_account_permissions([FromBody] List<UpdateManagerAccountPermissions> managerAccounts)
        {
            bool result = true;
            try
            {
                foreach (UpdateManagerAccountPermissions managerAccount in managerAccounts)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@department_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.DepartmentId
                        },
                        new SqlParameter("@jobtitle_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.JobtitleId
                        },
                        new SqlParameter("@permissions_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.PermissionsId == null ? DBNull.Value : managerAccount.PermissionsId
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_manager_account_permissions @department_id,@jobtitle_id,@permissions_id", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // PUT: api/Companies/UpdateManagerAgent
        [HttpPut("UpdateManagerAgent")]//變更管理員職務代理人
        public ActionResult<bool> update_manager_agent([FromBody] List<ManagerAccount> managerAccounts)
        {
            bool result = true;
            try
            {
                foreach (ManagerAccount managerAccount in managerAccounts)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.HashAccount
                        },
                        new SqlParameter("@hash_agent",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.HashAgent
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_manager_account_agent @hash_account,@hash_agent", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // PUT: api/Companies/UpdateManagerEnabled
        [HttpPut("UpdateManagerEnabled")]//啟用或停用管理員
        public ActionResult<bool> update_manager_enabled([FromBody] List<ManagerAccount> managerAccounts)
        {
            bool result = true;
            try
            {
                foreach (ManagerAccount managerAccount in managerAccounts)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.HashAccount
                        },
                        new SqlParameter("@manager_enabled",System.Data.SqlDbType.Bit)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.Enabled
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_manager_account_enabled @hash_account,@manager_enabled", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // Post: api/Companies/AddLog
        [HttpPost("AddLog")]//新增管理員
        public ActionResult<bool> add_log([FromBody] List<Log> log)
        {
            bool result = true;
            try
            {
                foreach (Log log1 in log)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@url",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = log1.Url
                        },
                        new SqlParameter("@input",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = log1.Input == null ? DBNull.Value : log1.Input
                        },
                        new SqlParameter("@response",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = log1.Response
                        },
                        new SqlParameter("@output",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = log1.Output == null ? DBNull.Value : log1.Output
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec add_log @url,@input,@response,@output", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // Post: api/Companies/AddManagerAccount
        [HttpPost("AddManagerAccount")]//新增管理員
        public ActionResult<bool> add_manager_account([FromBody] List<ManagerAccount> managerAccounts)
        {
            bool result = true;
            try
            {
                foreach (ManagerAccount managerAccount in managerAccounts)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.HashAccount
                        },
                        new SqlParameter("@manager_password",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = managerAccount.Password
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec add_manager_account @hash_account,@manager_password", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // Post: api/Companies/RenewWorktime
        [HttpPut("RenewWorktime")]
        public ActionResult<bool> renew_worktime([FromBody] List<RenewWorktime> renewWorktimes)
        {
            bool result = true;
            try
            {
                foreach (RenewWorktime renewWorktime in renewWorktimes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@Worktime",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = renewWorktime.Worktime
                        },
                        new SqlParameter("@NewWorktime",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = renewWorktime.NewWorktime
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec renew_employee_worktime @Worktime,@NewWorktime", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        public class RenewWorktime
        {
            public string Worktime { get; set; }
            public string NewWorktime { get; set; }
        }

        public class CompanyLogin
        {
            public string CompanyHash { get; set; }
            public string Name { get; set; }
        }//公司登入

        [HttpGet("Get_CompanyHash")]//取得公司HASH
        public async Task<CompanyLogin> Get_CompanyHash(string code,string password)
        {
            var get_company =await( _context.Companies
                        .FromSqlInterpolated($"EXECUTE dbo.login_employee {code},{password}")
                        ).ToListAsync();
            CompanyLogin company_login = new CompanyLogin
            {
                CompanyHash = get_company.Count !=0 ? get_company[0].CompanyHash : "",
                Name = get_company.Count != 0 ? get_company[0].Name : ""
            };
           
            return company_login;
        }
        [HttpGet("Get_CompanyAddress")]//取得公司地址
        public async Task<IEnumerable> Get_CompanyAddress(string company_hash)
        {
            var company = await (from t in _context.Companies
                                 where t.CompanyHash == company_hash
                                 select new
                                 {
                                     CompanyHash = t.CompanyHash,
                                     Address = t.Address,
                                     CoordinateX = t.CoordinateX,
                                     CoordinateY = t.CoordinateY
                                 }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(company);
            return jsonData;
        }

        // PUT: api/Companies/Update_CompanyAddress
        [HttpPut("Update_CompanyAddress")]//更新公司地址
        public ActionResult<bool> Update_CompanyAddress([FromBody] List<Company> companies)
        {
            bool result = true;
            try
            {
                foreach (var company in companies) {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_company",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = company.CompanyHash
                        },
                        new SqlParameter("@address",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = company.Address
                        },
                        new SqlParameter("@CoordinateX",System.Data.SqlDbType.Float)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = company.CoordinateX
                        },
                        new SqlParameter("@CoordinateY",System.Data.SqlDbType.Float)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = company.CoordinateY
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_CompanyAddress @hash_company,@address,@CoordinateX,@CoordinateY", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }


        // PUT: api/Companies/Update_WorkTime_RestTIme
        [HttpGet("Update_WorkTime_RestTime")]//更新公司上班時間
        public ActionResult<bool> update_worktime_resttime(string companyhash,string worktime ,string resttime)
        {
            bool result = true;
            try { 

                    var parameters = new[]
                    {
                        new SqlParameter("@hash_company",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = companyhash
                        },
                        new SqlParameter("@work_time",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(worktime).TimeOfDay
                        },
                        new SqlParameter("@rest_time",System.Data.SqlDbType.Time)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = DateTime.Parse(resttime).TimeOfDay
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_company_worktime_resttime @hash_company,@work_time,@rest_time", parameters: parameters) != 0 ? true : false;
                
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // PUT: api/Companies/UpdateManagerPassword
        [HttpPut("UpdateCompanyPassword")]//更新公司密碼
        public ActionResult<bool> update_company_password([FromBody] List<Company> companies)
        {
            bool result = true;
            try
            {
                foreach (Company company in companies)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_company",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = company.CompanyHash
                        },
                        new SqlParameter("@manager_password",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = company.ManagerPassword
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_company_manager_password @hash_company,@manager_password", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        //// POST: api/Companies/regist_company
        //[HttpPost("regist_company")]//註冊公司
        //public ActionResult<bool> regist_company([FromBody] List<Company> companies)
        //{
        //    bool result = true;
        //    try
        //    {
        //        foreach (Company company in companies)
        //        {
        //            //設定放入查詢的值
        //            var parameters = new[]
        //        {
        //                new SqlParameter("@company_name",System.Data.SqlDbType.NVarChar)
        //                {
        //                    Direction = System.Data.ParameterDirection.Input,
        //                    Value = company.Name
        //                },
        //                new SqlParameter("@company_password",System.Data.SqlDbType.VarChar)
        //                {
        //                    Direction = System.Data.ParameterDirection.Input,
        //                    Value = company.ManagerPassword
        //                }
        //            };
        //            result = _context.Database.ExecuteSqlRaw("exec regist_company @company_name,@company_password", parameters: parameters) != 0 ? true : false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = false;
        //        throw;
        //    }


        //    //輸出成功與否
        //    return result;
        //}

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("Get_WorkTime_RestTime/{hash_company}")]//取得上下班時間
        public async Task<IEnumerable> WorkTime_RestTime(string hash_company)
        {
            var worktime_resttime = await (from t in _context.Companies where t.CompanyHash == hash_company 
                                         select new
                                         {
                                            WorkTime = t.WorkTime,
                                            RestTime = t.RestTime
                                         }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(worktime_resttime);
            return jsonData;
        }

        [HttpGet("Review_Employee/{hash_company}")]//取得未審核員工資料
        public async Task<IEnumerable> Review_employee(string hash_company)
        {
            var review_employee = await (from t in _context.Employees
                                         join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount 
                                         join b in _context.Companies on t.CompanyHash equals b.CompanyHash
                                         where t.CompanyHash == hash_company && t.Enabled == null
                                         orderby t.CreatedTime
                                         select new
                                         {
                                             HashAccount = t.HashAccount,
                                             Companyid = b.Code,
                                             Name = a.Name,
                                             Email = a.Email,
                                             PhoneCode = t.PhoneCode,
                                             CreatedTime = t.CreatedTime,
                                             Enabled = t.Enabled
                                         }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(review_employee);
            return jsonData;
        }

        [HttpGet("Manager_GetPassEmployee2/{hash_account}")]//取得已審核員工資料
        public async Task<List<PassEmployee>> Manager_GetPassEmployee2(string hash_account)
        {
            var pass_employee = await (from t in _context.Employees
                                       join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                       join b in _context.EmployeeDepartmentTypes on a.DepartmentId equals b.DepartmentId
                                       join c in _context.EmployeeJobtitleTypes on a.JobtitleId equals c.JobtitleId
                                       where t.ManagerHash == hash_account && t.Enabled != null
                                       orderby a.Name
                                       select new
                                       {
                                           HashAccount = t.HashAccount,
                                           ManagerHash = t.ManagerHash,
                                           Name = a.Name,
                                           Phone = a.Phone,
                                           Department = b.Name,
                                           Jobtitle = c.Name,
                                           Email = a.Email,
                                           PhoneCode = t.PhoneCode,
                                           WorktimeId = t.WorktimeId,
                                           Enabled = t.Enabled
                                       }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(pass_employee);
            List<PassEmployee> passEmployees = JsonConvert.DeserializeObject<List<PassEmployee>>(jsonData);
            if (await BoolAgent(hash_account))//需要代理
            {
                var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();//找到要代理的人
                var bossCompany = await _context.Employees
                           .Where(db => db.HashAccount == bossHash)
                           .Select(db => db.CompanyHash).FirstOrDefaultAsync();//找到要代理的人

                var boss_permissions = await (from t in _context.ManagerAccounts
                                              join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                              where t.HashAccount.Equals(bossHash)
                                              select new
                                              {
                                                  EmployeeDisplay = a.EmployeeDisplay
                                              }).ToListAsync();

                var boss_display_id = boss_permissions[0].EmployeeDisplay;

                switch (boss_display_id)
                {
                    case 1:
                        List<PassEmployee> passEmployees1 = await Pass_employee(bossCompany);
                        passEmployees = passEmployees1;
                        break;
                    case 2:
                        List<PassEmployee> passEmployees2 = await Manager_GetPassEmployee2(bossHash);
                        foreach (var pass in passEmployees2)
                        {
                            if (passEmployees.FindIndex(item => item.HashAccount == pass.HashAccount) == -1)
                            {
                                PassEmployee search = new PassEmployee()
                                {
                                    HashAccount = pass.HashAccount,//員工編號
                                    Name = pass.Name,//員工姓名
                                    Phone = pass.Phone,//員工電話
                                    Department = pass.Department,//員工部門
                                    JobTitle = pass.JobTitle,//員工職稱
                                    Email = pass.Email, //員工電子郵件
                                    PhoneCode = pass.PhoneCode,//員工驗證碼(phone_code)
                                    Enabled = pass.Enabled,
                                    WorktimeId = pass.WorktimeId,
                                    ManagerHash = pass.ManagerHash
                                };
                                passEmployees.Add(search);
                            }
                        }
                        break;
                    case 3:
                        List<PassEmployee> passEmployees3 = await Manager_GetPassEmployee3(bossHash);
                        foreach (var pass in passEmployees3)
                        {
                            if (passEmployees.FindIndex(item => item.HashAccount == pass.HashAccount) == -1)
                            {
                                PassEmployee search = new PassEmployee()
                                {
                                    HashAccount = pass.HashAccount,//員工編號
                                    Name = pass.Name,//員工姓名
                                    Phone = pass.Phone,//員工電話
                                    Department = pass.Department,//員工部門
                                    JobTitle = pass.JobTitle,//員工職稱
                                    Email = pass.Email, //員工電子郵件
                                    PhoneCode = pass.PhoneCode,//員工驗證碼(phone_code)
                                    Enabled = pass.Enabled,
                                    WorktimeId = pass.WorktimeId,
                                    ManagerHash = pass.ManagerHash
                                };
                                passEmployees.Add(search);
                            }
                        }
                        break;
                }

            }
            return passEmployees;
        }
        public class PassEmployee
        {
            public string HashAccount { get; set; }//員工編號
            public string ManagerHash { get; set; }//管理員編號
            public string Name { get; set; }//員工姓名
            public string Phone { get; set; }//員工電話
            public string Department { get; set; }//員工部門
            public string JobTitle { get; set; }//員工職稱
            public string Email { get; set; }//員工電子郵件
            public string PhoneCode { get; set; }//員工驗證碼(phone_code)
            public string WorktimeId { get; set; }//上下班代碼
            public bool? Enabled { get; set; }//使用狀態
        }//已審核員工資料

        [HttpGet("Manager_GetPassEmployee3/{hash_account}")]//取得已審核員工資料
        public async Task<List<PassEmployee>> Manager_GetPassEmployee3(string hash_account)
        {
            var permissions_id = await _context.ManagerAccounts
                            .Where(db => db.HashAccount == hash_account)
                            .Select(db => db.PermissionsId).FirstOrDefaultAsync();

            var customeizationDisplayId = await _context.ManagerPermissions
                            .Where(db => db.PermissionsId == permissions_id)
                            .Select(db => db.CustomizationDisplay).FirstOrDefaultAsync();

            var customizationsDisplay = await (from t in _context.ManagerPermissionsCustomizations
                                              where t.PermissionsId == customeizationDisplayId
                                               select new
                                              {
                                                  DepartmentId = t.DepartmentId,
                                                  JobtitleId = t.JobtitleId
                                              }).ToListAsync();
            List<PassEmployee> passes = new List<PassEmployee>();
            foreach (var display in customizationsDisplay)
            {
                var pass_employee = await (from t in _context.Employees
                                           join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                           join b in _context.EmployeeDepartmentTypes on a.DepartmentId equals b.DepartmentId
                                           join c in _context.EmployeeJobtitleTypes on a.JobtitleId equals c.JobtitleId
                                           where a.DepartmentId == display.DepartmentId && a.JobtitleId==display.JobtitleId
                                           orderby a.Name
                                           select new
                                           {
                                               HashAccount = t.HashAccount,
                                               ManagerHash = t.ManagerHash,
                                               Name = a.Name,
                                               Phone = a.Phone,
                                               Department = b.Name,
                                               Jobtitle = c.Name,
                                               Email = a.Email,
                                               PhoneCode = t.PhoneCode,
                                               WorktimeId = t.WorktimeId,
                                               Enabled = t.Enabled
                                           }).ToListAsync();

                string jsonData = JsonConvert.SerializeObject(pass_employee);
                List<PassEmployee> passEmployees = JsonConvert.DeserializeObject<List<PassEmployee>>(jsonData);
                foreach (var passEmployee in passEmployees)
                {
                    PassEmployee search = new PassEmployee()
                    {
                        HashAccount = passEmployee.HashAccount,//員工編號
                        Name = passEmployee.Name,//員工姓名
                        Phone = passEmployee.Phone,//員工電話
                        Department = passEmployee.Department,//員工部門
                        JobTitle = passEmployee.JobTitle,//員工職稱
                        Email = passEmployee.Email, //員工電子郵件
                        PhoneCode = passEmployee.PhoneCode,//員工驗證碼(phone_code)
                        Enabled = passEmployee.Enabled,
                        WorktimeId = passEmployee.WorktimeId,
                        ManagerHash = passEmployee.ManagerHash
                    };
                    passes.Add(search);
                }
            }
            if (await BoolAgent(hash_account))//需要代理
            {
                var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();//找到要代理的人
                var bossCompany = await _context.Employees
                           .Where(db => db.HashAccount == bossHash)
                           .Select(db => db.CompanyHash).FirstOrDefaultAsync();//找到要代理的人

                var boss_permissions = await (from t in _context.ManagerAccounts
                                              join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                              where t.HashAccount.Equals(bossHash)
                                              select new
                                              {
                                                  EmployeeDisplay = a.EmployeeDisplay
                                              }).ToListAsync();

                var boss_display_id = boss_permissions[0].EmployeeDisplay;

                switch (boss_display_id)
                {
                    case 1:
                        List<PassEmployee> passEmployees = await Pass_employee(bossCompany);
                        passes = passEmployees;
                        break;
                    case 2:
                        List<PassEmployee> passEmployees2 = await Manager_GetPassEmployee2(bossHash);
                        foreach (var pass in passEmployees2)
                        {
                            if (passes.FindIndex(item => item.HashAccount == pass.HashAccount) == -1)
                            {
                                PassEmployee search = new PassEmployee()
                                {
                                    HashAccount = pass.HashAccount,//員工編號
                                    Name = pass.Name,//員工姓名
                                    Phone = pass.Phone,//員工電話
                                    Department = pass.Department,//員工部門
                                    JobTitle = pass.JobTitle,//員工職稱
                                    Email = pass.Email, //員工電子郵件
                                    PhoneCode = pass.PhoneCode,//員工驗證碼(phone_code)
                                    Enabled = pass.Enabled,
                                    WorktimeId = pass.WorktimeId,
                                    ManagerHash = pass.ManagerHash
                                };
                                passes.Add(search);
                            }
                        }
                        break;
                    case 3:
                        List<PassEmployee> passEmployees3 = await Manager_GetPassEmployee3(bossHash);
                        foreach (var pass in passEmployees3)
                        {
                            if (passes.FindIndex(item => item.HashAccount == pass.HashAccount) == -1)
                            {
                                PassEmployee search = new PassEmployee()
                                {
                                    HashAccount = pass.HashAccount,//員工編號
                                    Name = pass.Name,//員工姓名
                                    Phone = pass.Phone,//員工電話
                                    Department = pass.Department,//員工部門
                                    JobTitle = pass.JobTitle,//員工職稱
                                    Email = pass.Email, //員工電子郵件
                                    PhoneCode = pass.PhoneCode,//員工驗證碼(phone_code)
                                    Enabled = pass.Enabled,
                                    WorktimeId = pass.WorktimeId,
                                    ManagerHash = pass.ManagerHash
                                };
                                passes.Add(search);
                            }
                        }
                        break;
                }

            }
            return passes;
        }

        [HttpGet("Pass_Employee/{hash_company}")]//取得已審核員工資料
        public async Task<List<PassEmployee>> Pass_employee(string hash_company)
        {
            var pass_employee = await (from t in _context.Employees
                                         join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                         join b in _context.EmployeeDepartmentTypes on a.DepartmentId equals b.DepartmentId
                                         join c in _context.EmployeeJobtitleTypes on a.JobtitleId equals c.JobtitleId
                                         where t.CompanyHash == hash_company && t.Enabled != null
                                         orderby a.Name
                                         select new
                                         {
                                             HashAccount = t.HashAccount,
                                             ManagerHash = t.ManagerHash,
                                             Name = a.Name,
                                             Phone = a.Phone,
                                             Department = b.Name,
                                             Jobtitle = c.Name,
                                             Email = a.Email,
                                             PhoneCode = t.PhoneCode,
                                             WorktimeId =t.WorktimeId,
                                             Enabled = t.Enabled
                                         }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(pass_employee);
            List<PassEmployee> passEmployees = JsonConvert.DeserializeObject<List<PassEmployee>>(jsonData);
            return passEmployees;
        }

        [HttpGet("Get_All_Manager/{hash_company}")]//取得公司全部管理員資料
        public async Task<IEnumerable> Get_All_Manager(string hash_company)
        {
            var all_manager = await (from t in _context.ManagerAccounts
                                       join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                       join b in _context.EmployeeDepartmentTypes on a.DepartmentId equals b.DepartmentId
                                       join c in _context.EmployeeJobtitleTypes on a.JobtitleId equals c.JobtitleId
                                       join d in _context.Employees on t.HashAccount equals d.HashAccount  
                                       where d.CompanyHash == hash_company 
                                       orderby a.Name
                                       select new
                                       {
                                           ManagerHash = t.HashAccount,
                                           AgentHash = t.HashAgent,
                                           Name = a.Name,
                                           Email = a.Email,
                                           Department = b.Name,
                                           Jobtitle = c.Name,
                                           PermissionsId = t.PermissionsId,
                                           Enabled = t.Enabled
                                       }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(all_manager);
            return jsonData;
        }

        public partial class WorkRecord
        {
            public string HashAccount { get; set; }
            public int Num { get; set; }
            public string Name { get; set; }
            public DateTime WorkTime { get; set; }
            public DateTime RestTime { get; set; }
            public int WorkRecordId { get; set; }
        }//打卡紀錄Model

        [HttpGet("GetWorkRecord/{hash_company}")]//取得員工打卡紀錄
        public async Task<List<WorkRecord>> GetWorkRecord(string hash_company)
        {
            var Employee_Record = await (from t in _context.EmployeeWorkRecords
                                         join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                         join b in _context.Employees on t.HashAccount equals b.HashAccount
                                         where b.CompanyHash == hash_company && t.Enabled == true
                                         orderby t.CreatedTime 
                                         select new
                                         {
                                             HashAccount = t.HashAccount,
                                             員工姓名 = a.Name,
                                             紀錄時間 = t.CreatedTime,
                                             X_coordinate = t.CoordinateX,
                                             Y_coordinate = t.CoordinateY,
                                             Work_type = t.WorkTypeId,
                                         }).ToListAsync();

            List<WorkRecord> workRecord = new List<WorkRecord>();

            //計算JSON總長度
            int length = 0;
            foreach (var ever in Employee_Record)
            {
                length++;
            }

            //recorded接已完成登入的打卡紀錄
            List<int> recorded = new List<int>();
            int num = 0; //編號

            for (int i = 0; i < length-1; i++) //i=JSON裡的每筆上班紀錄
            {
                foreach (int pass in recorded) //pass=已完成登入的紀錄
                {
                    if (i == pass)//如果i筆記錄已完成登入，則換查看下一筆
                    {
                        i++;
                    }
                }
                var hashaccount = Employee_Record[i].HashAccount; //取得i筆上班紀錄的員工編號
                var name = Employee_Record[i].員工姓名;

                for (int j = 1; j <= length-1; j++)//從第1筆紀錄開始找i筆上班紀錄的下班紀錄
                {
                    foreach (int pass in recorded)//pass=已完成登入的紀錄
                    {
                        if (j == pass)
                        {
                            j++;//如果i筆記錄已完成登入，則換查看下一筆
                        }
                    }
                    if (hashaccount == Employee_Record[j].HashAccount && j != i)//如果第j筆資料員工編號與i筆資料相同，且與i為不同筆則登入至後台
                    {
                        num++;//編號
                        var worktime = Employee_Record[i].紀錄時間;//下班時間
                        var resttime = Employee_Record[j].紀錄時間;//上班時間
                        recorded.Add(i);//完成登入的該筆記錄至recorded
                        recorded.Add(j);//完成登入的該筆記錄至recorded

                        for (int k = 1; k <= recorded.Count - 1; k++)
                        {//執行的回數
                            for (int m = 1; m <= recorded.Count- k; m++)//執行的次數
                            {
                                if (recorded[m] < recorded[m - 1])
                                {
                                    //二數交換
                                    int temp = recorded[m];
                                    recorded[m] = recorded[m - 1];
                                    recorded[m - 1] = temp;
                                }
                            }
                        }
                        workRecord.Add(new WorkRecord
                        {
                            HashAccount = hashaccount,
                            Num = num,
                            Name = name,
                            WorkTime = worktime,
                            RestTime = resttime
                        });

                        break;
                    }
                }

            }
            return workRecord;
        }

        [HttpGet("GetWorkRecord2/{hash_account}")]//取得員工打卡紀錄
        public async Task<List<WorkRecord>> GetWorkReccord2(string hash_account)
        {
            var Employee_Record = await (from t in _context.EmployeeWorkRecords
                                         join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                         join b in _context.Employees on t.HashAccount equals b.HashAccount
                                         where b.ManagerHash == hash_account && t.Enabled == true
                                         orderby t.CreatedTime
                                         select new
                                         {
                                             HashAccount = t.HashAccount,
                                             員工姓名 = a.Name,
                                             紀錄時間 = t.CreatedTime,
                                             X_coordinate = t.CoordinateX,
                                             Y_coordinate = t.CoordinateY,
                                             Work_type = t.WorkTypeId,
                                             WorkRecordId = t.WorkRecordsId
                                         }).ToListAsync();

            List<WorkRecord> workRecord = new List<WorkRecord>();

            //計算JSON總長度
            int length = 0;
            foreach (var ever in Employee_Record)
            {
                length++;
            }

            //recorded接已完成登入的打卡紀錄
            List<int> recorded = new List<int>();
            int num = 0; //編號

            for (int i = 0; i < length - 1; i++) //i=JSON裡的每筆上班紀錄
            {
                foreach (int pass in recorded) //pass=已完成登入的紀錄
                {
                    if (i == pass)//如果i筆記錄已完成登入，則換查看下一筆
                    {
                        i++;
                    }
                }
                var hashaccount = Employee_Record[i].HashAccount; //取得i筆上班紀錄的員工編號
                var name = Employee_Record[i].員工姓名;

                for (int j = 1; j <= length - 1; j++)//從第1筆紀錄開始找i筆上班紀錄的下班紀錄
                {
                    foreach (int pass in recorded)//pass=已完成登入的紀錄
                    {
                        if (j == pass)
                        {
                            j++;//如果i筆記錄已完成登入，則換查看下一筆
                        }
                    }
                    if (hashaccount == Employee_Record[j].HashAccount && j != i)//如果第j筆資料員工編號與i筆資料相同，且與i為不同筆則登入至後台
                    {
                        num++;//編號
                        var worktime = Employee_Record[i].紀錄時間;//下班時間
                        var resttime = Employee_Record[j].紀錄時間;//上班時間
                        recorded.Add(i);//完成登入的該筆記錄至recorded
                        recorded.Add(j);//完成登入的該筆記錄至recorded

                        for (int k = 1; k <= recorded.Count - 1; k++)
                        {//執行的回數
                            for (int m = 1; m <= recorded.Count - k; m++)//執行的次數
                            {
                                if (recorded[m] < recorded[m - 1])
                                {
                                    //二數交換
                                    int temp = recorded[m];
                                    recorded[m] = recorded[m - 1];
                                    recorded[m - 1] = temp;
                                }
                            }
                        }
                        workRecord.Add(new WorkRecord
                        {
                            HashAccount = hashaccount,
                            Num = num,
                            Name = name,
                            WorkTime = worktime,
                            RestTime = resttime,
                            WorkRecordId = Employee_Record[j].WorkRecordId
                        });

                        break;
                    }
                }

            }
            if (await BoolAgent(hash_account))//需要代理
            {
                var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();//找到要代理的人
                var bossCompany = await _context.Employees
                           .Where(db => db.HashAccount == bossHash)
                           .Select(db => db.CompanyHash).FirstOrDefaultAsync();//找到要代理的人

                var boss_permissions = await (from t in _context.ManagerAccounts
                                              join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                              where t.HashAccount.Equals(bossHash)
                                              select new
                                              {
                                                  EmployeeDisplay = a.EmployeeDisplay
                                              }).ToListAsync();

                var boss_display_id = boss_permissions[0].EmployeeDisplay;

                switch (boss_display_id)
                {
                    case 1:
                        List<WorkRecord> workRecords = await GetWorkRecord(bossCompany);
                        workRecord = workRecords;
                        break;
                    case 2:
                        List<WorkRecord> workRecords2 = await GetWorkReccord2(bossHash);
                        foreach (var work in workRecords2)
                        {
                            if (workRecord.FindIndex(item => item.WorkRecordId == work.WorkRecordId) == -1)
                            {
                                WorkRecord search = new WorkRecord()
                                {
                                    HashAccount = work.HashAccount,
                                    Num = work.Num,
                                    Name = work.Name,
                                    WorkTime = work.WorkTime,
                                    RestTime = work.RestTime,
                                    WorkRecordId = work.WorkRecordId
                                };
                                workRecord.Add(search);
                            }
                        }
                        break;
                    case 3:
                        List<WorkRecord> workRecords3 = await GetWorkRecord3(bossHash);
                        foreach (var work in workRecords3)
                        {
                            if (workRecord.FindIndex(item => item.WorkRecordId == work.WorkRecordId) == -1)
                            {
                                WorkRecord search = new WorkRecord()
                                {
                                    HashAccount = work.HashAccount,
                                    Num = work.Num,
                                    Name = work.Name,
                                    WorkTime = work.WorkTime,
                                    RestTime = work.RestTime,
                                    WorkRecordId = work.WorkRecordId
                                };
                                workRecord.Add(search);
                            }
                        }
                        break;
                }

            }
            return workRecord;
        }

        public class WorkRecord3
        {
            public string HashAccount { get; set; }
            public string 員工姓名 { get; set; }
            public DateTime 紀錄時間 { get; set; }
            public double X_coordinate { get; set; }
            public double Y_coordinate { get; set; }
            public int Work_type { get; set; }
            public int WorkRecordId { get; set; }
        }
        [HttpGet("GetWorkRecord3/{hash_account}")]//取得員工打卡紀錄
        public async Task<List<WorkRecord>> GetWorkRecord3(string hash_account)
        {
            var permissions_id = await _context.ManagerAccounts
                            .Where(db => db.HashAccount == hash_account)
                            .Select(db => db.PermissionsId).FirstOrDefaultAsync();

            var customeizationDisplayId = await _context.ManagerPermissions
                            .Where(db => db.PermissionsId == permissions_id)
                            .Select(db => db.CustomizationDisplay).FirstOrDefaultAsync();

            var customizationsDisplay = await (from t in _context.ManagerPermissionsCustomizations
                                               where t.PermissionsId == customeizationDisplayId
                                               select new
                                               {
                                                   DepartmentId = t.DepartmentId,
                                                   JobtitleId = t.JobtitleId
                                               }).ToListAsync();
            List<WorkRecord3> Employee_Record = new List<WorkRecord3>();
            foreach (var display in customizationsDisplay)
            {
                var work = await (from t in _context.EmployeeWorkRecords
                                             join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                             join b in _context.Employees on t.HashAccount equals b.HashAccount
                                             where a.DepartmentId==display.DepartmentId && a.JobtitleId==display.JobtitleId && t.Enabled == true
                                             orderby t.CreatedTime
                                             select new
                                             {
                                                 HashAccount = t.HashAccount,
                                                 員工姓名 = a.Name,
                                                 紀錄時間 = t.CreatedTime,
                                                 X_coordinate = t.CoordinateX,
                                                 Y_coordinate = t.CoordinateY,
                                                 Work_type = t.WorkTypeId,
                                                 WorkRecordId = t.WorkRecordsId
                                             }).ToListAsync();

                string json = JsonConvert.SerializeObject(work);
                List<WorkRecord3> works = JsonConvert.DeserializeObject<List<WorkRecord3>>(json);
                foreach (var work1 in works)
                {
                    WorkRecord3 search = new WorkRecord3()
                    {
                        HashAccount = work1.HashAccount,
                        員工姓名 = work1.員工姓名,
                        紀錄時間 = work1.紀錄時間,
                        X_coordinate = work1.X_coordinate,
                        Y_coordinate = work1.Y_coordinate,
                        Work_type = work1.Work_type,
                        WorkRecordId = work1.WorkRecordId
                    };
                    Employee_Record.Add(search);
                }
            }

            List<WorkRecord> workRecord = new List<WorkRecord>();

            //計算JSON總長度
            int length = 0;
            foreach (var ever in Employee_Record)
            {
                length++;
            }

            //recorded接已完成登入的打卡紀錄
            List<int> recorded = new List<int>();
            int num = 0; //編號

            for (int i = 0; i < length - 1; i++) //i=JSON裡的每筆上班紀錄
            {
                foreach (int pass in recorded) //pass=已完成登入的紀錄
                {
                    if (i == pass)//如果i筆記錄已完成登入，則換查看下一筆
                    {
                        i++;
                    }
                }
                var hashaccount = Employee_Record[i].HashAccount; //取得i筆上班紀錄的員工編號
                var name = Employee_Record[i].員工姓名;

                for (int j = 1; j <= length - 1; j++)//從第1筆紀錄開始找i筆上班紀錄的下班紀錄
                {
                    foreach (int pass in recorded)//pass=已完成登入的紀錄
                    {
                        if (j == pass)
                        {
                            j++;//如果i筆記錄已完成登入，則換查看下一筆
                        }
                    }
                    if (hashaccount == Employee_Record[j].HashAccount && j != i)//如果第j筆資料員工編號與i筆資料相同，且與i為不同筆則登入至後台
                    {
                        num++;//編號
                        var worktime = Employee_Record[i].紀錄時間;//下班時間
                        var resttime = Employee_Record[j].紀錄時間;//上班時間
                        recorded.Add(i);//完成登入的該筆記錄至recorded
                        recorded.Add(j);//完成登入的該筆記錄至recorded

                        for (int k = 1; k <= recorded.Count - 1; k++)
                        {//執行的回數
                            for (int m = 1; m <= recorded.Count - k; m++)//執行的次數
                            {
                                if (recorded[m] < recorded[m - 1])
                                {
                                    //二數交換
                                    int temp = recorded[m];
                                    recorded[m] = recorded[m - 1];
                                    recorded[m - 1] = temp;
                                }
                            }
                        }
                        workRecord.Add(new WorkRecord
                        {
                            HashAccount = hashaccount,
                            Num = num,
                            Name = name,
                            WorkTime = worktime,
                            RestTime = resttime,
                            WorkRecordId = Employee_Record[j].WorkRecordId
                        });

                        break;
                    }
                }

            }
            if (await BoolAgent(hash_account))//需要代理
            {
                var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();//找到要代理的人
                var bossCompany = await _context.Employees
                           .Where(db => db.HashAccount == bossHash)
                           .Select(db => db.CompanyHash).FirstOrDefaultAsync();//找到要代理的人

                var boss_permissions = await (from t in _context.ManagerAccounts
                                              join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                              where t.HashAccount.Equals(bossHash)
                                              select new
                                              {
                                                  EmployeeDisplay = a.EmployeeDisplay
                                              }).ToListAsync();

                var boss_display_id = boss_permissions[0].EmployeeDisplay;

                switch (boss_display_id)
                {
                    case 1:
                        List<WorkRecord> workRecords = await GetWorkRecord(bossCompany);
                        workRecord = workRecords;
                        break;
                    case 2:
                        List<WorkRecord> workRecords2 = await GetWorkReccord2(bossHash);
                        foreach (var work in workRecords2)
                        {
                            if (workRecord.FindIndex(item => item.WorkRecordId == work.WorkRecordId) == -1)
                            {
                                WorkRecord search = new WorkRecord()
                                {
                                    HashAccount = work.HashAccount,
                                    Num = work.Num,
                                    Name = work.Name,
                                    WorkTime = work.WorkTime,
                                    RestTime = work.RestTime,
                                    WorkRecordId = work.WorkRecordId
                                };
                                workRecord.Add(search);
                            }
                        }
                        break;
                    case 3:
                        List<WorkRecord> workRecords3 = await GetWorkRecord3(bossHash);
                        foreach (var work in workRecords3)
                        {
                            if (workRecord.FindIndex(item => item.WorkRecordId == work.WorkRecordId) == -1)
                            {
                                WorkRecord search = new WorkRecord()
                                {
                                    HashAccount = work.HashAccount,
                                    Num = work.Num,
                                    Name = work.Name,
                                    WorkTime = work.WorkTime,
                                    RestTime = work.RestTime,
                                    WorkRecordId = work.WorkRecordId
                                };
                                workRecord.Add(search);
                            }
                        }
                        break;
                }

            }
            return workRecord;
        }

        [HttpGet("Review_TripRecord/{hash_company}")]//取得未審核公差資料
        public async Task<IEnumerable> Review_TripRecord(string hash_company)
        {
            var review_triprecord = await (from t in _context.EmployeeTripRecords
                                         join a in _context.Employees on t.HashAccount equals a.HashAccount
                                         join b in _context.Companies on a.CompanyHash equals b.CompanyHash
                                         join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                         where a.CompanyHash == hash_company && t.Review == null
                                         orderby t.CreatedTime
                                         select new
                                         {
                                             TripRecordId = t.TripRecordsId,
                                             HashAccount = t.HashAccount,
                                             Name = c.Name,
                                             StartDate = t.StartDate,
                                             EndDate = t.EndDate,
                                             Location = t.Location,
                                             Reason = t.Reason,
                                             Review = t.Review,
                                             CreatedTime = t.CreatedTime, 
                                         }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(review_triprecord);
            return jsonData;
        }
        [HttpGet("Pass_TripRecord/{hash_company}")]//取得已審核公差資料
        public async Task<IEnumerable> Pass_TripRecord(string hash_company)
        {
            var pass_triprecord = await (from t in _context.EmployeeTripRecords
                                           join a in _context.Employees on t.HashAccount equals a.HashAccount
                                           join b in _context.Companies on a.CompanyHash equals b.CompanyHash
                                           join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                           where a.CompanyHash == hash_company && t.Review != null
                                           orderby t.CreatedTime
                                           select new
                                           {
                                               TripRecordId = t.TripRecordsId,
                                               HashAccount = t.HashAccount,
                                               Name = c.Name,
                                               StartDate = t.StartDate,
                                               EndDate = t.EndDate,
                                               Location = t.Location,
                                               Reason = t.Reason,
                                               Review = t.Review,
                                               CreatedTime = t.CreatedTime,
                                           }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(pass_triprecord);
            return jsonData;
        }

        [HttpGet("Detail_Trip2Record/{hash_company}")]//取得詳細公差紀錄
        public async Task<IEnumerable> Detail_Trip2Record(string hash_company)
        {
            var detail_trip2Record = await (from t in _context.EmployeeTrip2Records
                                            join a in _context.Employees on t.HashAccount equals a.HashAccount
                                            join b in _context.EmployeeInformations on t.HashAccount equals b.HashAccount
                                            where a.CompanyHash.Equals(hash_company)
                                            select new
                                            {
                                                Trip2RecordId = t.Trip2RecordsId,
                                                GroupId = t.GroupId,
                                                HashAccount = t.HashAccount,
                                                Name = b.Name,
                                                Trip2Type = t.Trip2TypeId,
                                                CoordinateX = t.CoordinateX,
                                                CoordinateY = t.CoordinateY,
                                                Address = t.Address,
                                                CreatedTime = t.CreatedTime,
                                            }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(detail_trip2Record);
            return jsonData;
        }

        public partial class Trip2Record
        {
            public int GroupId { get; set; }
            public int Num { get; set; }
            public string Name { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }//公差紀錄Model

        [HttpGet("Get_Trip2Record/{hash_company}")]//取得公差紀錄
        public async Task<List<Trip2Record>> Get_Trip2Record(string hash_company)
        {
            var get_trip2Record = await (from t in _context.EmployeeTrip2Records
                                            join a in _context.Employees on t.HashAccount equals a.HashAccount
                                            join b in _context.EmployeeInformations on t.HashAccount equals b.HashAccount
                                            where a.CompanyHash == hash_company && t.Trip2TypeId != 2
                                            orderby t.CreatedTime descending
                                            select new
                                            {
                                                GroupId = t.GroupId,
                                                Name = b.Name,
                                                Trip2TypeId = t.Trip2TypeId,
                                                CreatedTime = t.CreatedTime,
                                            }).ToListAsync();

            List<Trip2Record> trip2Record = new List<Trip2Record>();

            //計算JSON總長度
            int length = 0;
            foreach (var ever in get_trip2Record)
            {
                length++;
            }

            //recorded接已完成登入的公差紀錄
            List<int> recorded = new List<int>();
            int num = 0; //編號

            for (int i = 0; i < length - 1; i++) //i=JSON裡的每筆公差紀錄
            {
                for(int n = 0; n < recorded.Count ; n++) //pass=已完成登入的紀錄
                {
                    if (get_trip2Record[i].GroupId == recorded[n])//如果i筆記錄已完成登入，則換查看下一筆
                    {
                        i++;
                        n = -1;
                    }
                }

                for (int j = 1; j <= length - 1; j++)//從第1筆紀錄開始找同樣的的Group_id
                {
                    if (get_trip2Record[i].GroupId == get_trip2Record[j].GroupId && j != i)//如果第j筆資料的Group_id與i筆相同，且與i為不同筆則登入至後台
                    {
                        num++;//編號
                        recorded.Add(get_trip2Record[i].GroupId);//完成登入的Group_id記錄至recorded

                        for (int k = 1; k <= recorded.Count - 1; k++)
                        {//執行的回數
                            for (int m = 1; m <= recorded.Count - k; m++)//執行的次數
                            {
                                if (recorded[m] < recorded[m - 1])
                                {
                                    //二數交換
                                    int temp = recorded[m];
                                    recorded[m] = recorded[m - 1];
                                    recorded[m - 1] = temp;
                                }
                            }
                        }
                        trip2Record.Add(new Trip2Record
                        {
                            GroupId = get_trip2Record[i].GroupId,
                            Num = num,
                            Name = get_trip2Record[j].Name,
                            StartTime = get_trip2Record[j].CreatedTime,//開始時間
                            EndTime = get_trip2Record[i].CreatedTime//結束時間
                        });

                        break;
                    }
                }

            }

            return trip2Record;
        }

        [HttpGet("Get_Trip2Record2/{hash_account}")]//取得公差紀錄
        public async Task<List<Trip2Record>> Get_Trip2Record2(string hash_account)
        {
            var get_trip2Record = await (from t in _context.EmployeeTrip2Records
                                         join a in _context.Employees on t.HashAccount equals a.HashAccount
                                         join b in _context.EmployeeInformations on t.HashAccount equals b.HashAccount
                                         where a.ManagerHash.Equals(hash_account) && t.Trip2TypeId != 2
                                         orderby t.CreatedTime descending
                                         select new
                                         {
                                             GroupId = t.GroupId,
                                             Name = b.Name,
                                             Trip2TypeId = t.Trip2TypeId,
                                             CreatedTime = t.CreatedTime,
                                         }).ToListAsync();

            List<Trip2Record> trip2Record = new List<Trip2Record>();

            //計算JSON總長度
            int length = 0;
            foreach (var ever in get_trip2Record)
            {
                length++;
            }

            //recorded接已完成登入的公差紀錄
            List<int> recorded = new List<int>();
            int num = 0; //編號

            for (int i = 0; i < length - 1; i++) //i=JSON裡的每筆公差紀錄
            {
                for (int n = 0; n < recorded.Count; n++) //pass=已完成登入的紀錄
                {
                    if (get_trip2Record[i].GroupId == recorded[n])//如果i筆記錄已完成登入，則換查看下一筆
                    {
                        i++;
                        n = -1;
                    }
                }

                for (int j = 1; j <= length - 1; j++)//從第1筆紀錄開始找同樣的的Group_id
                {
                    if (get_trip2Record[i].GroupId == get_trip2Record[j].GroupId && j != i)//如果第j筆資料的Group_id與i筆相同，且與i為不同筆則登入至後台
                    {
                        num++;//編號
                        recorded.Add(get_trip2Record[i].GroupId);//完成登入的Group_id記錄至recorded

                        for (int k = 1; k <= recorded.Count - 1; k++)
                        {//執行的回數
                            for (int m = 1; m <= recorded.Count - k; m++)//執行的次數
                            {
                                if (recorded[m] < recorded[m - 1])
                                {
                                    //二數交換
                                    int temp = recorded[m];
                                    recorded[m] = recorded[m - 1];
                                    recorded[m - 1] = temp;
                                }
                            }
                        }
                        trip2Record.Add(new Trip2Record
                        {
                            GroupId = get_trip2Record[i].GroupId,
                            Num = num,
                            Name = get_trip2Record[j].Name,
                            StartTime = get_trip2Record[j].CreatedTime,//開始時間
                            EndTime = get_trip2Record[i].CreatedTime//結束時間
                        });

                        break;
                    }
                }

            }
            if (await BoolAgent(hash_account))//需要代理
            {
                var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();//找到要代理的人
                var bossCompany = await _context.Employees
                           .Where(db => db.HashAccount == bossHash)
                           .Select(db => db.CompanyHash).FirstOrDefaultAsync();//找到要代理的人

                var boss_permissions = await (from t in _context.ManagerAccounts
                                              join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                              where t.HashAccount.Equals(bossHash)
                                              select new
                                              {
                                                  EmployeeDisplay = a.EmployeeDisplay
                                              }).ToListAsync();

                var boss_display_id = boss_permissions[0].EmployeeDisplay;

                switch (boss_display_id)
                {
                    case 1:
                        List<Trip2Record> trip2Records = await Get_Trip2Record(bossCompany);
                        trip2Record = trip2Records;
                        break;
                    case 2:
                        List<Trip2Record> trip2Records2 = await Get_Trip2Record2(bossHash);
                        foreach (var trip in trip2Records2)
                        {
                            if (trip2Record.FindIndex(item => item.GroupId == trip.GroupId) == -1)
                            {
                                Trip2Record search = new Trip2Record()
                                {
                                    GroupId = trip.GroupId,
                                    Num = trip.Num,
                                    Name = trip.Name,
                                    StartTime = trip.StartTime,//開始時間
                                    EndTime = trip.EndTime//結束時間
                                };
                                trip2Record.Add(search);
                            }
                        }
                        break;
                    case 3:
                        List<Trip2Record> trip2Records3 = await Get_Trip2Record3(bossHash);
                        foreach (var trip in trip2Records3)
                        {
                            if (trip2Record.FindIndex(item => item.GroupId == trip.GroupId) == -1)
                            {
                                Trip2Record search = new Trip2Record()
                                {
                                    GroupId = trip.GroupId,
                                    Num = trip.Num,
                                    Name = trip.Name,
                                    StartTime = trip.StartTime,//開始時間
                                    EndTime = trip.EndTime//結束時間
                                };
                                trip2Record.Add(search);
                            }
                        }
                        break;
                }

            }
            return trip2Record;
        }

        public class Trip2Record3 
        {
            public int GroupId { get; set; }
            public string Name { get; set; }
            public int Trip2TypeId { get; set; }
            public DateTime CreatedTime { get; set; }
        }
        [HttpGet("Get_Trip2Record3/{hash_account}")]//取得已審核員工資料
        public async Task<List<Trip2Record>> Get_Trip2Record3(string hash_account)
        {
            var permissions_id = await _context.ManagerAccounts
                            .Where(db => db.HashAccount == hash_account)
                            .Select(db => db.PermissionsId).FirstOrDefaultAsync();

            var customeizationDisplayId = await _context.ManagerPermissions
                            .Where(db => db.PermissionsId == permissions_id)
                            .Select(db => db.CustomizationDisplay).FirstOrDefaultAsync();

            var customizationsDisplay = await (from t in _context.ManagerPermissionsCustomizations
                                               where t.PermissionsId == customeizationDisplayId
                                               select new
                                               {
                                                   DepartmentId = t.DepartmentId,
                                                   JobtitleId = t.JobtitleId
                                               }).ToListAsync();
            List<Trip2Record3> get_trip2Record = new List<Trip2Record3>();
            foreach (var display in customizationsDisplay)
            {
                var trip2Records = await (from t in _context.EmployeeTrip2Records
                                             join a in _context.Employees on t.HashAccount equals a.HashAccount
                                             join b in _context.EmployeeInformations on t.HashAccount equals b.HashAccount
                                             where b.DepartmentId==display.DepartmentId && b.JobtitleId==display.JobtitleId && t.Trip2TypeId != 2
                                             orderby t.CreatedTime descending
                                             select new
                                             {
                                                 GroupId = t.GroupId,
                                                 Name = b.Name,
                                                 Trip2TypeId = t.Trip2TypeId,
                                                 CreatedTime = t.CreatedTime,
                                             }).ToListAsync();

                string json = JsonConvert.SerializeObject(trip2Records);
                List<Trip2Record3> trip2Records1 = JsonConvert.DeserializeObject<List<Trip2Record3>>(json);
                foreach (var trip in trip2Records1)
                {
                    Trip2Record3 search = new Trip2Record3()
                    {
                        GroupId = trip.GroupId,
                        Name = trip.Name,
                        Trip2TypeId = trip.Trip2TypeId,
                        CreatedTime = trip.CreatedTime
                    };
                    get_trip2Record.Add(search);
                }
            }

            List<Trip2Record> trip2Record = new List<Trip2Record>();

            //計算JSON總長度
            int length = 0;
            foreach (var ever in get_trip2Record)
            {
                length++;
            }

            //recorded接已完成登入的公差紀錄
            List<int> recorded = new List<int>();
            int num = 0; //編號

            for (int i = 0; i < length - 1; i++) //i=JSON裡的每筆公差紀錄
            {
                for (int n = 0; n < recorded.Count; n++) //pass=已完成登入的紀錄
                {
                    if (get_trip2Record[i].GroupId == recorded[n])//如果i筆記錄已完成登入，則換查看下一筆
                    {
                        i++;
                        n = -1;
                    }
                }

                for (int j = 1; j <= length - 1; j++)//從第1筆紀錄開始找同樣的的Group_id
                {
                    if (get_trip2Record[i].GroupId == get_trip2Record[j].GroupId && j != i)//如果第j筆資料的Group_id與i筆相同，且與i為不同筆則登入至後台
                    {
                        num++;//編號
                        recorded.Add(get_trip2Record[i].GroupId);//完成登入的Group_id記錄至recorded

                        for (int k = 1; k <= recorded.Count - 1; k++)
                        {//執行的回數
                            for (int m = 1; m <= recorded.Count - k; m++)//執行的次數
                            {
                                if (recorded[m] < recorded[m - 1])
                                {
                                    //二數交換
                                    int temp = recorded[m];
                                    recorded[m] = recorded[m - 1];
                                    recorded[m - 1] = temp;
                                }
                            }
                        }
                        trip2Record.Add(new Trip2Record
                        {
                            GroupId = get_trip2Record[i].GroupId,
                            Num = num,
                            Name = get_trip2Record[j].Name,
                            StartTime = get_trip2Record[j].CreatedTime,//開始時間
                            EndTime = get_trip2Record[i].CreatedTime//結束時間
                        });

                        break;
                    }
                }

            }
            if (await BoolAgent(hash_account))//需要代理
            {
                var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();//找到要代理的人
                var bossCompany = await _context.Employees
                           .Where(db => db.HashAccount == bossHash)
                           .Select(db => db.CompanyHash).FirstOrDefaultAsync();//找到要代理的人

                var boss_permissions = await (from t in _context.ManagerAccounts
                                              join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                              where t.HashAccount.Equals(bossHash)
                                              select new
                                              {
                                                  EmployeeDisplay = a.EmployeeDisplay
                                              }).ToListAsync();

                var boss_display_id = boss_permissions[0].EmployeeDisplay;

                switch (boss_display_id)
                {
                    case 1:
                        List<Trip2Record> trip2Records = await Get_Trip2Record(bossCompany);
                        trip2Record = trip2Records;
                        break;
                    case 2:
                        List<Trip2Record> trip2Records2 = await Get_Trip2Record2(bossHash);
                        foreach (var trip in trip2Records2)
                        {
                            if (trip2Record.FindIndex(item => item.GroupId == trip.GroupId) == -1)
                            {
                                Trip2Record search = new Trip2Record()
                                {
                                    GroupId = trip.GroupId,
                                    Num = trip.Num,
                                    Name = trip.Name,
                                    StartTime = trip.StartTime,//開始時間
                                    EndTime = trip.EndTime//結束時間
                                };
                                trip2Record.Add(search);
                            }
                        }
                        break;
                    case 3:
                        List<Trip2Record> trip2Records3 = await Get_Trip2Record3(bossHash);
                        foreach (var trip in trip2Records3)
                        {
                            if (trip2Record.FindIndex(item => item.GroupId == trip.GroupId) == -1)
                            {
                                Trip2Record search = new Trip2Record()
                                {
                                    GroupId = trip.GroupId,
                                    Num = trip.Num,
                                    Name = trip.Name,
                                    StartTime = trip.StartTime,//開始時間
                                    EndTime = trip.EndTime//結束時間
                                };
                                trip2Record.Add(search);
                            }
                        }
                        break;
                }

            }
            return trip2Record;

        }

        [HttpGet("Review_LeaveRecord/{hash_company}")]//取得未審核請假資料
        public async Task<IEnumerable> Review_LeaveRecord(string hash_company)
        {
            var review_leaverecord = await (from t in _context.EmployeeLeaveRecords
                                           join a in _context.Employees on t.HashAccount equals a.HashAccount
                                           join b in _context.EmployeeLeaveTypes on t.LeaveTypeId equals b.LeaveTypeId
                                           join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                           where a.CompanyHash == hash_company && t.Review == null
                                           orderby t. CreatedTime
                                           select new
                                           {
                                               LeaveRecordId =t.LeaveRecordsId,
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
            return jsonData;
        }
        [HttpGet("Pass_LeaveRecord/{hash_company}")]//取得已審核請假資料
        public async Task<List<LeaveRecord>> Pass_LeaveRecord(string hash_company)
        {
            var pass_leaverecord = await (from t in _context.EmployeeLeaveRecords
                                           join a in _context.Employees on t.HashAccount equals a.HashAccount
                                           join b in _context.EmployeeLeaveTypes on t.LeaveTypeId equals b.LeaveTypeId
                                           join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                           where a.CompanyHash == hash_company && t.Review != null
                                           orderby t.StartDate descending
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

            string jsonData = JsonConvert.SerializeObject(pass_leaverecord);
            List<LeaveRecord> leaveRecords = JsonConvert.DeserializeObject<List<LeaveRecord>>(jsonData);
            return leaveRecords;
        }

        [HttpGet("Pass_LeaveRecord2/{hash_account}")]//取得已審核請假資料
        public async Task<List<LeaveRecord>> Pass_LeaveRecord2(string hash_account)
        {
            var pass_leaverecord = await (from t in _context.EmployeeLeaveRecords
                                          join a in _context.Employees on t.HashAccount equals a.HashAccount
                                          join b in _context.EmployeeLeaveTypes on t.LeaveTypeId equals b.LeaveTypeId
                                          join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                          where a.ManagerHash.Equals(hash_account) && t.Review != null
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

            string jsonData = JsonConvert.SerializeObject(pass_leaverecord);
            List<LeaveRecord> leaveRecords = JsonConvert.DeserializeObject<List<LeaveRecord>>(jsonData);
            if (await BoolAgent(hash_account))//需要代理
            {
                var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();//找到要代理的人
                var bossCompany = await _context.Employees
                           .Where(db => db.HashAccount == bossHash)
                           .Select(db => db.CompanyHash).FirstOrDefaultAsync();//找到要代理的人

                var boss_permissions = await (from t in _context.ManagerAccounts
                                              join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                              where t.HashAccount.Equals(bossHash)
                                              select new
                                              {
                                                  EmployeeDisplay = a.EmployeeDisplay
                                              }).ToListAsync();

                var boss_display_id = boss_permissions[0].EmployeeDisplay;

                switch (boss_display_id)
                {
                    case 1:
                        List<LeaveRecord> leaveRecords1 = await Pass_LeaveRecord(bossCompany);
                        leaveRecords = leaveRecords1;
                        break;
                    case 2:
                        List<LeaveRecord> leaveRecords2 = await Pass_LeaveRecord2(bossHash);
                        foreach (var leaveRecord in leaveRecords2)
                        {
                            if (leaveRecords.FindIndex(item => item.LeaveRecordId == leaveRecord.LeaveRecordId) == -1)
                            {
                                LeaveRecord search = new LeaveRecord()
                                {
                                    LeaveRecordId = leaveRecord.LeaveRecordId,//請假編號
                                    HashAccount = leaveRecord.HashAccount,//員工編號
                                    Name = leaveRecord.Name,//員工姓名
                                    LeaveType = leaveRecord.LeaveType,//假別
                                    StartDate = leaveRecord.StartDate,//開始時間
                                    EndDate = leaveRecord.EndDate,//結束時間
                                    Reason = leaveRecord.Reason,//備註(事由)
                                    Review = leaveRecord.Review,//審核狀態
                                    CreatedTime = leaveRecord.CreatedTime//申請時間
                                };
                                leaveRecords.Add(search);
                            }
                        }
                        break;
                    case 3:
                        List<LeaveRecord> leaveRecords3 = await Pass_LeaveRecord3(bossHash);
                        foreach (var leaveRecord in leaveRecords3)
                        {
                            if (leaveRecords.FindIndex(item => item.LeaveRecordId == leaveRecord.LeaveRecordId) == -1)
                            {
                                LeaveRecord search = new LeaveRecord()
                                {
                                    LeaveRecordId = leaveRecord.LeaveRecordId,//請假編號
                                    HashAccount = leaveRecord.HashAccount,//員工編號
                                    Name = leaveRecord.Name,//員工姓名
                                    LeaveType = leaveRecord.LeaveType,//假別
                                    StartDate = leaveRecord.StartDate,//開始時間
                                    EndDate = leaveRecord.EndDate,//結束時間
                                    Reason = leaveRecord.Reason,//備註(事由)
                                    Review = leaveRecord.Review,//審核狀態
                                    CreatedTime = leaveRecord.CreatedTime//申請時間
                                };
                                leaveRecords.Add(search);
                            }
                        }
                        break;
                }

            }
            return leaveRecords;
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

        [HttpGet("Pass_LeaveRecord3/{hash_account}")]//取得已審核請假資料
        public async Task<List<LeaveRecord>> Pass_LeaveRecord3(string hash_account)
        {
            var permissions_id = await _context.ManagerAccounts
                            .Where(db => db.HashAccount == hash_account)
                            .Select(db => db.PermissionsId).FirstOrDefaultAsync();

            var customeizationDisplayId = await _context.ManagerPermissions
                            .Where(db => db.PermissionsId == permissions_id)
                            .Select(db => db.CustomizationDisplay).FirstOrDefaultAsync();

            var customizationsDisplay = await (from t in _context.ManagerPermissionsCustomizations
                                               where t.PermissionsId == customeizationDisplayId
                                               select new
                                               {
                                                   DepartmentId = t.DepartmentId,
                                                   JobtitleId = t.JobtitleId
                                               }).ToListAsync();
            List<LeaveRecord> leaveRecords = new List<LeaveRecord>();
            foreach (var display in customizationsDisplay)
            {
                var pass_leaverecord = await (from t in _context.EmployeeLeaveRecords
                                              join a in _context.Employees on t.HashAccount equals a.HashAccount
                                              join b in _context.EmployeeLeaveTypes on t.LeaveTypeId equals b.LeaveTypeId
                                              join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                              where c.DepartmentId==display.DepartmentId && c.JobtitleId==display.JobtitleId && t.Review != null
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

                string jsonData = JsonConvert.SerializeObject(pass_leaverecord);
                List<LeaveRecord> leaveRecords1 = JsonConvert.DeserializeObject<List<LeaveRecord>>(jsonData);
                foreach (var leaveRecord in leaveRecords1)
                {
                    LeaveRecord search = new LeaveRecord()
                    {
                        LeaveRecordId = leaveRecord.LeaveRecordId,//請假編號
                        HashAccount = leaveRecord.HashAccount,//員工編號
                        Name = leaveRecord.Name,//員工姓名
                        LeaveType = leaveRecord.LeaveType,//假別
                        StartDate = leaveRecord.StartDate,//開始時間
                        EndDate = leaveRecord.EndDate,//結束時間
                        Reason = leaveRecord.Reason,//備註(事由)
                        Review = leaveRecord.Review,//審核狀態
                        CreatedTime = leaveRecord.CreatedTime//申請時間
                    };
                    leaveRecords.Add(search);
                }
            }

            if (await BoolAgent(hash_account))//需要代理
            {
                var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();//找到要代理的人
                var bossCompany = await _context.Employees
                           .Where(db => db.HashAccount == bossHash)
                           .Select(db => db.CompanyHash).FirstOrDefaultAsync();//找到要代理的人

                var boss_permissions = await( from t in _context.ManagerAccounts
                                       join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                       where t.HashAccount.Equals(bossHash) 
                                       select new 
                                       {
                                           EmployeeDisplay = a.EmployeeDisplay
                                       }).ToListAsync();

                var boss_display_id = boss_permissions[0].EmployeeDisplay;

                switch (boss_display_id) 
                {
                    case 1:
                        List<LeaveRecord> leaveRecords1 = await Pass_LeaveRecord(bossCompany);
                        leaveRecords = leaveRecords1;
                        break;
                    case 2:
                        List<LeaveRecord> leaveRecords2 = await Pass_LeaveRecord2(bossHash);
                        foreach (var leaveRecord in leaveRecords2)
                        {
                            if (leaveRecords.FindIndex(item=>item.LeaveRecordId==leaveRecord.LeaveRecordId)==-1) 
                            {
                                LeaveRecord search = new LeaveRecord()
                                {
                                    LeaveRecordId = leaveRecord.LeaveRecordId,//請假編號
                                    HashAccount = leaveRecord.HashAccount,//員工編號
                                    Name = leaveRecord.Name,//員工姓名
                                    LeaveType = leaveRecord.LeaveType,//假別
                                    StartDate = leaveRecord.StartDate,//開始時間
                                    EndDate = leaveRecord.EndDate,//結束時間
                                    Reason = leaveRecord.Reason,//備註(事由)
                                    Review = leaveRecord.Review,//審核狀態
                                    CreatedTime = leaveRecord.CreatedTime//申請時間
                                };
                                leaveRecords.Add(search);
                            }
                        }
                        break;
                    case 3:
                        List<LeaveRecord> leaveRecords3 = await Pass_LeaveRecord3(bossHash);
                        foreach (var leaveRecord in leaveRecords3)
                        {
                            if (leaveRecords.FindIndex(item => item.LeaveRecordId == leaveRecord.LeaveRecordId) == -1)
                            {
                                LeaveRecord search = new LeaveRecord()
                                {
                                    LeaveRecordId = leaveRecord.LeaveRecordId,//請假編號
                                    HashAccount = leaveRecord.HashAccount,//員工編號
                                    Name = leaveRecord.Name,//員工姓名
                                    LeaveType = leaveRecord.LeaveType,//假別
                                    StartDate = leaveRecord.StartDate,//開始時間
                                    EndDate = leaveRecord.EndDate,//結束時間
                                    Reason = leaveRecord.Reason,//備註(事由)
                                    Review = leaveRecord.Review,//審核狀態
                                    CreatedTime = leaveRecord.CreatedTime//申請時間
                                };
                                leaveRecords.Add(search);
                            }
                        }
                        break;
                }

            }

            return leaveRecords;
        }
        public class BossSettingPermissions 
        {
            public bool SettingWorktime { get; set; }
            public bool SettingDepartmentJobtitle { get; set; }
            public bool SettingLocation { get; set; }
        }

        [HttpGet]
        public async Task<bool> BoolAgent(string hash_account) //判斷是否需要代理
        {
            bool result = false;
            var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();//找到要代理的人
            if (bossHash == null)
            {
                return false;
            }

            var pass_leaverecord = from t in _context.EmployeeLeaveRecords
                                   where t.HashAccount.Equals(bossHash) && t.Review == true && t.StartDate <= DateTime.Now && t.EndDate >= DateTime.Now
                                   orderby t.CreatedTime
                                   select t;//找到要代理的人現在是否請假

            result = pass_leaverecord.Count() != 0 ? true : false; 

            if (result == false)//沒請假
            {
                var trip2Records = await _context.EmployeeTrip2Records
                            .Where(db => db.HashAccount == bossHash)
                            .OrderByDescending(db => db.CreatedTime)
                            .Select(db => db.Trip2TypeId).FirstOrDefaultAsync();//現在是否在公差

                result = trip2Records == 1 || trip2Records == 2 ? true : false;

                if (result == false)//沒公差
                {
                    var Enabled = await _context.ManagerAccounts
                            .Where(db => db.HashAccount == bossHash)
                            .Select(db => db.Enabled).FirstOrDefaultAsync();//管理員權限是否被停用

                    result = Enabled == false ? true : false;
                    if (result == false)//沒停用
                    {
                        return false;
                    }
                }
            }

            return result;
        }

        [HttpGet("GetbossSettingPermissions/{hash_account}")]//取得職務代理人的setting權限
        public async Task<IEnumerable> GetAgentSettingPermissions(string hash_account)
        {
            bool result = false;
            var bossHash = await _context.ManagerAccounts
                            .Where(db => db.HashAgent == hash_account)
                            .Select(db => db.HashAccount).FirstOrDefaultAsync();
            if (bossHash == null)
            {
                List<BossSettingPermissions> bossPermission = new List<BossSettingPermissions>();
                BossSettingPermissions bossSetting = new BossSettingPermissions()
                {
                    SettingWorktime = false,
                    SettingDepartmentJobtitle = false,
                    SettingLocation = false
                };
                bossPermission.Add(bossSetting);
                return bossPermission;
            }

            var boss_permissions = await (from t in _context.ManagerAccounts
                                          join a in _context.ManagerPermissions on t.PermissionsId equals a.PermissionsId
                                          where t.HashAccount.Equals(bossHash)
                                          select new
                                          {
                                              SettingWorktime = a.SettingWorktime,
                                              SettingDepartmentJobtitle = a.SettingDepartmentJobtitle,
                                              SettingLocation = a.SettingLocation
                                          }).ToListAsync();

            string json = JsonConvert.SerializeObject(boss_permissions);
            List<BossSettingPermissions> bossPermissions = JsonConvert.DeserializeObject<List<BossSettingPermissions>>(json);

            if (boss_permissions.Count == 0)
            {
                List<BossSettingPermissions> bossPermission = new List<BossSettingPermissions>();
                BossSettingPermissions bossSetting = new BossSettingPermissions()
                {
                    SettingWorktime = true,
                    SettingDepartmentJobtitle = true,
                    SettingLocation = true
                };
                bossPermission.Add(bossSetting);
                bossPermissions = bossPermission;
            }


            var pass_leaverecord = from t in _context.EmployeeLeaveRecords
                                   where t.HashAccount.Equals(bossHash) && t.Review == true && t.StartDate <= DateTime.Now && t.EndDate >= DateTime.Now
                                   orderby t.CreatedTime
                                   select t;

            result = pass_leaverecord.Count() != 0 ? true : false;

            if (result == false)
            {
                var trip2Records = await _context.EmployeeTrip2Records
                            .Where(db => db.HashAccount == bossHash)
                            .OrderByDescending(db => db.CreatedTime)
                            .Select(db => db.Trip2TypeId).FirstOrDefaultAsync();

                result = trip2Records == 1 || trip2Records == 2 ? true : false;

                if (result == false)
                {
                    var Enabled = await _context.ManagerAccounts
                            .Where(db => db.HashAccount == bossHash)
                            .Select(db => db.Enabled).FirstOrDefaultAsync();

                    result = Enabled == false ? true : false;
                    if (result == false)
                    {
                        List<BossSettingPermissions> bossPermission = new List<BossSettingPermissions>();
                        BossSettingPermissions bossSetting = new BossSettingPermissions()
                        {
                            SettingWorktime = false,
                            SettingDepartmentJobtitle = false,
                            SettingLocation = false
                        };
                        bossPermission.Add(bossSetting);
                        return bossPermission;
                    }
                }
            }

            return bossPermissions;
        }
        

        private bool CompanyExists(string id)
        {
            return _context.Companies.Any(e => e.CompanyHash == id);
        }
    }
}
