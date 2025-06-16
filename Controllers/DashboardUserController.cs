using Microsoft.AspNetCore.Mvc;

namespace BbibbJobStreetJwtToken.Controllers
{
    public class DashboardUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
