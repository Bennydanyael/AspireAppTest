using Microsoft.EntityFrameworkCore;

namespace AspireAppTest.ApiService.Data
{
    public class AppDbContextCoffee : DbContext
    {
        public AppDbContextCoffee(DbContextOptions<AppDbContextCoffee> options): base(options)
        {
        }
        public DbSet<Sale> Sales { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>()
                .HasKey(s => s.Id);
        }
    }
}
