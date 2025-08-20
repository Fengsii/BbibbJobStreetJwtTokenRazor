using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BbibbJobStreetJwtToken.Controllers
{
    public class DashboardAdminController : Controller
    {
        private readonly IKategoriPekerjaan _kategoriPekerjaan;
        public DashboardAdminController(IKategoriPekerjaan kategoriPekerjaan)
        {
            _kategoriPekerjaan = kategoriPekerjaan;
        }

        public IActionResult Index()
        {
            return View();
        }



        //==== UNTUK KATEGORI ====\\
        public IActionResult KategoriPekerjaan()
        {
            var data = _kategoriPekerjaan.GetListKategoriPekerjaan();
            return View(data);

        }

        public IActionResult KategoriPekerjaanAddUpdate(int id)
        {
            var data = _kategoriPekerjaan.GetListKategoriPekerjaanById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult KategoriPekerjaanAddUpdate(KategoriPekerjaanDTO kategoriPekerjaanDTO)
        {
            if (kategoriPekerjaanDTO.Id == 0)
            {
                var data = _kategoriPekerjaan.AddKategoriPekerjaaan(kategoriPekerjaanDTO);
                if (data)
                {
                    return RedirectToAction("KategoriPekerjaan");
                }
            }
            else
            {
                var data = _kategoriPekerjaan.UpdateKategoriPekerjaan(kategoriPekerjaanDTO);
                if (data)
                {
                    return RedirectToAction("KategoriPekerjaan");
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult DeleteKategoriPekerjaan(int id)
        {
            var data = _kategoriPekerjaan.DeleteKategoriPekerjaan(id);
            if (data)
            {
                return RedirectToAction("KategoriPekerjaan");
            }
            return BadRequest("Gagal menghapus supplier.");
        }

    }
}
