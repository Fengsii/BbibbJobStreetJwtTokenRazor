using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BbibbJobStreetJwtToken.Controllers
{
    [Authorize(Roles = "company")]
    public class DashboardPerusahaanController : Controller
    {
        private readonly IKategoriPekerjaan _kategoriPekerjaan;
        private readonly ILowonganPekerjaan _lowonganPekerjaan;
        private readonly IPerusahaan _perusahaan;
        private readonly ILamaran _lamaran;
        private readonly ICompanyDashboard _companyDashboard;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationContext _context;
        private readonly IValidator<LowonganPekerjaanAddUpdateDTO> _jobValidator;
        private readonly IValidator<RegisterPerusahaanDTO> _registerCompanyValidator;
        public DashboardPerusahaanController(IKategoriPekerjaan kategoriPekerjaan, 
            ILowonganPekerjaan lowonganPekerjaan, IPerusahaan perusahaan, 
            IHttpContextAccessor contextAccessor, ApplicationContext context, 
            ILamaran lamaran, ICompanyDashboard companyDashboard, IValidator<RegisterPerusahaanDTO> registerCompanyValidator,
            IValidator<LowonganPekerjaanAddUpdateDTO> jobValidator) 
        {
            _kategoriPekerjaan = kategoriPekerjaan;
            _lowonganPekerjaan = lowonganPekerjaan;
            _perusahaan = perusahaan;
            _httpContextAccessor = contextAccessor;
            _context = context;
            _lamaran = lamaran;
            _companyDashboard = companyDashboard;
            _jobValidator = jobValidator;
            _registerCompanyValidator = registerCompanyValidator;
        }

        ///=============== UNTUK COMPANY PROFILE ==================\\\
  
        public IActionResult CompanyProfile()
        {
            var data = _perusahaan.GetCurrentCompany();
            return View(data);
        }

        ///=============== UNTUK COMPANY PROFILE SETTING ==================\\\
 
        [HttpGet] // Tambahkan ini
        public IActionResult CompanyProfileSetting()
        {
            var currentCompany = _perusahaan.GetCurrentCompany();

            // Convert ke RegisterPerusahaanDTO untuk form edit
            var dto = new RegisterPerusahaanDTO
            {
                PerusahaanId = currentCompany.PerusahaanId,
                NamaPerusahaan = currentCompany.NamaPerusahaan,
                Email = currentCompany.Email,
                Telepon = currentCompany.Telepon,
                Alamat = currentCompany.Alamat,
                Kota = currentCompany.Kota,
                Provinsi = currentCompany.Provinsi,
                BidangUsaha = currentCompany.BidangUsaha,
                TanggalBerdiri = currentCompany.TanggalBerdiri
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CompanyProfileSetting(RegisterPerusahaanDTO dto)
        {
            // Manual validasi pakai FluentValidation
            ValidationResult validationResult = await _registerCompanyValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    // masukkan error ke ModelState supaya muncul di View
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(dto);
            }

            var data = _perusahaan.UpdateCompany(dto);
            if (data)
            {
                return RedirectToAction(nameof(CompanyProfile));
            }
            return View(dto);
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
        public async Task<IActionResult> LowonganPekerjaanAddUpdate(LowonganPekerjaan lowonganPekerjaan)
        {

            var lowonganDTO = new LowonganPekerjaanAddUpdateDTO
            {
                Id = lowonganPekerjaan.Id,
                Judul = lowonganPekerjaan.Judul,
                Posisi = lowonganPekerjaan.Posisi,
                Alamat = lowonganPekerjaan.Alamat,
                Deskripsi = lowonganPekerjaan.Deskripsi,
                TanggalDibuat = DateTime.Now,
                status = lowonganPekerjaan.status,
                KategoriId = lowonganPekerjaan.KategoriId,
            };

            ValidationResult validationResult = await _jobValidator.ValidateAsync(lowonganDTO);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    // masukkan error ke ModelState supaya muncul di View
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                // Pastikan ViewBag.Kategori diisi kembali karena return view
                ViewBag.Kategori = _kategoriPekerjaan.KategoriPekerjaan();
                return View(lowonganPekerjaan);
            }


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
            // Jika gagal tanpa alasan yang jelas
            TempData["ErrorMessage"] = "Terjadi kesalahan saat menyimpan lowongan";
            ViewBag.Kategori = _kategoriPekerjaan.KategoriPekerjaan();
            return View(lowonganPekerjaan);
        }





        [HttpPost]
        public IActionResult DeleteLowonganPekerjaan(int id)
        {
            var data = _lowonganPekerjaan.DeleteLowonganPekerjaan(id);
            if (data)
            {
                return RedirectToAction("LowonganPekerjaan");
            }
            return BadRequest("Gagal menghapus lowongan.");
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
                return RedirectToAction("PelamarKerja");
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
