using BbibbJobStreetJwtToken.Helpers;
using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

// Replace 'YourDbContext' with the name of your own DbContext derived class.
builder.Services.AddDbContext<ApplicationContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(builder.Configuration.GetConnectionString("MySQLconnection"), serverVersion)
    // The following three options help with debugging, but should
    // be changed or removed for production.
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);



// Konfigurasi JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

// Validasi apakah secret key ada
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT Secret Key tidak ditemukan di appsettings.json");
}


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        ClockSkew = TimeSpan.Zero // Tidak ada toleransi waktu
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Ambil token dari cookie bernama "jwt_token"
            if (context.Request.Cookies.ContainsKey("jwt_token"))
            {
                context.Token = context.Request.Cookies["jwt_token"];
            }
            return Task.CompletedTask;
        }
    };
});



// Registrasi Services
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IPerusahaan, PerusahaanService>();
builder.Services.AddScoped<ILowonganPekerjaan, LowonganPekerjaanService>();
builder.Services.AddScoped<IKategoriPekerjaan,  KategoriPekerjaanService>();
builder.Services.AddScoped<ILamaran, LamaranService>();
builder.Services.AddScoped<ILowonganTersimpan, LowonganTersimpanService>();
builder.Services.AddScoped<IHistoryLamaran, HistoryLamaranService>();
builder.Services.AddScoped<ICompanyDashboard,  CompanyDashboardService>();
builder.Services.AddScoped<IDashboardAdminService, DashboardAdminService>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IEmailHelper, EmailHelper>();
builder.Services.AddScoped<IFileHelper, FileHelper>();
builder.Services.AddScoped<IImageHelper, ImageHelper>();
builder.Services.AddScoped<IEnkripsiPassword, EnkripsiPasswordHelper>();
builder.Services.AddScoped<ILoginLayout, LoginLayoutService>();
builder.Services.AddHttpContextAccessor(); // Untuk mengakses HttpContext


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


//// Dan sebelum UseAuthorization()
//app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Harus sebelum UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LoginPage}/{action=Index}/{id?}");
//pattern: "{controller=LoginAndRegisterPage}/{action=RegisterUser}/{id?}");

app.Run();
