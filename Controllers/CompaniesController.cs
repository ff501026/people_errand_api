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
    public class CompaniesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public CompaniesController(people_errandContext context)
        {
            _context = context;
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

        // POST: api/Companies/regist_company
        [HttpPost("regist_company")]//註冊公司
        public ActionResult<bool> regist_company([FromBody] List<Company> companies)
        {
            bool result = true;
            try
            {
                foreach (Company company in companies)
                {
                        //設定放入查詢的值
                        var parameters = new[]
                    {
                        new SqlParameter("@company_name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = company.Name
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec regist_company @company_name", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }

            
            //輸出成功與否
            return result ;
        }

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

        [HttpGet("Pass_Employee/{hash_company}")]//取得已審核員工資料
        public async Task<IEnumerable> Pass_employee(string hash_company)
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
                                             Name = a.Name,
                                             Phone = a.Phone,
                                             Department = b.Name,
                                             Jobtitle = c.Name,
                                             Email = a.Email,
                                             PhoneCode = t.PhoneCode,
                                             Enabled = t.Enabled
                                         }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(pass_employee);
            return jsonData;
        }

        public partial class WorkRecord
        {
            public int Num { get; set; }
            public string Name { get; set; }
            public DateTime WorkTime { get; set; }
            public DateTime RestTime { get; set; }
        }//打卡紀錄Model

        [HttpGet("GetWorkRecord/{hash_company}")]//取得員工打卡紀錄
        public async Task<IEnumerable> GetWorkReccord(string hash_company)
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
                            Num = num,
                            Name = name,
                            WorkTime = worktime,
                            RestTime = resttime
                        });

                        break;
                    }
                }

            }

            string jsonData = JsonConvert.SerializeObject(workRecord);
            return jsonData;
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
                                            orderby t.GroupId
                                            select new
                                            {
                                                Trip2RecordId = t.Trip2RecordsId,
                                                GroupId = t.GroupId,
                                                HashAccount = t.HashAccount,
                                                Name = b.Name,
                                                Trip2Type = t.Trip2TypeId,
                                                CoordinateX = t.CoordinateX,
                                                CoordinateY = t.CoordinateY,
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
        public async Task<IEnumerable> Get_Trip2Record(string hash_company)
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

            string jsonData = JsonConvert.SerializeObject(trip2Record);
            return jsonData;
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
        public async Task<IEnumerable> Pass_LeaveRecord(string hash_company)
        {
            var pass_leaverecord = await (from t in _context.EmployeeLeaveRecords
                                           join a in _context.Employees on t.HashAccount equals a.HashAccount
                                           join b in _context.EmployeeLeaveTypes on t.LeaveTypeId equals b.LeaveTypeId
                                           join c in _context.EmployeeInformations on t.HashAccount equals c.HashAccount
                                           where a.CompanyHash == hash_company && t.Review != null
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
            return jsonData;
        }

        private bool CompanyExists(string id)
        {
            return _context.Companies.Any(e => e.CompanyHash == id);
        }
    }
}
