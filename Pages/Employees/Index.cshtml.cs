using EmployeeApp.Data;
using EmployeeApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Employee> Employees { get; set; } = new();

        // For search & sorting
        public string CurrentFilter { get; set; } = string.Empty;
        public string CurrentSort { get; set; } = string.Empty;
        public string NameSort { get; set; } = string.Empty;
        public string DobSort { get; set; } = string.Empty;
        public string DeptSort { get; set; } = string.Empty;

        public async Task OnGetAsync(string? search, string? sortOrder)
        {
            // Keep track of sort state
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DobSort = sortOrder == "dob" ? "dob_desc" : "dob";
            DeptSort = sortOrder == "dept" ? "dept_desc" : "dept";
            CurrentSort = sortOrder ?? "";
            CurrentFilter = search ?? "";

            var query = _db.Employees.Include(e => e.Department).AsQueryable();

            // 🔍 Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search) ||
                                         e.Address.Contains(search) ||
                                         e.Department.Name.Contains(search));
            }

            // 🔽 Sorting
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(e => e.Name),
                "dob" => query.OrderBy(e => e.DOB),
                "dob_desc" => query.OrderByDescending(e => e.DOB),
                "dept" => query.OrderBy(e => e.Department.Name),
                "dept_desc" => query.OrderByDescending(e => e.Department.Name),
                _ => query.OrderBy(e => e.Name)
            };

            Employees = await query.ToListAsync();
        }
    }
}
