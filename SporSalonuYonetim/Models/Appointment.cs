using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporSalonuYonetim.Models
{
    public class Appointment
    {
        [Key]  // primery key
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Tarih ve saat seçimi zorunludur.")]
        [Display(Name = "Randevu tarihi")]
        public DateTime Date { get; set; }

        //iliskiler

        //hangi uye aldı? (AppUser ile baglanti)
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        //hangi antrenör?
        [Required(ErrorMessage ="Antrenör seçimi zorunludur.")]
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        //hangi hizmet
        [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }


        // Randevu alındığı andaki fiyat 
        [Display(Name = "Ödenen Ücret")]
        [Column(TypeName = "decimal(18,2)")] // Para birimi formatı için
        public decimal Price { get; set; }

    }
}
