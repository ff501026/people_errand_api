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
    public class CompaniesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public CompaniesController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        // GET: api/Companies/company_hash
        [HttpGet("{company_hash}")]
        public async Task<ActionResult<Company>> GetCompany(string company_hash)
        {
            var company = await _context.Companies
                .Where(o => o.CompanyHash == company_hash)
                .Select(o => o).FirstOrDefaultAsync();
                       

            if (company == null)
            {
                return NotFound();
            }

            return company;
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
        public async Task<ActionResult<bool>> regist_company(string company_name)
        {
            var parameters = new[]
            {
                new SqlParameter("@company_name",System.Data.SqlDbType.NVarChar)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = company_name
                }
            };

            var result = _context.Database.ExecuteSqlRaw("exec regist_company @company_name", parameters: parameters);
            //輸出成功與否
            return result != 0 ? true : false;
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

        private bool CompanyExists(string id)
        {
            return _context.Companies.Any(e => e.CompanyHash == id);
        }
    }
}
