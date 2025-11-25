using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SporSalonuYonetim.Models;
using SporSalonuYonetim.ViewModels;

namespace SporSalonuYonetim.Controllers
{
    public class AccountController : Controller
    {
        //UserManager: Kullanıcı olusturma silme sifre degistimr islemleri
        private readonly UserManager<AppUser> _userManager;

        //SignManager: kullanıcının oturm acmasini cookie olusturmasini saglar
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // KAYIT SAYFASINI GOSTERME (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // KAYIT ISLEMINI YAPMA (POST)

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid) {
                //viewmodelden gelen veriyi gercek appuser nesnesine donustur
                var user = new AppUser
                {
                    UserName = model.Email,  //kullanici adı eposta
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };

                //CreateAsync metodu sifreyi otomatik hashler(kripto) ve kaydeder
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home"); //anasayfaya gonder
                }

                //hata varsa (orn sifrede bi sikinti var), hatalari ekrana bas
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);

        }

        //Login islemleri

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if ((ModelState.IsValid))
            {
                // PasswordSignInAsync: Veritabanına bakar, e-posta ve şifre eşleşiyor mu kontrol eder.
                // false (lockoutOnFailure): Şifreyi 3 kere yanlış girince hesabı kilitleme (şimdilik kapalı).
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if ((result.Succeeded))
                {
                    return RedirectToAction("Index", "Home");
                }

                //hata varsa
                ModelState.AddModelError("", "E-posta veya şifre hatalı.");               
            }
            return View(model);            
        }


        //Çıkış yap (logout)

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); //cerezi sil
            return RedirectToAction("Index", "Home");
        }





    }
}
