using System.ComponentModel.DataAnnotations;

namespace CompanyManagementAPI.DTOs
{
    public class DepartmentDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}