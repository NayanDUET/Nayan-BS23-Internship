using Microsoft.EntityFrameworkCore;
using StudentPortWeb.Data.Entities;

namespace StudentPortWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; }
    }
}
