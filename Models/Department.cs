using System.ComponentModel.DataAnnotations;

namespace EmployeeApp.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
