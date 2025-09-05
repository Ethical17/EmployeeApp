using EmployeeApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace EmployeeApi.Pages.Employees
{
    public class ExportModel : PageModel
    {
        private readonly AppDbContext _db;

        public ExportModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(string? search, string? sortOrder)
        {
            var query = _db.Employees.Include(e => e.Department).AsQueryable();

            // Apply filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search) ||
                                         e.Address.Contains(search) ||
                                         e.Department.Name.Contains(search));
            }

            // Apply sort
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(e => e.Name),
                "dob" => query.OrderBy(e => e.DOB),
                "dob_desc" => query.OrderByDescending(e => e.DOB),
                "dept" => query.OrderBy(e => e.Department.Name),
                "dept_desc" => query.OrderByDescending(e => e.Department.Name),
                _ => query.OrderBy(e => e.Name)
            };

            var employees = await query.ToListAsync();

            // Generate Excel using EPPlus
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Employees");

            ws.Cells[1, 1].Value = "Name";
            ws.Cells[1, 2].Value = "DOB";
            ws.Cells[1, 3].Value = "Address";
            ws.Cells[1, 4].Value = "Department";

            int row = 2;
            foreach (var e in employees)
            {
                ws.Cells[row, 1].Value = e.Name;
                ws.Cells[row, 2].Value = e.DOB.ToShortDateString();
                ws.Cells[row, 3].Value = e.Address;
                ws.Cells[row, 4].Value = e.Department?.Name;
                row++;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
        }
    }
}
