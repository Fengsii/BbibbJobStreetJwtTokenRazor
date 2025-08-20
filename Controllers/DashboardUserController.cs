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
        private readonly IHistoryLamaran _historyLamaran;

        public DashboardUserController(ILowonganPekerjaan lowonganPekerjaan, ILamaran lamaran, ILowonganTersimpan lowonganTersimpan, IHistoryLamaran historyLamaran)
        {
            _lowonganPekerjaan = lowonganPekerjaan;
            _lamaran = lamaran;
            _lowonganTersimpan = lowonganTersimpan;
            _historyLamaran = historyLamaran;
        }


        //================ Lowongan Pekerjaan =============\\

        public IActionResult Index(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                // Jika ada parameter search, gunakan method search
                var searchResults = _lowonganPekerjaan.SearchLowonganPekerjaan(search);
                return View(searchResults);
            }
            else
            {
                // Jika tidak ada parameter search, tampilkan semua
                var data = _lowonganPekerjaan.GetListLowonganPekerjaanForUser();
                return View(data);
            }
        }



        //=================  Form Lamaran Pekerjaan ===============\\
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




        //================ Lowongan Tersimpan =============\\
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

        [HttpPost]
        public IActionResult DeleteLowonganTersimpan(int id)
        {
            var data = _lowonganTersimpan.DeleteLowonganTersimpan(id);
            if (data)
            {
                return RedirectToAction(nameof(LowonganTersimpan));
            }
            return BadRequest("Gagal menghapus.");
        }




        //============== History Lamaran ==============\\

        public IActionResult HistoryLamaran()
        {
            var data = _historyLamaran.GetListHistoryLowongan();
            return View(data);
        }

        [HttpPost]
        public IActionResult DeleteHistoryLamaran(int id)
        {
            var data = _historyLamaran.DeletelamaranTersimpan(id);
            if (data)
            {
                return RedirectToAction(nameof(HistoryLamaran));
            }
            return BadRequest("Gagal menghapus.");
        }




        //================ Notifications =============\\
        public IActionResult Notifications()
        {

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var data = _lowonganPekerjaan.GetNotificationsForUser(userId);
            return View(data);
        }

        [HttpPost]
        public IActionResult DeleteNotifications(int id)
        {
            var data = _lowonganPekerjaan.DeleteNotifications(id);
            if (data)
            {
                return RedirectToAction(nameof(Notifications));
            }
            return BadRequest("Gagal menghapus.");
        }

    }
}
