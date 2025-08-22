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
        private readonly ICompanyDashboard _companyDashboard;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationContext _context;
        public DashboardPerusahaanController(IKategoriPekerjaan kategoriPekerjaan, 
            ILowonganPekerjaan lowonganPekerjaan, 
            IPerusahaan perusahaan, 
            IHttpContextAccessor contextAccessor, ApplicationContext context, 
            ILamaran lamaran, ICompanyDashboard companyDashboard)
        {
            _kategoriPekerjaan = kategoriPekerjaan;
            _lowonganPekerjaan = lowonganPekerjaan;
            _perusahaan = perusahaan;
            _httpContextAccessor = contextAccessor;
            _context = context;
            _lamaran = lamaran;
            _companyDashboard = companyDashboard;
        }

        ///=============== UNTUK DASHBOARD ==================\\\
        public IActionResult Index()
        {
            var data = _companyDashboard.GetDashboardData();
            return View(data);
        }

        ///=============== UNTUK LOWONGAN PEKERJAAN ==============\\\
        public IActionResult LowonganPekerjaan(int? page, string searchTerm = "")
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var data = _lowonganPekerjaan.GetListLowonganPekerjaan(pageNumber, pageSize, searchTerm);

            ViewBag.SearchTerm = searchTerm;
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

        //HttpGet khusus buat nampilin form edit
        [HttpGet]
        public IActionResult UpdateStatusPelamar(int id)
        {
            var lamaran = _lamaran.GetLamaranById(id);
            if (lamaran == null)
            {
                return NotFound();
            }

            var dto = new LamaranAddUpdateDTO
            {
                Id = lamaran.Id,
                Nama = lamaran.Nama,
                Email = lamaran.Email,
                NoHP = lamaran.NoHP,
                Pendidikan = lamaran.Pendidikan,
                Status = lamaran.Status
            };

            return View(dto);
        }


        [HttpPost]
        public IActionResult UpdateStatusPelamar(LamaranAddUpdateDTO dto)
        {
            var data = _lamaran.UpdateLamaran(dto);
            if (data)
            {
                return RedirectToAction(nameof(PelamarKerja));
            }
            return View();
        }

        [HttpPost]
        public IActionResult DeleteLamaran(int id)
        {
            var data = _lamaran.DeleteLamaran(id);
            if (data)
            {
                return RedirectToAction("LowonganPekerjaan");
            }
            return BadRequest("Gagal menghapus supplier.");
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
