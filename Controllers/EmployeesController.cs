
using EmployeeApp.Data;
using EmployeeApp.DTOs;
using EmployeeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
namespace EmployeeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private const int MaxPageSize = 100;

        public EmployeesController(AppDbContext db)
        {
            _db = db;
        }




        [HttpGet]
        public async Task<IActionResult> Get(
        [FromQuery] string? search,
        [FromQuery] int? departmentId,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;

            var q = _db.Employees.Include(e => e.Department).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(e => e.Name.Contains(s) || (e.Address != null && e.Address.Contains(s)));
            }
            if (departmentId.HasValue)
                q = q.Where(e => e.DepartmentId == departmentId.Value);

            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            q = (sortBy?.ToLower()) switch
            {
                "name" => desc ? q.OrderByDescending(e => e.Name) : q.OrderBy(e => e.Name),
                "dob" => desc ? q.OrderByDescending(e => e.DOB) : q.OrderBy(e => e.DOB),
                "department" => desc ? q.OrderByDescending(e => e.Department!.Name) : q.OrderBy(e => e.Department!.Name),
                _ => q.OrderBy(e => e.Id)
            };

            var total = await q.CountAsync(ct);
            var items = await q.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(e => new EmployeeListDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    DOB = e.DOB,
                    Address = e.Address,
                    DepartmentName = e.Department != null ? e.Department.Name : null,
                    HasPicture = e.Picture != null
                })
                .ToListAsync(ct);

            return Ok(new { total, pageIndex, pageSize, items });
        }

        // GET api/employees/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _db.Employees.Include(x => x.Department).AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            return Ok(e); // returns full entity (including Picture bytes) - careful with size
        }

        // GET picture (return image file)
        [HttpGet("{id:int}/picture")]
        public async Task<IActionResult> GetPicture(int id)
        {
            var e = await _db.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (e?.Picture == null || e.Picture.Length == 0) return NotFound();
            // try to detect image type (default to jpeg)
            return File(e.Picture, "image/jpeg");
        }

        // POST api/employees  (multipart/form-data)
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] EmployeeCreateDto dto)
        {
            var emp = new Employee
            {
                Name = dto.Name,
                DOB = dto.DOB,
                Address = dto.Address,
                DepartmentId = dto.DepartmentId
            };

            if (dto.PictureFile != null && dto.PictureFile.Length > 0)
            {
                // validate extension
                var allowed = new[] { ".jpg", ".jpeg", ".png" };
                var ext = Path.GetExtension(dto.PictureFile.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                    return BadRequest("Only .jpg/.jpeg/.png allowed for pictures.");

                using var ms = new MemoryStream();
                await dto.PictureFile.CopyToAsync(ms);
                emp.Picture = ms.ToArray();
            }

            _db.Employees.Add(emp);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = emp.Id }, new { emp.Id });
        }

        // PUT api/employees/5  (multipart/form-data)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] EmployeeUpdateDto dto)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            emp.Name = dto.Name;
            emp.DOB = dto.DOB;
            emp.Address = dto.Address;
            emp.DepartmentId = dto.DepartmentId;

            if (dto.PictureFile != null && dto.PictureFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.PictureFile.CopyToAsync(ms);
                emp.Picture = ms.ToArray();
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/employees/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null) return NotFound();
            _db.Employees.Remove(emp);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET api/employees/export  (returns excel of filtered results)
        [HttpGet("export")]
        public IActionResult Export(string search = "", string sortColumn = "Name", string sortOrder = "asc")
        {
            var employees = _db.Employees
                .Include(e => e.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                employees = employees.Where(e =>
                    e.Name.Contains(search) ||
                    e.Department.Name.Contains(search));
            }

            employees = sortColumn.ToLower() switch
            {
                "id" => (sortOrder == "desc") ? employees.OrderByDescending(e => e.Id) : employees.OrderBy(e => e.Id),
                "department" => (sortOrder == "desc") ? employees.OrderByDescending(e => e.Department.Name) : employees.OrderBy(e => e.Department.Name),
                _ => (sortOrder == "desc") ? employees.OrderByDescending(e => e.Name) : employees.OrderBy(e => e.Name),
            };

            var empList = employees.ToList();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Employees");

                ws.Cells[1, 1].Value = "ID";
                ws.Cells[1, 2].Value = "Name";
                ws.Cells[1, 3].Value = "Department";

                for (int i = 0; i < empList.Count; i++)
                {
                    ws.Cells[i + 2, 1].Value = empList[i].Id;
                    ws.Cells[i + 2, 2].Value = empList[i].Name;
                    ws.Cells[i + 2, 3].Value = empList[i].Department?.Name;
                }

                ws.Cells.AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Employees.xlsx");
            }
        }
    }

}

