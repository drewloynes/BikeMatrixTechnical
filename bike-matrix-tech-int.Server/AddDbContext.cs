using Microsoft.EntityFrameworkCore;

namespace bike_matrix_tech_int.Server
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bike> Bikes { get; set; }
    }
}