using Microsoft.EntityFrameworkCore;

namespace EmployeeManageAPI.DataModels
{
    public class DataContext : DbContext
    {
        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-BSDT7T4;Database=EmployeeManageDb;Integrated security=true");
            }
        }
    }
}
