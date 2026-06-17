using Microsoft.EntityFrameworkCore;
using PetCareBackend.Services;

var builder = WebApplication.CreateBuilder(args);
// Load appsettings.Local.json nếu có (để chứa Google OAuth config mà không commit lên GitHub)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
// Add controllers
builder.Services.AddControllers();

// HttpClient để gọi API AI (Gemini - endpoint tương thích OpenAI)
builder.Services.AddHttpClient();

// Bộ nhớ cache dùng để lưu mã OTP tạm thời (quên mật khẩu)
builder.Services.AddMemoryCache();

// Đăng ký service gửi email và lưu OTP
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<IOtpStoreService, OtpStoreService>();

// Add CORS - cho phép Angular gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Kết nối SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// ── Google OAuth ────────────────────────────────────────────────
//Thêm defaultScheme = "Cookies" vào đây
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme          = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
.AddGoogle("Google", options =>
{
    options.ClientId     = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.CallbackPath = "/signin-google";
});
// ───────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors("AllowAngular");
app.UseAuthentication(); // 
app.UseAuthorization();  // 

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.MapControllers();
app.Run();