using Microsoft.AspNetCore.Identity;

namespace SporSalonuYonetim.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

// IdentityUser sınıfından miras almamizin nedeni: 
// Email, PasswordHash, PhoneNumber gibi alanlar otomatik gelmesi.
