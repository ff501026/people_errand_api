﻿using System;
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
    public class EmployeeDepartmentTypesController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeDepartmentTypesController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeDepartmentTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDepartmentType>>> GetEmployeeDepartmentTypes()
        {
            return await _context.EmployeeDepartmentTypes.ToListAsync();
        }

        // GET: api/EmployeeDepartmentTypes/5
        [HttpGet("{company_hash}")]
        public async Task<IEnumerable> Get_Department(string company_hash)
        {
            var department = await (from t in _context.EmployeeDepartmentTypes
                                 where t.CompanyHash.Equals(company_hash)
                                 select new
                                 {
                                     DepartmentId = t.DepartmentId,
                                     Name = t.Name
                                 }).ToListAsync();

            string jsonData = JsonConvert.SerializeObject(department);
            return jsonData;
        }

        // PUT: api/EmployeeDepartmentTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateDepartment")]
        public ActionResult<bool> update_department([FromBody] List<EmployeeDepartmentType> employeeDepartmentTypes)
        {
            bool result = true;
            try
            {
                foreach (EmployeeDepartmentType employeeDepartmentType in employeeDepartmentTypes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@department_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeDepartmentType.DepartmentId
                        },
                        new SqlParameter("@department_name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeDepartmentType.Name
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec update_department @department_id,@department_name", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // POST: api/add_department
        [HttpPost("add_department")]
        public ActionResult<bool> add_department([FromBody]List<EmployeeDepartmentType> employeeDepartmentTypes)
        {
            bool result = true;
            try 
            {
                foreach (EmployeeDepartmentType employeeDepartmentType in employeeDepartmentTypes)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@department_name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeDepartmentType.Name
                        },
                        new SqlParameter("@company_hash",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeDepartmentType.CompanyHash
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec add_department @department_name,@company_hash", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                return result;
            }
            return result;
        }


        [HttpDelete("DeleteDepartment/{department_id}")]
        public async Task<bool> DeleteDepartment(int department_id)
        {
            bool result = true;
            try
            {
                var parameters = new[]
                {
                            new SqlParameter("@department_id",System.Data.SqlDbType.Int)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = department_id
                            }
                        };
                result = _context.Database.ExecuteSqlRaw("exec delete_department @department_id", parameters: parameters) != 0 ? true : false;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        private bool EmployeeDepartmentTypeExists(int id)
        {
            return _context.EmployeeDepartmentTypes.Any(e => e.DepartmentId == id);
        }
    }
}
