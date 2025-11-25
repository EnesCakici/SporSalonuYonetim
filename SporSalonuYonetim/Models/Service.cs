using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [Display(Name ="Hizmet Adı")]
        public string ServiceName { get; set; }

        [Display(Name = "Süre")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Ücret")]
        public decimal Price { get; set; }



    }
}
