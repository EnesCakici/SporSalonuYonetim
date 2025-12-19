using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.Http.Headers;

namespace SporSalonuYonetim.Controllers
{
    [Authorize]
    public class AiSupportController : Controller
    {
        // 🔑 GROQ API ANAHTARINI BURAYA YAPIŞTIR
        private const string ApiKey = "";


        // Groq API Adresi
        private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePlan(int age, int weight, int height, string goal)
        {
            try
            {
                string userPrompt = $"Yaş: {age}, Kilo: {weight}, Boy: {height}, Hedef: {goal}. Bu kişi için VKI hesapla, HTML formatında (div, ul, li, strong, h4 etiketlerini kullanarak) şık bir diyet ve egzersiz programı yaz. Sadece HTML kodunu ver, markdown işareti kullanma.";

                // Groq (Llama3) Modeli Ayarları
                var requestBody = new
                {
                    model = "llama-3.1-8b-instant", // Ücretsiz ve hızlı model
                    messages = new[]
                    {
                        new { role = "system", content = "Sen uzman bir spor hocasısın. Cevaplarını sadece HTML formatında ver." },
                        new { role = "user", content = userPrompt }
                    },
                    temperature = 0.7
                };

                string jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    // Groq Auth Header
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                    var response = await client.PostAsync(ApiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var jsonNode = JsonNode.Parse(jsonResponse);

                        string aiText = jsonNode?["choices"]?[0]?["message"]?["content"]?.ToString();

                        string title = goal == "zayiflama" ? "🔥 Yağ Yakımı Programı" : "💪 Kas İnşa Programı";

                        return Json(new { success = true, title = title, message = aiText });
                    }
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        return Json(new { success = false, message = "Groq Hatası: " + response.StatusCode + " - " + errorMsg });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Sunucu Hatası: " + ex.Message });
            }
        }
    }
}