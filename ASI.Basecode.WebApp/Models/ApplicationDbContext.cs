using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.WebApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSet for categories
<<<<<<< HEAD
        public DbSet<CategoryModel> Categories { get; set; }
        //public DbSet<DebitModel> Debits { get; set; }
        //public DbSet<LiabilityModel> Liabilities { get; set; }
=======
        public DbSet<CategoryModel> M_Category { get; set; }
>>>>>>> 51c208272ba491f3a0cb176cd8638b9aa32ac5d7
    }
}
