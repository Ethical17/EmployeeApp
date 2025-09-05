using EmployeeApp.Data;
using EmployeeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        [BindProperty]
        public IFormFile? PictureFile { get; set; }

        public List<Department> Departments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await _db.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (Employee == null)
            {
                return NotFound();
            }

            Departments = await _db.Departments.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var emp = await _db.Employees.FindAsync(Employee.Id);
            if (emp == null) return NotFound();

            if (!ModelState.IsValid)
            {
                Departments = await _db.Departments.ToListAsync();
                return Page();
            }

            emp.Name = Employee.Name;
            emp.DOB = Employee.DOB;
            emp.Address = Employee.Address;
            emp.DepartmentId = Employee.DepartmentId;

            // Handle picture upload
            if (PictureFile != null && PictureFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await PictureFile.CopyToAsync(ms);
                emp.Picture = ms.ToArray();
            }

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
