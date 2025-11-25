using Microsoft.EntityFrameworkCore;

namespace SporSalonuYonetim.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { }

        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Service> Services { get; set; }



    }
}