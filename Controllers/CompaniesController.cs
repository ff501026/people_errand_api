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

        //// GET: api/Companies
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        //{
        //    return await _context.Companies.ToListAsync();
        //}

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

            return company_hash+"\n"+coordinate_X+"\n"+coordinate_Y;
        }

        // PUT: api/Companies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(string id, Company company)
        {
            if (id != company.CompanyHash)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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

      

        // POST: api/Companies/regist_company
        [HttpPost("regist_company")]
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


        //[HttpGet("GetEmployee_Record/{hash_company}")]
        //public async Task<IEnumerable> GetEmployee_Reccord(string hash_company)
        //{
        //    var Employee_Record = await (from t in _context.Employees
        //                                 join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
        //                                 join b in _context.EmployeeWorkRecords on t.HashAccount equals b.HashAccount
        //                                 where t.CompanyHash == hash_company && b.Enabled==true
        //                                 select new
        //                                 {
        //                                     員工姓名 = a.Name,
        //                                     紀錄時間 = b.CreatedTime,
        //                                     X_coordinate = b.CoordinateX,
        //                                     Y_coordinate = b.CoordinateY,
        //                                     Work_type = b.WorkTypeId,
        //                                 }).ToListAsync();

        //    return Employee_Record;
        //}


        public partial class WorkRecord
        {
            public int Num { get; set; }
            public string Name { get; set; }
            public DateTime WorkTime { get; set; }
            public DateTime RestTime { get; set; }
        }

        [HttpGet("GetWorkRecord/{hash_company}")]
        public async Task<IEnumerable> GetWorkReccord(string hash_company)
        {
            var Employee_Record = await (from t in _context.Employees
                                         join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                         join b in _context.EmployeeWorkRecords on t.HashAccount equals b.HashAccount
                                         where t.CompanyHash == hash_company && b.Enabled == true
                                         select new
                                         {
                                             員工姓名 = a.Name,
                                             紀錄時間 = b.CreatedTime,
                                             X_coordinate = b.CoordinateX,
                                             Y_coordinate = b.CoordinateY,
                                             Work_type = b.WorkTypeId,
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
                var name = Employee_Record[i].員工姓名; //取得i筆上班紀錄的員工姓名

                for (int j = 1; j <= length-1; j++)//從第1筆紀錄開始找i筆上班紀錄的下班紀錄
                {
                    foreach (int pass in recorded)//pass=已完成登入的紀錄
                    {
                        if (j == pass)
                        {
                            j++;//如果i筆記錄已完成登入，則換查看下一筆
                        }
                    }
                    if (name == Employee_Record[j].員工姓名 && j != i)//如果第j筆資料名字與i筆資料相同，且與i為不同筆則登入至後台
                    {
                        num++;//編號
                        var worktime = Employee_Record[i].紀錄時間;//上班時間
                        var resttime = Employee_Record[j].紀錄時間;//下班時間
                        recorded.Add(i);//完成登入的該筆記錄至recorded
                        recorded.Add(j);//完成登入的該筆記錄至recorded


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

        private bool CompanyExists(string id)
        {
            return _context.Companies.Any(e => e.CompanyHash == id);
        }
    }
}
