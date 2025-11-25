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
    }
}
