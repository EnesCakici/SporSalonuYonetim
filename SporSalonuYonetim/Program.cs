using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Models; 

var builder = WebApplication.CreateBuilder(args);

// Projeye veritabaný servisini ekler ve PostgreSQL kullanacaðýný belirtip, 
// baðlantý adresini (þifre vs.) appsettings.json dosyasýndan okur.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

/*
 AddDbContext: "Benim veritabaný sýnýfým bu, projeye tanýt" der.
UseNpgsql: "Microsoft SQL deðil, PostgreSQL motorunu kullan" der.
GetConnectionString: "Þifreyi kodun içine yazma, git appsettings.json dosyasýndan oku" der.*/


//Identity(uyelik) servisi

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;  //rakam zorunl diil
    options.Password.RequiredLength = 3; //en az 3 karakter
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;  //sembol zorunlulugunu kaldýrdým. bunlar test icin. yoksa real de true þeklinde çalýþacak
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        //yazdýgým seeder sinifini cagiriyorum
        await SporSalonuYonetim.Services.UserSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Seed hatasý " + ex.Message);
    }
}

app.Run();
