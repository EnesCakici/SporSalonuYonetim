using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models
{
    public class Trainer
    {
        [Key]  // primery key
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Ad Soyad Girilmesi Zorunludur.")]
        [Display(Name = "Adı Soyadı")]
        public string TrainerName { get; set; }

        [Required(ErrorMessage = "Uzmanlık Alanı Girilmesi Zorunludur.")]
        [Display(Name = "Uzmanlık Alanı")]
        public string Specialization { get; set; }  //uzmanlık


        [Display(Name = "Fotograf")]
        public string? ImageUrl { get; set; }  //fotografyolu

    }
}
