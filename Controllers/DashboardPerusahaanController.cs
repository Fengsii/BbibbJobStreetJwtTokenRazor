using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BbibbJobStreetJwtToken.Controllers
{
    public class DashboardPerusahaanController : Controller
    {
        private readonly IKategoriPekerjaan _kategoriPekerjaan;
        private readonly ILowonganPekerjaan _lowonganPekerjaan;
        private readonly IPerusahaan _perusahaan;
        public DashboardPerusahaanController(IKategoriPekerjaan kategoriPekerjaan, ILowonganPekerjaan lowonganPekerjaan, IPerusahaan perusahaan)
        {
            _kategoriPekerjaan = kategoriPekerjaan;
            _lowonganPekerjaan = lowonganPekerjaan;
            _perusahaan = perusahaan;
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
            if(kategoriPekerjaanDTO.Id == 0)
            {
                var data = _kategoriPekerjaan.AddKategoriPekerjaaan(kategoriPekerjaanDTO);
                if(data)
                {
                    return RedirectToAction("KategoriPekerjaan");
                }
            }
            else
            {
                var data = _kategoriPekerjaan.UpdateKategoriPekerjaan(kategoriPekerjaanDTO);
                if(data)
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
            if(data)
            {
                return RedirectToAction("KategoriPekerjaan");
            }
            return BadRequest("Gagal menghapus supplier.");
        }



        //==== UNTUK LOWONGAN PEKERJAAN ====\\
        public IActionResult LowonganPekerjaan()
        {
            var data = _lowonganPekerjaan.GetListLowonganPekerjaan();
            return View(data);
        }

        public IActionResult LowonganPekerjaanAddUpdate(int id)
        {
            ViewBag.Perusahaan = _perusahaan.Perusahaan();
            ViewBag.Kategori = _kategoriPekerjaan.KategoriPekerjaan();
            var data = _lowonganPekerjaan.GetLowonganPekerjaanById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult LowonganPekerjaanAddUpdate(LowonganPekerjaan lowonganPekerjaan)
        {
            var lowonganDTO = new LowonganPekerjaanAddUpdateDTO
            {
                Id = lowonganPekerjaan.Id,
                Judul = lowonganPekerjaan.Judul,
                Posisi = lowonganPekerjaan.Posisi,
                Deskripsi = lowonganPekerjaan.Deskripsi,
                TanggalDibuat = lowonganPekerjaan.TanggalDibuat,
                status = lowonganPekerjaan.status,
                KategoriId = lowonganPekerjaan.KategoriId,
                PerusahaanId = lowonganPekerjaan.PerusahaanId,
            };

            if (lowonganPekerjaan.Id == 0)
            {
                var data = _lowonganPekerjaan.AddLowonganPekerjaan(lowonganDTO);
                if (data)
                {
                    return RedirectToAction("LowonganPekerjaan");
                }
            }
            else
            {
                var data = _lowonganPekerjaan.UpdateLowonganPekerjaan(lowonganDTO);
                if (data)
                {
                    return RedirectToAction("LowonganPekerjaan");
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult DeleteLowonganPekerjaan(int id)
        {
            var data = _lowonganPekerjaan.DeleteLowonganPekerjaan(id);
            if (data)
            {
                return RedirectToAction("LowonganPekerjaan");
            }
            return BadRequest("Gagal menghapus supplier.");
        }

    }
}
