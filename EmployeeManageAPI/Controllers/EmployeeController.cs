using BCrypt.Net;
using EmployeeManageAPI.DataModels;
using EmployeeManageAPI.DtoModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public EmployeeController(IConfiguration configuration)
        {
            _context = new DataContext();
            _configuration = configuration;
        }
        [HttpPost]
        [Route("AddEmployee"),Authorize(Roles ="Admin")]
        public ActionResult AddEmployee(Employee emp)
        {
            try
            {
                emp.EmployeePassword = BCrypt.Net.BCrypt.HashPassword(emp.EmployeePassword);
                _context.Employees.Add(emp);
                _context.SaveChanges();
                return Ok("Employee Added Successfully");
            }catch(Exception ex){
                return BadRequest(ex.Message.ToString());
            }
        }
        [HttpPost]
        [Route("Login")]
        public ActionResult<Object> Login(LoginReq req)
        {
            try
            {
                Employee Result = _context.Employees.Where(x=>x.EmployeeName==req.Username).FirstOrDefault();
                if (Result == null)
                {
                    throw new Exception("Invalid Credentials");
                }
                if (!BCrypt.Net.BCrypt.Verify(req.Passsword, Result.EmployeePassword))
                {
                    throw new Exception("Invalid Credentials");
                }
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,Result.EmployeeName)
                };
                if (Result.IsAdmin)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                }
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(1),
                    signingCredentials: creds
                    );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                //if (Result.IsAdmin)
                //    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                //else
                //    claims.Add(new Claim(ClaimTypes.Role, "User"));
                //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
                //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                //var token = new JwtSecurityToken(
                //    claims: claims,
                //    expires: DateTime.Now.AddMinutes(1),
                //    signingCredentials: creds
                //    );
                //var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new
                {
                    Message = "Login Successful",
                    jwt = jwt
                });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        [HttpGet]
        [Route("GetAllEmployee")]
        public ActionResult<List<Employee>> GetAllEmployee() 
        {
            try
            {
                List<Employee> EmpList = _context.Employees.ToList<Employee>();
                return Ok(EmpList);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetEmployee/{Id}")]
        public ActionResult<Employee> GetEmployeeById(int Id) 
        {
            try
            {
                Employee result = _context.Employees.FirstOrDefault(x=>x.EmployeeId == Id);
                return Ok(result);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPut]
        [Route("UpdateEmployee"),Authorize(Roles= "Admin")]
        public ActionResult UpdateEmployee(Employee emp)
        {
            try
            {
                var Result = _context.Employees.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                Result.EmployeeName = emp.EmployeeName;
                Result.EmployeeEmail = emp.EmployeeEmail;
                Result.EmployeePassword = BCrypt.Net.BCrypt.HashPassword(emp.EmployeePassword);
                Result.IsAdmin = emp.IsAdmin;
                _context.SaveChanges();
                return Ok("Employee Added Successfully");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("DeleteEmplyee/{Id}"),Authorize(Roles="Admin")]
        public ActionResult DeleteEmployee(int Id)
        {
            try
            {
                var result =  _context.Employees.Where(x => x.EmployeeId == Id).FirstOrDefault();
                _context.Employees.Remove(result);
                _context.SaveChanges();
                return Ok("Employee Deleted Successfully");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
    }
}
