using CompanyManagementAPI.Data;
using CompanyManagementAPI.DTOs;
using CompanyManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetDepartment()
        {
            var Dep = _context.Departments.Include(e => e.Employees).ToList();
            if (Dep == null)
            {
                return NotFound();
            }
            return Ok(Dep);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetDepartmentById(int id)
        {
            var Dep = _context.Departments.Include(a => a.Employees).FirstOrDefault(e => e.Id == id);
            if (Dep == null)
            {
                return NotFound(new { message = $"No Department with this {id} ID  " });
            }
            return Ok(Dep);
        }
        [HttpPost]
        public IActionResult CreateDepartment([FromBody] DepartmentDto dto)
        {
            var Department = new Department
            {
                Name = dto.Name,
                Description = dto.Description,
                Location = dto.Location,
                CreatedAt = DateTime.UtcNow
            };
            _context.Departments.Add(Department);
            _context.SaveChanges();
            return CreatedAtAction(
                nameof(GetDepartmentById),
                new { id = Department.Id },
                Department

                );
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateDepartment(int id, [FromBody] DepartmentDto dto)
        {
            var Department = _context.Departments.Find(id);
                if (Department==null) 
            {
                return NotFound(new { message = $"Department with ID {id} not found." });
            }
              Department.Name = dto.Name;
            Department.Description = dto.Description;
            Department.Location = dto.Location;
            _context.SaveChanges();
            return Ok(Department);

        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteDepartment(int id)
        {
            var Department = _context.Departments.Find(id);
            if (Department == null)
            {
                return NotFound(new
                {
                    message = $"Department with ID {id} not found."
                });

            }
            bool hasEmployee = _context.Employees.Any(e => e.DepartmentId==id);
            if (hasEmployee)
                return BadRequest(new
                {
                    message = "Cannot delete a department that still has employees."
                });
            _context.Departments.Remove(Department);
            _context.SaveChanges();
            return NoContent();

        }
    }
}

        