using EmployeeApp.Data;
using EmployeeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace EmployeeApp.Pages.Employees
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;
        public CreateModel(AppDbContext db) => _db = db;

        [BindProperty] public InputModel Input { get; set; } = new();
        public SelectList Departments { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var deps = await _db.Departments.AsNoTracking().ToListAsync();
            Departments = new SelectList(deps, "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var emp = new Employee
            {
                Name = Input.Name,
                DOB = Input.DOB,
                Address = Input.Address,
                DepartmentId = Input.DepartmentId
            };

            if (Input.PictureFile != null && Input.PictureFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await Input.PictureFile.CopyToAsync(ms);
                emp.Picture = ms.ToArray();
            }

            _db.Employees.Add(emp);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }

        public class InputModel
        {
            public string Name { get; set; } = "";
            public DateTime DOB { get; set; }
            public string? Address { get; set; }
            public int DepartmentId { get; set; }
            public IFormFile? PictureFile { get; set; }
        }


    }
}
