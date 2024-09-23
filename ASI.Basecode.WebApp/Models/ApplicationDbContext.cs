using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.WebApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSet for categories
        public DbSet<CategoryModel> Categories { get; set; }
    }
}
