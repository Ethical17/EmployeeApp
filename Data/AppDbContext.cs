using EmployeeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().HasKey(e => e.Id);
            modelBuilder.Entity<Department>().HasKey(d => d.Id);


            modelBuilder.Entity<Employee>()
                .Property(e => e.Picture)
                .HasColumnType("varbinary(max)");

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "HR" },
                new Department { Id = 2, Name = "Engineering" },
                new Department { Id = 3, Name = "Sales" }
            );
        }

    }
}
