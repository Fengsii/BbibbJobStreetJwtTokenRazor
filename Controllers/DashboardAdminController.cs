using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models.DTO;
using BbibbJobStreetJwtToken.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BbibbJobStreetJwtToken.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardAdminController : Controller
    {
        private readonly IKategoriPekerjaan _kategoriPekerjaan;
        private readonly ILowonganPekerjaan _lowonganPekerjaan;
        private readonly IPerusahaan _perusahaan;
        private readonly IUser _user;
        private readonly IDashboardAdminService _dashboardAdminService;
        private readonly IValidator<KategoriPekerjaanDTO> _categoryJobValidator;
        public DashboardAdminController(IKategoriPekerjaan kategoriPekerjaan, 
            ILowonganPekerjaan lowonganPekerjaan,
            IPerusahaan perusahaan, IUser user, 
            IDashboardAdminService dashboardAdminService,
            IValidator<KategoriPekerjaanDTO> categoryJobValidator)
        {
            _kategoriPekerjaan = kategoriPekerjaan;
            _lowonganPekerjaan = lowonganPekerjaan;
            _perusahaan = perusahaan;
            _user = user;
            _dashboardAdminService = dashboardAdminService;
            _categoryJobValidator = categoryJobValidator;
        }


        ///========================= UNTUK DASHBOARD ADMIN =====================\\\

        public IActionResult Index()
        {
            var dashboardStats = _dashboardAdminService.GetDashboardStats();
            return View(dashboardStats);
        }

        ///========================= UNTUK KATEGORI =====================\\\

        // Action baru dengan pagination dan searching
        public IActionResult KategoriPekerjaan(int? page, string searchTerm = "")
        {
            int pageNumber = page ?? 1;
            int pageSize = 5; // Jumlah item per halaman

            var kategoriList = _kategoriPekerjaan.GetListKategoriPekerjaan(pageNumber, pageSize, searchTerm);

            // Simpan searchTerm di ViewBag untuk digunakan di view
            ViewBag.SearchTerm = searchTerm;

            return View(kategoriList);
        }

        //public IActionResult KategoriPekerjaanAddUpdate(int id)
        //{
        //    var data = _kategoriPekerjaan.GetListKategoriPekerjaanById(id);
        //    return View(data);
        //}

        public IActionResult KategoriPekerjaanAddUpdate(int id)
        {
            var data = _kategoriPekerjaan.GetListKategoriPekerjaanById(id);
            return View(data); // sekarang DTO, cocok dengan view
        }


        [HttpPost]
        public async Task<IActionResult> KategoriPekerjaanAddUpdate(KategoriPekerjaanDTO kategoriPekerjaanDTO)
        {
            ValidationResult validationResult = await _categoryJobValidator.ValidateAsync(kategoriPekerjaanDTO);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(kategoriPekerjaanDTO);
            }
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
            return BadRequest("Gagal menghapus kategori.");
        }


        ///========================= UNTUK LOWONGAN =====================\\\

        public IActionResult LowonganList(int? page, string searchTerm = "")
        {
            int pageNumber = page ?? 1;
            int pageSize = 5; // Jumlah item per halaman

            var lowonganList = _lowonganPekerjaan.GetListLowonganPekerjaanForSuperAdmin(pageNumber, pageSize, searchTerm);

            // Simpan searchTerm di ViewBag untuk digunakan di view
            ViewBag.SearchTerm = searchTerm;

            return View(lowonganList);
        }

        [HttpPost]
        public IActionResult DeleteLowonganPekerjaan(int id)
        {
            var data = _lowonganPekerjaan.DeleteLowonganPekerjaanForSuperAdmin(id);
            if (data)
            {
                return RedirectToAction("LowonganList");
            }
            return BadRequest("Gagal menghapus lowongan.");
        }



        ///========================= UNTUK COMPANY =====================\\\

        public IActionResult CompanyList(int? page, string searchTerm = "")
        {
            int pageNumber = page ?? 1;
            int pageSize = 5; // Jumlah item per halaman

            var companyList = _perusahaan.GetCurrentCompanyForSuperAdmin(pageNumber, pageSize, searchTerm);

            // Simpan searchTerm di ViewBag untuk digunakan di view
            ViewBag.SearchTerm = searchTerm;

            return View(companyList);
        }

        public IActionResult UpdateCompany(int id)
        {
            var company = _perusahaan.GetCurrentCompanyForSuperAdmin()
                .FirstOrDefault(x => x.PerusahaanId == id);

            if (company == null)
            {
                return NotFound();
            }

            // Convert ke RegisterPerusahaanDTO untuk form edit
            var dto = new RegisterPerusahaanDTO
            {
                PerusahaanId = company.PerusahaanId,
                NamaPerusahaan = company.NamaPerusahaan,
                Email = company.Email,
                Telepon = company.Telepon,
                Alamat = company.Alamat,
                Kota = company.Kota,
                Provinsi = company.Provinsi,
                BidangUsaha = company.BidangUsaha,
                TanggalBerdiri = company.TanggalBerdiri,
                Status = company.Status
            };

            return View(dto);
        }

        [HttpPost]
        public IActionResult UpdateCompany(RegisterPerusahaanDTO dto)
        {
            var data = _perusahaan.UpdateCompanyForSuperAdmin(dto);
            if (data)
            {
                return RedirectToAction(nameof(CompanyList));
            }
            return View(dto);
        }

        [HttpPost]
        public IActionResult DeleteCompany(int id)
        {
            var data = _perusahaan.DeleteCompanyForSuperAdmin(id);
            if (data)
            {
                return RedirectToAction("CompanyList");
            }
            return BadRequest("Gagal menghapus Company.");
        }




        ///========================= UNTUK USER =====================\\\
        public IActionResult UserList(int? page, string searchTerm = "")
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var userList = _user.GetListUsers(pageNumber, pageSize, searchTerm);
            ViewBag.SearchTerm = searchTerm;

            return View(userList);
        }

        public IActionResult UpdateUser(int id)
        {
            var user = _user.GetListUsers()
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var dto = new UserUpdateDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Posisi = user.Posisi,
                Status = user.Status
            };

            return View(dto);
        }

        [HttpPost]
        public IActionResult UpdateUser(UserUpdateDTO dto)
        {
            var data = _user.UpdateUser(dto);
            if (data)
            {
                return RedirectToAction(nameof(UserList));
            }
            return View(dto);
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var data = _user.DeleteUser(id);
            if (data)
            {
                return RedirectToAction("UserList");
            }
            return BadRequest("Gagal menghapus User.");
        }

    }
}
