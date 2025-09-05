using System.ComponentModel.DataAnnotations;

namespace EmployeeApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250)]
        public string Address { get; set; }

        public byte[]? Picture { get; set; }

        [Required(ErrorMessage = "Department is required")]
        

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
