using EmployeeApp.Data;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();   // API endpoints
app.MapRazorPages(); ;

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
