using System.ComponentModel.DataAnnotations;

namespace CompanyManagementAPI.DTOs
{
    public class EmployeeDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be greater than zero.")]
        public decimal Salary { get; set; }

        public DateTime? HireDate { get; set; }

        [Required]
        public int DepartmentId { get; set; }
    }
}