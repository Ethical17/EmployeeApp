using System;
namespace EmployeeApp.DTOs
{
    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime DOB { get; set; }
        public string? Address { get; set; }
        public string? DepartmentName { get; set; }
        public bool HasPicture { get; set; }
    }
}
