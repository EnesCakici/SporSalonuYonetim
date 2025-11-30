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
            // Kullanıcıyı Bul ve Ata
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            appointment.AppUserId = userId;

            // Hizmet Detaylarını (Süre ve Fiyat) Çek
            var service = await _context.Services.FindAsync(appointment.ServiceId);

            if (service != null)
            {
                appointment.Price = service.Price;
            }
            else
            {
                // Hizmet bulunamazsa listeleri doldurup hata dön
                ModelState.AddModelError("", "Hizmet bulunamadı.");
                ViewBag.Trainers = new SelectList(_context.Trainers, "TrainerId", "TrainerName");
                ViewBag.Services = new SelectList(_context.Services, "ServiceId", "ServiceName");
                return View(appointment);
            }

            // Validation Temizliği
            ModelState.Remove("AppUserId");
            ModelState.Remove("AppUser");
            ModelState.Remove("Trainer");
            ModelState.Remove("Service");

            if (ModelState.IsValid)
            {
                // ÇAKIŞMA KONTROLÜ
                DateTime yeniBaslangic = appointment.Date;
                DateTime yeniBitis = yeniBaslangic.AddMinutes(service.DurationMinutes);

                var cakisanRandevu = await _context.Appointments
                    .Include(a => a.Service) // Süresini hesaplamak için servise ihtiyacımız var
                    .Where(a => 
                        a.TrainerId == appointment.TrainerId && 
                        (a.Date < yeniBitis && a.Date.AddMinutes(a.Service.DurationMinutes) > yeniBaslangic)
                    )
                    .FirstOrDefaultAsync();

                bool isBooked = await _context.Appointments
                    .Include(a => a.Service)
                    .AnyAsync(a =>
                        a.TrainerId == appointment.TrainerId &&
                        (a.Date < yeniBitis && a.Date.AddMinutes(a.Service.DurationMinutes) > yeniBaslangic)
                    );

                if (isBooked)
                {
                    ModelState.AddModelError("", $"Seçilen saatlerde eğitmen dolu. (Bu hizmet {service.DurationMinutes} dakika sürüyor.)");

                    // HATA OLDUĞUNDA DROPDOWNLARI TEKRAR DOLDURMAK ŞARTTIR!
                    ViewBag.Trainers = new SelectList(_context.Trainers, "TrainerId", "TrainerName", appointment.TrainerId);
                    ViewBag.Services = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);
                    return View(appointment);
                }

                _context.Add(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Model hatalıysa (Örn: Tarih seçmediyse) DROPDOWNLARI TEKRAR DOLDUR
            ViewBag.Trainers = new SelectList(_context.Trainers, "TrainerId", "TrainerName", appointment.TrainerId);
            ViewBag.Services = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);
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

        //Silme - onay ekrani
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //sadece kendi randevularını sil
            var appointments = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.AppointmentId == id && m.AppUserId == userId);

            if(appointments == null) return NotFound(); return View(appointments);
        }

        //Silme işlemi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appoinment = await _context.Appointments.FirstOrDefaultAsync(a => a.AppUserId == userId && a.AppointmentId == id);

            if(appoinment !=null)
            {
                _context.Appointments.Remove(appoinment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //Duzenleme İslemi
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sadece kendi randevularını duzenle
            var appointment = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.AppointmentId == id && m.AppUserId == userId);

            // Kontrolü en başta yapıyoruz
            if (appointment == null) return NotFound();

            // Dropdownları dolduruyoruz ki duzenlerken secili gelsin
            ViewBag.Trainers = new SelectList(_context.Trainers, "TrainerId", "TrainerName", appointment.TrainerId);
            ViewBag.Services = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            //id kontorlu
            if (id != appointment.AppointmentId) return NotFound();

            //Kullanıcıyı tekrar atıyoruz - guvenlık icin formdan beklemiyoruz
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            appointment.AppUserId = userId;

            //Fiyatı Guncelle
            //Eger hizmeti degistirdiyse fiyat da degissin
            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if(service != null)
            {
                appointment.Price = service.Price;
            }

            //formdan gelmedigi icin bos gelir hata verir, hata vermesin diye temizliyoruz
            ModelState.Remove("AppUserId");
            ModelState.Remove("AppUser");
            ModelState.Remove("Trainer");
            ModelState.Remove("Service");

            if (ModelState.IsValid)
            {
                //Sure hesapli cakicma kontrolu  
                DateTime yeniBaslangic = appointment.Date;
                DateTime yeniBitis = yeniBaslangic.AddMinutes(service.DurationMinutes);

                // Veritabanındaki diğer randevuları kontrol et
                bool isBooked = await _context.Appointments
                    .Include(a => a.Service)
                    .AnyAsync(a =>
                        a.TrainerId == appointment.TrainerId &&
                        a.AppointmentId != id && // Kendisi hariç
                        (a.Date < yeniBitis && a.Date.AddMinutes(a.Service.DurationMinutes) > yeniBaslangic)
                    );

                if (isBooked)
                {
                    ModelState.AddModelError("", $"Seçilen saatlerde eğitmen dolu. (Bu hizmet {service.DurationMinutes} dakika sürüyor ve başka bir randevu ile çakışıyor.)");

                    ViewBag.Trainers = new SelectList(_context.Trainers, "TrainerId", "TrainerName", appointment.TrainerId);
                    ViewBag.Services = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);
                    return View(appointment);
                }

                _context.Update(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Trainers = new SelectList(_context.Trainers, "TrainerId", "TrainerName", appointment.TrainerId);
            ViewBag.Services = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }

    }

}