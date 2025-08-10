using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BbibbJobStreetJwtToken.Controllers
{
    public class DashboardUserController : BaseController
    {

        private readonly ILowonganPekerjaan _lowonganPekerjaan;
        private readonly ILamaran _lamaran;
        private readonly ILowonganTersimpan _lowonganTersimpan;

        public DashboardUserController(ILowonganPekerjaan lowonganPekerjaan, ILamaran lamaran, ILowonganTersimpan lowonganTersimpan)
        {
            _lowonganPekerjaan = lowonganPekerjaan;
            _lamaran = lamaran;
            _lowonganTersimpan = lowonganTersimpan;
        }

        public IActionResult Index()
        {
            var data = _lowonganPekerjaan.GetListLowonganPekerjaan();
            return View(data);
        }

        [Authorize]
        [HttpGet]
        public IActionResult FormLamaran(int lowonganId)
        {
            var model = new LamaranAddUpdateDTO
            {
                LowonganId = lowonganId
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FormLamaran(LamaranAddUpdateDTO lamaranAddUpdateDTO, int lowonganId)
        {
            // Ambil UserId dari JWT token
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                // Kalau token tidak valid atau tidak ada ID, kembalikan Unauthorized
                return Unauthorized();
            }

            lamaranAddUpdateDTO.UserId = int.Parse(userIdClaim); // isi UserId di DTO

            if (lamaranAddUpdateDTO.Id == 0)
            {
                var data = await _lamaran.AddLamaran(lamaranAddUpdateDTO, lowonganId);
                if (data)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                var data = _lamaran.UpdateLamaran(lamaranAddUpdateDTO);
                if (data)
                {
                    return RedirectToAction("Index");
                }
            }

            // Jika gagal, kembalikan ke view dengan model yang sama
            return View(lamaranAddUpdateDTO);
        }

        
        public IActionResult LowonganTersimpan()
        {
            var data = _lowonganTersimpan.GetListLowonganTersimpan();
            return View(data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LowonganTersimpan(LowonganTersimpanAddUpdateDTO lowonganTersimpan, int lowonganId)
        {
            // Ambil UserId dari JWT token
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                // Kalau token tidak valid atau tidak ada ID, kembalikan Unauthorized
                return Unauthorized();
            }

            lowonganTersimpan.PenggunaId = int.Parse(userIdClaim); // isi UserId di DTO

            if (lowonganTersimpan.Id == 0)
            {
                var data = await _lowonganTersimpan.AddLowonganTersimpan(lowonganTersimpan, lowonganId);
                if (data)
                {
                    return RedirectToAction("Index");
                }
            }
           

            // Jika gagal, kembalikan ke view dengan model yang sama
            return View(lowonganTersimpan);
        }

    }
}
