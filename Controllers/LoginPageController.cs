using BbibbJobStreetJwtToken.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BbibbJobStreetJwtToken.Controllers
{
    public class LoginPageController : Controller
    {
        private readonly JwtHelper _jwtHelper;
        public LoginPageController(JwtHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
        }
        public IActionResult Index()
        {
            if (Request.Cookies.TryGetValue("jwt_token", out var token))
            {
                if (_jwtHelper.ValidateToken(token))
                {
                    if (Request.Cookies.TryGetValue("user_role", out var role))
                    {
                        switch (role)
                        {
                            case "Admin":
                                return RedirectToAction("Index", "DashboardAdmin");
                            case "User":
                                return RedirectToAction("Index", "DashboardUser");
                            case "company":
                                return RedirectToAction("Index", "DashboardPerusahaan");
                        }
                    }
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            // Hapus cookie token
            Response.Cookies.Delete("jwt_token");

            // Hapus cookie role
            Response.Cookies.Delete("user_role");

            // Pesan berhasil logout
            TempData["Success"] = "Berhasil logout";

            // Kembali ke halaman login
            return RedirectToAction("Index");
        }

    }
}
