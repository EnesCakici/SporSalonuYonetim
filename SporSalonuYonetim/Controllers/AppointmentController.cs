using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers
{
    [Authorize] //sadece giris yapmis uyeler randevu alabilsin
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Randevu alma sayfasi (get)
        [HttpGet]
        public IActionResult Create()
        {
            // Dropdown için verileri hazırlayıp "Çantaya (ViewBag)" koyuyoruz.
            // SelectList(Liste, "ArkaPlandakiDeğer", "EkrandaGörünecekYazı")
            ViewBag.Trainers = new SelectList(_context.Trainers, "TrainerId", "TrainerName");
            ViewBag.Services = new SelectList(_context.Services, "ServiceId", "ServiceName");

            return View();
        }

        //Randevuyu kaydetme (post)
        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            //giris yapan kullanicimin id'sini buluyoruz
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            appointment.AppUserId = userId;

            // fiyati hizmet tablosundan çekip sabitliyoruz
            var selectService = await _context.Services.FindAsync(appointment.ServiceId);
            if (selectService != null)
            {
                appointment.Price = selectService.Price;
            }

            //kayit islemi
            //bazi zorunlu alan hatalarini silme islemi - cunku onlari biz doldurduk
            ModelState.Remove("AppUserId");
            ModelState.Remove("AppUser");
            ModelState.Remove("Trainer");
            ModelState.Remove("Service");

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();

                //basarili olursa anasayfaya yonlendir
                return RedirectToAction("Index");
            }

            //Hata varsa dropdownları tekrar doldur sayfa bozuk gelmesin
            ViewBag.Trainers = new SelectList(_context.Trainers, "TrainerId", "TrainerName");
            ViewBag.Services = new SelectList(_context.Services, "ServiceId", "ServiceName");
            return View(appointment);


        }

        //RANDEVULARIM SAYFASI

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //Giris yapan kullanicinin ıd'si
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Sadece bu kullaniciya ait Randevulari getir
            var appointments = await _context.Appointments
                                    .Include(a => a.Trainer)
                                    .Include(a => a.Service)
                                    .Where(a => a.AppUserId == userId)   //filtreleme
                                    .OrderByDescending(a => a.Date) //en yeni tarihten azalan
                                    .ToListAsync();

            return View(appointments);
                
        }
    }

}