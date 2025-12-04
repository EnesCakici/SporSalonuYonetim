using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Models;
using SporSalonuYonetim.ViewModels;

namespace SporSalonuYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //veritabanindan vitrin icin veri cekiyoruz
            var model = new HomeViewModel
            {
                Trainers = await _context.Trainers.Take(3).ToListAsync(),
                Services = await _context.Services.Take(3).ToListAsync()
            };
            //View'i model'a gonderiyoruz
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ApiTest()
        {
            return View();
        }

        //Varsayikan Hata yonetimi
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
