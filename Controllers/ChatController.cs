using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace PetCareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public ChatController(IHttpClientFactory httpFactory, IConfiguration config, AppDbContext context)
        {
            _httpFactory = httpFactory;
            _config = config;
            _context = context;
        }

        public class ChatMessage
        {
            public string Role { get; set; } = "user";      // "user" | "assistant"
            public string Content { get; set; } = string.Empty;
        }

        public class ChatRequest
        {
            public string Message { get; set; } = string.Empty;
            public List<ChatMessage>? History { get; set; }
        }

        // POST: api/chat
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Message))
                return BadRequest(new { error = "Tin nhắn trống." });

            // Cấu hình lấy từ appsettings.json + user-secrets (config-driven)
            var apiKey  = _config["AI:ApiKey"];
            var model   = _config["AI:Model"]   ?? "gemini-2.5-flash";
            var baseUrl = _config["AI:BaseUrl"] ?? "https://generativelanguage.googleapis.com/v1beta/openai/chat/completions";
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, new { error = "Chưa cấu hình AI:ApiKey (dùng dotnet user-secrets)." });

            // Lấy dịch vụ thật trong DB để bot tư vấn đúng giá + đúng dịch vụ của phòng khám
            var dichVus = await _context.DichVus
                .Where(d => d.TrangThai)
                .Select(d => $"- {d.TenDichVu}: chó {d.GiaCho:N0}đ, mèo {d.GiaMeo:N0}đ, khác {d.GiaKhac:N0}đ")
                .ToListAsync();

            var systemPrompt =
                "Bạn là trợ lý ảo của phòng khám thú cưng PetCare. " +
                "Trả lời NGẮN GỌN, thân thiện, bằng tiếng Việt. " +
                "Tư vấn chăm sóc chó/mèo, gợi ý dịch vụ phù hợp và hướng dẫn đặt lịch. " +
                "Tuyệt đối không chẩn đoán bệnh nặng — hãy khuyên chủ nuôi đưa thú đến khám trực tiếp. " +
                "Chỉ tư vấn dựa trên danh sách dịch vụ dưới đây:\n" +
                (dichVus.Count > 0 ? string.Join("\n", dichVus) : "(Chưa có dịch vụ nào trong hệ thống.)");

            // Dựng mảng messages: system -> lịch sử (tối đa 10 tin gần nhất) -> tin nhắn mới
            var messages = new List<object> { new { role = "system", content = systemPrompt } };
            if (req.History != null)
                foreach (var m in req.History.TakeLast(10))
                    messages.Add(new { role = m.Role, content = m.Content });
            messages.Add(new { role = "user", content = req.Message });

            var body = new { model, messages, temperature = 0.7, max_tokens = 500 };

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var httpContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            HttpResponseMessage resp;
            try
            {
                resp = await client.PostAsync(baseUrl, httpContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Không gọi được dịch vụ AI.", detail = ex.Message });
            }

            var respText = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                return StatusCode(500, new { error = "Lỗi từ dịch vụ AI.", detail = respText });

            try
            {
                using var doc = JsonDocument.Parse(respText);
                var reply = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
                return Ok(new { reply });
            }
            catch
            {
                return StatusCode(500, new { error = "Phản hồi AI không hợp lệ.", detail = respText });
            }
        }
    }
}
