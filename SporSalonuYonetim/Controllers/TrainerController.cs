using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Trainers.ToListAsync();
            return View(trainers);
        }

        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }

        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Trainers.Add(trainer);  //veritabanina ekle
                await _context.SaveChangesAsync(); //degisiklikleri kaydet
                return RedirectToAction(nameof(Index)); //listeye geri don
            }
            //hata varsa sayfayi tekrar goster
            return View(trainer);
        }


        //Duzenleme islemleri
        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if ((id == null)) return NotFound();
            
            var trainer = await _context.Trainers.FindAsync(id);  //o id ye sahip trainer ı bul
            if(trainer == null) return NotFound();
            return View(trainer);
        }

        //duzenlenmis veriyi veritabanina kaydet
        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trainer trainer)
        {
            if (id != trainer.TrainerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Trainers.Any(e => e.TrainerId == trainer.TrainerId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        //Silme islemleri
        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if ((id == null)) return NotFound();
            
            var trainer = await _context.Trainers.FindAsync(id);
            if(trainer == null) return NotFound();

            return View(trainer);
        }

        [Authorize(Roles = "Admin")] //Yalnizca admin rolunde olanlar girebilir
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer); //kuyruga silinecek olarak ekle
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        



        
    }
}
