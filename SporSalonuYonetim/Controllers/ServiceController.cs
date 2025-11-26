using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers
{
    public class ServiceController : Controller
    {

        //Veritabani baglantimizi tuttacak degisken
        private readonly ApplicationDbContext _context;

        //Constructor - yapici method - dependency injection
        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        //3. Listeleme sayfasi index
        public async Task<IActionResult> Index()
        {
            //Veritabanindaki tüm services'leri listeye çevirip view e gönderir
            var services = await _context.Services.ToListAsync();
            return View(services);
        }


        //Get : Sayfayı göstermek için
        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post : Formdan gelen veriyi kaydetmek için
        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpPost]
        [ValidateAntiForgeryToken] //guvemlik onlemi
        public async Task<IActionResult> Create(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Services.Add(service);  //veritabanina ekle
                await _context.SaveChangesAsync(); //degisiklikleri kaydet
                return RedirectToAction(nameof(Index)); //Listeye geri don
            }
            //hata varsa sayfayı tekrar goster
            return View(service);
        }



        //DUZENLEME ISLEMLERİ

        // duzenleme sayfasi islemleri - mevcut verileri kutucaklara doldurur
        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services.FindAsync(id);  //o id ye sahip hizmeti bul veritabaninda

            if (service == null) return NotFound();

            return View(service); //bulunan veriyi view sayfasina gonder

        }

        //duzenlenmis veriyi veritabanina kaydeder
        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.ServiceId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service); //Entity frameworke bunu guncelle de
                    await _context.SaveChangesAsync(); //veritabanina yaz
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Services.Any(e => e.ServiceId == service.ServiceId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index)); //listeye don        
            }
            return View(service); //hata verirse sayfayi tekrar gonder
        }


        //SILME ISLEMLERİ

        //silmek istedigine emin misin onay sayfasi icin
        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            return View(service);  //silincek veriyi onay ekranına gonder
        }


        //silme islemini yapar
        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);  //kuyruga silinecek olarak ekle
            }

            await _context.SaveChangesAsync();  //veritabanindan silinecek olarak ekle
            return RedirectToAction(nameof(Index));

        }
    }
}