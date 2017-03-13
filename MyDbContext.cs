using Microsoft.EntityFrameworkCore;

namespace EFTest
{
    class MyDbContext : DbContext 
    {
        public DbSet<Thing> Things { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("User ID=jonathantaylor;Host=localhost;Port=5432;");
        }
    }
}
