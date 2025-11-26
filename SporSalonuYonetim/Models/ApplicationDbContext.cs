using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SporSalonuYonetim.Models
{

    //basta DbContext olan miras alma olayini IdendityDbcontext<AppUser> olarak degistiriyoruz   --> güvenlik sistemi kurulu hazır bir binadır
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { }

        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        //identyDbcontext, appuser ve roller için dbset olayını hallediyor



    }
}