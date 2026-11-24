using System.ComponentModel.DataAnnotations;

namespace CompanyManagementAPI.Models
{
    public class Employee
    {
        public int ID { get; set; }

        [Required]
        public string fullname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string email { get; set; }  =string.Empty ;
        [Range(.1,double.MaxValue ,ErrorMessage = " Salary must be greater than zero.")]
        public decimal salary { get; set; }

        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        public int DepartmentId {  get; set; }

        public Department? Department { get; set; }
    }
}
