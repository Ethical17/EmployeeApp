using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace EmployeeApp.DTOs
{
    public class EmployeeCreateDto
    {
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        [Required] public DateTime DOB { get; set; }
        [MaxLength(250)] public string? Address { get; set; }
        [Required] public int DepartmentId { get; set; }
        public IFormFile? PictureFile { get; set; } // multipart form file
    }
}
