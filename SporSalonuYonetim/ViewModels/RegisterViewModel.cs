using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [Display(Name = "Soayd")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage ="Geçerli bir e-posta giriniz.")]
        [Display(Name ="E-posta")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre (Tekrar)")]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }

    }
}
