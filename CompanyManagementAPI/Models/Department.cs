using System.ComponentModel.DataAnnotations;

namespace CompanyManagementAPI.Models
{
    public class Department
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } =string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location {  get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    }

}
