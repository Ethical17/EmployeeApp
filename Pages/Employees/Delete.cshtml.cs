using EmployeeApp.Data;
using EmployeeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Pages.Employees
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _db;

        public DeleteModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await _db.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (Employee == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var emp = await _db.Employees.FindAsync(id);

            if (emp == null)
                return NotFound();

            _db.Employees.Remove(emp);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
