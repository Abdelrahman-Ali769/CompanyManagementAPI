
using CompanyManagementAPI.Data;
using CompanyManagementAPI.DTOs;
using CompanyManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public IActionResult GetAllEmployees(
        //    [FromQuery] int pageNumber = 1,
        //    [FromQuery] int pageSize = 10,
        //    [FromQuery] string? sortBySalary = null)
        //{
        //    if (pageNumber < 1) pageNumber = 1;
        //    if (pageSize < 1) pageSize = 10;

        //    var query = _context.Employees
        //        .Include(e => e.Department)
        //        .AsQueryable();


        //    if (!string.IsNullOrEmpty(sortBySalary))
        //    {
        //        query = sortBySalary.ToLower() == "desc"
        //            ? query.OrderByDescending(e => e.salary)
        //            : query.OrderBy(e => e.salary);
        //    }


        //    var totalCount = query.Count();

        //    var employees = query
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    return Ok(new
        //    {
        //        TotalCount = totalCount,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        Data = employees
        //    });
        //}
        [HttpGet]
        public IActionResult GetEmployeeById()
        {
            var Employee = _context.Employees.Include(d => d.Department);
            if (Employee==null)
                return NotFound(new
                {
                    message = $"Employee  not found."
                });
            return Ok(Employee);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetEmployeeById(int id) 
        { 
            var Employee =_context.Employees.Include(d=>d.Department).FirstOrDefault(e=>e.ID==id);
            if (Employee == null)
                return NotFound(new
                {
                    message = $"Employee with ID {id} not found."
                });
            return Ok(Employee);
        }


        [HttpPost]    
        public IActionResult CreateEmployee([FromBody] EmployeeDto dto)
        {
            var departmentExists = _context.Departments.Any(d => d.Id == dto.DepartmentId);
            if (!departmentExists)
                return BadRequest(new
                {
                    message = $"Department with ID {dto.DepartmentId} does not exist."
                });
            var emailExists = _context.Employees.Any(e => e.email == dto.Email);

            if (emailExists)
                return BadRequest(new
                {
                    message = "Email is already in use."
                });
            var employee = new Employee
            {
                fullname = dto.FullName,
                email = dto.Email,
                salary = dto.Salary,
                HireDate = dto.HireDate ?? DateTime.UtcNow,
                DepartmentId = dto.DepartmentId
            };
            _context.Employees.Add(employee);
            _context.SaveChanges();

            var result = _context.Employees
       .Where(e => e.ID == employee.ID)
       .Select(e => new
       {
           e.ID,
           e.fullname,
           e.email,
           e.salary,
           e.HireDate,
           e.DepartmentId,
           Department = new
           {
               e.Department.Name
           }
       })
       .FirstOrDefault();

            return CreatedAtAction(
                nameof(GetEmployeeById),
                new { id = employee.ID },
                result
            );
        }


        [HttpPut("{id:int}")]
        public IActionResult UpdateEmployee(int id, [FromBody] EmployeeDto dto)
        {
            var employee = _context.Employees.Find(id);

            if (employee == null)
                return NotFound(new
                {
                    message = $"Employee with ID {id} not found."
                });

            var departmentExists = _context.Departments
                .Any(d => d.Id == dto.DepartmentId);

            if (!departmentExists)
                return BadRequest(new
                {
                    message = $"Department with ID {dto.DepartmentId} does not exist."
                });

            var emailExists = _context.Employees
                .Any(e => e.email == dto.Email && e.ID != id);

            if (emailExists)
                return BadRequest(new
                {
                    message = "Email is already in use by another employee."
                });

            employee.fullname = dto.FullName;
            employee.email = dto.Email;
            employee.salary = dto.Salary;
            employee.HireDate = dto.HireDate ?? employee.HireDate;
            employee.DepartmentId = dto.DepartmentId;

            _context.SaveChanges();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _context.Employees.Find(id);

            if (employee == null)
                return NotFound(new
                {
                    message = $"Employee with ID {id} not found."
                });

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("department/{departmentId:int}")]
        public IActionResult GetEmployeesByDepartment(int departmentId)
        {
            var employees = _context.Employees
                .Include(e => e.Department)
                .Where(e => e.DepartmentId == departmentId)
                .ToList();

            return Ok(employees);
        }

        [HttpGet("salary/{salary:decimal}")]
        public IActionResult GetEmployeesBySalary(decimal salary)
        {
            var employees = _context.Employees
                .Include(e => e.Department)
                .Where(e => e.salary >= salary)
                .ToList();

            return Ok(employees);
        }

        [HttpGet("search/{name?}")]
        public IActionResult SearchEmployees(string? name)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(e => e.fullname.Contains(name));
            }

            var employees = query.ToList();

            return Ok(employees);
        }

        [HttpGet("by-name/{name:alpha}")]
        public IActionResult GetEmployeesByAlphaName(string name)
        {
            var employees = _context.Employees
                .Include(e => e.Department)
                .Where(e => e.fullname.Contains(name))
                .ToList();

            return Ok(employees);
        }


        [HttpGet("filter")]
        public IActionResult FilterEmployees(
            [FromQuery] int? departmentId,
            [FromQuery] decimal? minSalary,
            [FromQuery] decimal? maxSalary)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(e => e.DepartmentId == departmentId.Value);

            if (minSalary.HasValue)
                query = query.Where(e => e.salary >= minSalary.Value);

            if (maxSalary.HasValue)
                query = query.Where(e => e.salary <= maxSalary.Value);

            var employees = query.ToList();

            return Ok(employees);
        }
    }
}
