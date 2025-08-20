using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BbibbJobStreetJwtToken.Controllers
{
    public class DashboardPerusahaanController : Controller
    {
        private readonly IKategoriPekerjaan _kategoriPekerjaan;
        private readonly ILowonganPekerjaan _lowonganPekerjaan;
        private readonly IPerusahaan _perusahaan;
        private readonly ILamaran _lamaran;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationContext _context;
        public DashboardPerusahaanController(IKategoriPekerjaan kategoriPekerjaan, ILowonganPekerjaan lowonganPekerjaan, IPerusahaan perusahaan, IHttpContextAccessor contextAccessor, ApplicationContext context, ILamaran lamaran)
        {
            _kategoriPekerjaan = kategoriPekerjaan;
            _lowonganPekerjaan = lowonganPekerjaan;
            _perusahaan = perusahaan;
            _httpContextAccessor = contextAccessor;
            _context = context;
            _lamaran = lamaran;
        }
        public IActionResult Index()
        {
            return View();
        }

        //==== UNTUK LOWONGAN PEKERJAAN ====\\
        public IActionResult LowonganPekerjaan()
        {
            var data = _lowonganPekerjaan.GetListLowonganPekerjaan();
            return View(data);
        }

        public IActionResult LowonganPekerjaanAddUpdate(int id)
        {
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
                Alamat = lowonganPekerjaan.Alamat,
                Deskripsi = lowonganPekerjaan.Deskripsi,
                TanggalDibuat = lowonganPekerjaan.TanggalDibuat,
                status = lowonganPekerjaan.status,
                KategoriId = lowonganPekerjaan.KategoriId,
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



        //============= UNTUK LAMARAN PEKERJAAN ==============\\

        public IActionResult PelamarKerja(int? page, string searchTerm = "")
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var data = _lamaran.GetListLamaran(pageNumber, pageSize, searchTerm);

            ViewBag.SearchTerm = searchTerm; // biar value input search tetap ada
            return View(data);
        }



        [HttpGet]
        public async Task<IActionResult> DownloadCv(int lamaranId)
        {
            var fileBytes = await _lamaran.DownloadCvAsync(lamaranId);
            var lamaran = _context.Lamarans.FirstOrDefault(x => x.Id == lamaranId);

            if (lamaran == null || string.IsNullOrEmpty(lamaran.CV))
                return NotFound("CV tidak ditemukan");

            var fileName = lamaran.CV.EndsWith(".pdf") ? lamaran.CV : $"{lamaran.CV}.pdf";

            return File(fileBytes, "application/pdf", fileName);
        }


    }
}
