using BbibbJobStreetJwtToken.Helpers;
using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;

namespace BbibbJobStreetJwtToken.Controllers
{
    public class LoginAndRegisterPageController : Controller
    {
        private readonly IUser _userService;
        private readonly IPerusahaan _perusahaan;
        private readonly ApplicationContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<LoginAndRegisterPageController> _logger;
        public LoginAndRegisterPageController(IUser userService, ApplicationContext context, JwtHelper jwtHelper, IPerusahaan perusahaan, ILogger<LoginAndRegisterPageController> logger)
        {
            _userService = userService;
            _context = context;
            _jwtHelper = jwtHelper;
            _perusahaan = perusahaan;
            _logger = logger;
        }
      
        public IActionResult RegisterUser()
        {
            return View();
        }

        public IActionResult RegisterPerusahaan()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors));
                    Console.WriteLine($"Validasi gagal: {errors}");
                    TempData["Error"] = "Data tidak valid";
                    return View(model);
                }

                Console.WriteLine("Memeriksa keberadaan user...");
                var userExists = await _userService.UserExistsAsync(model.Username, model.Email);
                if (userExists)
                {
                    Console.WriteLine("User sudah ada");
                    TempData["Error"] = "Username/email sudah digunakan";
                    return View(model);
                }


                Console.WriteLine("Membuat user baru...");
                var result = await _userService.RegisterAsync(model);

                if (!result)
                {
                    Console.WriteLine("Registrasi gagal (return false)");
                    TempData["Error"] = "Registrasi gagal";
                    return View(model);
                }

                Console.WriteLine("Registrasi berhasil");
                TempData["Success"] = "Registrasi berhasil! Silakan login.";
                return RedirectToAction("LoginUser");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.ToString()}");
                TempData["Error"] = $"Gagal registrasi: {ex.Message}";
                return View(model);
            }
        }




        public IActionResult LoginUser()
        {
            return View();
        }

        public IActionResult LoginPerusahaan()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginUserDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Data tidak valid";
                    return View(model); // Kembali ke view dengan error
                }

                var token = await _userService.LoginAsync(model);
                if (token == null)
                {
                    TempData["Error"] = "Username/password salah";
                    return View(model);
                }

                // Dapatkan user dari database (untuk role)
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user == null)
                {
                    TempData["Error"] = "User tidak ditemukan di database";
                    return View(model);
                }

                // Simpan token dan role di cookie
                Response.Cookies.Append("jwt_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    //Expires = DateTime.Now.AddSeconds(30)
                    //Expires = DateTime.Now.AddMinutes(5) // Sesuaikan dengan expiry token JWT
                    Expires = DateTime.Now.AddHours(1) // Sesuaikan dengan expiry token
                });

                Response.Cookies.Append("user_role", user.Role, new CookieOptions
                {
                    HttpOnly = false // Untuk diakses JS
                });

                // Redirect berdasarkan role
                return user.Role == "Admin"
                    ? RedirectToAction("Index", "DashboardAdmin")
                    : RedirectToAction("Index", "DashboardUser");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception saat login: {ex}");
                TempData["Error"] = $"Terjadi kesalahan saat login: {ex.Message}";
                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> RegisterPerusahaan(RegisterPerusahaanDTO model)
        {
            try
            {
                // Cek validasi model
                if (!ModelState.IsValid)
                {
                    // Tampilkan error validasi
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors));
                    TempData["Error"] = "Error validasi: " + errors;
                    return View(model);
                }

                // Cek apakah perusahaan sudah ada
                if (await _perusahaan.PerusahaanExistsAsync(model.NamaPerusahaan, model.Email))
                {
                    return View(model);
                }

                var result = await _perusahaan.RegisterAsync(model);
                if (!result)
                {
                    TempData["Error"] = "Registrasi gagal";
                    return View(model);
                }

                // Redirect ke login jika sukses
                return RedirectToAction("LoginPerusahaan");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                TempData["Error"] = $"Gagal registrasi: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoginPerusahaan(LoginPerusahaanDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Data tidak valid";
                    return View(model); 
                }

                var token = await _perusahaan.LoginAsync(model);
                if (token == null)
                {
                    TempData["Error"] = "Username/password salah";
                    return View(model);
                }

                // Dapatkan perusahaan dari database (untuk role)
                var user = await _context.Perusahaans.FirstOrDefaultAsync(u => u.NamaPerusahaan == model.NamaPerusahaan);
                if (user == null)
                {
                    return View(model);
                }

                // Simpan token dan role di cookie
                Response.Cookies.Append("jwt_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddSeconds(10)
                    //Expires = DateTime.Now.AddMinutes(5) // Sesuaikan dengan expiry token JWT
                    //Expires = DateTime.Now.AddHours(1) // Sesuaikan dengan expiry token
                });

                Response.Cookies.Append("user_role", user.Role, new CookieOptions
                {
                    HttpOnly = false // Untuk diakses JS
                });

                
                return RedirectToAction("Index", "DashboardPerusahaan");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception saat login: {ex}");
                TempData["Error"] = $"Terjadi kesalahan saat login: {ex.Message}";
                return View(model);
            }
        }

    }
}
