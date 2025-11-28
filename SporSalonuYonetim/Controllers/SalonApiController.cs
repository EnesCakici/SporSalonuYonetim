/*
 REST API: Dışarıya açılan kapıdır.

[HttpGet], [Route], return Ok(...) kısımları REST API yapısıdır.

LINQ: İçerideki işçidir. Veritabanında sorgulama yapar.

.Where(...), .Select(...), .ToList() kısımları LINQ kodudur. */


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalonApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalonApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        //1- Tum Egitmenleri Getir     //https://localhost:7131/api/SalonApi/Trainers
        [HttpGet("Trainers")]
        public async Task<IActionResult> GetTrainers()
        {
            //Select --> yalnizca grekli olan veriyi cekiyoruz
            var trainers = await _context.Trainers.Select(t => new
            {
                t.TrainerId,
                t.TrainerName,
                t.Specialization,
                t.ImageUrl
            })
                .ToListAsync();
            return Ok(trainers);  //başarılı doner ( başarılının kodu 200)

        }

        //2- Hizmetleri Getir
        [HttpGet("Services")]  //https://localhost:7131/api/SalonApi/Services
        public async Task<IActionResult> GetServices()
        {
            var services = await _context.Services.Select(s => new
            {
                s.ServiceId , s.ServiceName , s.Price , s.DurationMinutes
            })
                .ToListAsync();
            return Ok(services);
        }

        //3- Filtreleme - uzmanlıga gore  //https://localhost:7131/api/SalonApi/SearchTrainer?skill=Boks
        [HttpGet("SearchTrainer")]
        public async Task<IActionResult> SearchTrainer(string skill)
        {
            var trainers = await _context.Trainers.Where(t=> t.Specialization.ToLower().Contains(skill.ToLower()))
            .Select(t => new
            {
                t.TrainerName,
                t.Specialization,
            })
            .ToListAsync() ;

        if(trainers.Count == 0)
            {
                return NotFound("Bu uzmanlık alanında eğitmen bulunamadı.");
            }
        return Ok(trainers);

        }

        //4- Tarihe gore randevuları getir   //https://localhost:7131/api/SalonApi/GetAppointments?date=2025-11-27
        [HttpGet("GetAppointments")]  
        public async Task<IActionResult> GetAppointments(DateTime date)
        {
            var appointments = await _context.Appointments.Where(a => a.Date.Date == date.Date)
                .Select(a => new
                {
                    Tarih = a.Date,
                    Egitmen = a.Trainer.TrainerName,
                    Hizmet = a.Service.ServiceName,
                    Uye = a.AppUser.UserName
                })
                .ToListAsync() ;
            if (appointments.Count == 0)
            {
                return NotFound("Bu tarihte kayıtlı bir randevu bulunamadı");
            }
            return Ok(appointments);
        }
    }
}
