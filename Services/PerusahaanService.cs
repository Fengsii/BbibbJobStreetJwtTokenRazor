using BbibbJobStreetJwtToken.Helpers;
using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;
using static BbibbJobStreetJwtToken.Models.GeneralStatus;
using static System.Net.Mime.MediaTypeNames;

namespace BbibbJobStreetJwtToken.Services
{
    public class PerusahaanService : IPerusahaan
    {
        private readonly ApplicationContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IEnkripsiPassword _enkripsiPassword;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailHelper _emailHelper;
        private readonly ILogger<PerusahaanService> _logger;

        public PerusahaanService(ApplicationContext applicationContext, JwtHelper jwtHelper, IEnkripsiPassword enkripsiPassword, IImageHelper imageHelper, IHttpContextAccessor httpContextAccessor, IEmailHelper emailHelper, ILogger<PerusahaanService> logger)
        {
            _context = applicationContext;
            _jwtHelper = jwtHelper;
            _enkripsiPassword = enkripsiPassword;
            _imageHelper = imageHelper;
            _httpContextAccessor = httpContextAccessor;
            _emailHelper = emailHelper;
            _logger = logger;
        }

        private int GetCurrentPerusahaanId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }




        public async Task<bool> SendOtpAsync(string emailOrUsername)
        {
            try
            {
                emailOrUsername = emailOrUsername.Trim().ToLower();
                Console.WriteLine($"[SEND OTP] EmailOrUsername masuk: '{emailOrUsername}'");

                var user = await _context.Perusahaans
                    .FirstOrDefaultAsync(x =>
                        x.Email.ToLower() == emailOrUsername || x.NamaPerusahaan.ToLower() == emailOrUsername);


                if (user == null)
                {
                    Console.WriteLine("[SEND OTP] User tidak ditemukan di DB.");
                    return false;
                }
                else
                {
                    Console.WriteLine($"[SEND OTP] User ditemukan: {user.NamaPerusahaan}, {user.Email}");
                }



                // Hapus OTP lama jika sudah expired
                if (user.OtpExpiredAt != null && user.OtpExpiredAt < DateTime.Now)
                {
                    user.OtpCode = null;
                    user.OtpExpiredAt = null;
                }

                // Generate OTP baru
                var otp = new Random().Next(100000, 999999).ToString();
                user.OtpCode = otp;
                user.OtpExpiredAt = DateTime.Now.AddMinutes(10);

                await _context.SaveChangesAsync();

                // Kirim OTP ke email user
                var sent = await _emailHelper.SendOtpEmailAsync(user.Email, otp);
                return sent;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Terjadi kesalahan saat menyimpan OTP ke database.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan umum saat mengirim OTP.");
                return false;
            }
        }

        public async Task<bool> ResetPasswordWithOtpAsync(VerifyOtpAndResetPasswordDTO dto)
        {
            try
            {
                var user = await _context.Perusahaans.FirstOrDefaultAsync(u =>
                    (u.Email.ToLower() == dto.EmailOrUsername.ToLower() || u.NamaPerusahaan.ToLower() == dto.EmailOrUsername.ToLower())
                    && u.Status != GeneralStatusData.Delete && u.Status != GeneralStatusData.Unactive);

                if (user == null)
                    return false;

                if (user.OtpCode != dto.OtpCode)
                    return false;

                if (user.OtpExpiredAt == null || user.OtpExpiredAt < DateTime.Now)
                    return false;

                user.Password = _enkripsiPassword.HashPassword(dto.NewPassword);

                // Hapus OTP setelah berhasil reset password
                user.OtpCode = null;
                user.OtpExpiredAt = null;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Terjadi kesalahan saat menyimpan password baru ke database.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan umum saat mereset password.");
                return false;
            }
        }





        public List<CompanyViewDTO> GetListCompany()
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.Perusahaans.Where(x => x.Status != GeneralStatus.GeneralStatusData.Delete && x.Id == perusahaanId).Select(x => new CompanyViewDTO
            {
                PerusahaanId = x.Id,
                LogoPath = x.LogoPath,
                NamaPerusahaan = x.NamaPerusahaan,
                Email = x.Email,
                Password = "********",
                Role = x.Role,
                Telepon = x.Telepon,
                Alamat = x.Alamat,
                Kota = x.Kota,
                Provinsi = x.Provinsi,
                BidangUsaha = x.BidangUsaha,
                TanggalBerdiri = x.TanggalBerdiri,
                Status = x.Status,
            }).ToList();
            return data;

        }

        public CompanyViewDTO GetCurrentCompany()
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.Perusahaans
                .Where(x => x.Status != GeneralStatus.GeneralStatusData.Delete && x.Id == perusahaanId)
                .Select(x => new CompanyViewDTO
                {
                    PerusahaanId = x.Id,
                    LogoPath = "/upload/" + Path.GetFileName(x.LogoPath),
                    NamaPerusahaan = x.NamaPerusahaan,
                    Email = x.Email,
                    Password = "********",
                    Role = x.Role,
                    Telepon = x.Telepon,
                    Alamat = x.Alamat,
                    Kota = x.Kota,
                    Provinsi = x.Provinsi,
                    BidangUsaha = x.BidangUsaha,
                    TanggalBerdiri = x.TanggalBerdiri,
                    Status = x.Status,
                })
                .FirstOrDefault();

            return data ?? new CompanyViewDTO();
        }


        public IPagedList<CompanyViewDTO> GetCurrentCompanyForSuperAdmin(int page, int pageSize, string searchTerm = "")
        {
            var query = _context.Perusahaans
                .Where(x => x.Status != GeneralStatus.GeneralStatusData.Delete)
                .Select(x => new CompanyViewDTO
                {
                    PerusahaanId = x.Id,
                    LogoPath = "/upload/" + Path.GetFileName(x.LogoPath),
                    NamaPerusahaan = x.NamaPerusahaan,
                    Email = x.Email,
                    Password = "********",
                    Role = x.Role,
                    Telepon = x.Telepon,
                    Alamat = x.Alamat,
                    Kota = x.Kota,
                    Provinsi = x.Provinsi,
                    BidangUsaha = x.BidangUsaha,
                    TanggalBerdiri = x.TanggalBerdiri,
                    Status = x.Status,
                });

            // Tambahkan pencarian
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x =>
                    x.NamaPerusahaan.Contains(searchTerm) ||
                    x.BidangUsaha.Contains(searchTerm) ||
                    x.Alamat.Contains(searchTerm) ||
                    x.Kota.Contains(searchTerm) ||
                    x.Provinsi.Contains(searchTerm));
            }
            // Return dengan pagination
            return query.OrderBy(x => x.NamaPerusahaan)
                        .ToPagedList(page, pageSize);
        }


        public List<CompanyViewDTO> GetCurrentCompanyForSuperAdmin()
        {

            var data = _context.Perusahaans
                .Where(x => x.Status != GeneralStatus.GeneralStatusData.Delete)
                .Select(x => new CompanyViewDTO
                {
                    PerusahaanId = x.Id,
                    LogoPath = "/upload/" + Path.GetFileName(x.LogoPath),
                    NamaPerusahaan = x.NamaPerusahaan,
                    Email = x.Email,
                    Password = "********",
                    Role = x.Role,
                    Telepon = x.Telepon,
                    Alamat = x.Alamat,
                    Kota = x.Kota,
                    Provinsi = x.Provinsi,
                    BidangUsaha = x.BidangUsaha,
                    TanggalBerdiri = x.TanggalBerdiri,
                    Status = x.Status,
                }).ToList();

            return data;
        }


        public Perusahaan GetCompanyById(int id)
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.Perusahaans
                .Where(x => x.Id == id &&
                          x.Status != GeneralStatus.GeneralStatusData.Delete &&
                          x.Id == perusahaanId)
                .FirstOrDefault();

            return data ?? new Perusahaan();
        }

        public bool UpdateCompany(RegisterPerusahaanDTO dto)
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.Perusahaans
                .FirstOrDefault(x => x.Id == dto.PerusahaanId && x.Id == perusahaanId);

            if (data == null)
            {
                return false;
            }

            // Update logo kalau ada file baru
            if (dto.LogoPath != null && dto.LogoPath.Length > 0)
            {
                data.LogoPath = _imageHelper.Save(dto.LogoPath, "upload");
            }

            data.NamaPerusahaan = dto.NamaPerusahaan;
            data.Email = dto.Email;
            data.Telepon = dto.Telepon;
            data.Alamat = dto.Alamat;
            data.Kota = dto.Kota;
            data.Provinsi = dto.Provinsi;
            data.BidangUsaha = dto.BidangUsaha;
            data.TanggalBerdiri = dto.TanggalBerdiri;
            data.Status = GeneralStatus.GeneralStatusData.Active;
            data.Role = "company"; // role default
            data.UpdateAt = DateTime.Now;

            // Update password hanya kalau user isi password baru
            if (!string.IsNullOrEmpty(dto.Password))
            {
                data.Password = _enkripsiPassword.HashPassword(dto.Password);
            }

            _context.Perusahaans.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateCompanyForSuperAdmin(RegisterPerusahaanDTO dto)
        {

            var data = _context.Perusahaans
                .FirstOrDefault(x => x.Id == dto.PerusahaanId);

            if (data == null)
            {
                return false;
            }

            data.Status = dto.Status;

            _context.Perusahaans.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteCompanyForSuperAdmin(int id)
        {
            var data = _context.Perusahaans
                .FirstOrDefault(x => x.Id == id);

            if (data == null)
            {
                return false;
            }

            data.Status = GeneralStatus.GeneralStatusData.Delete;
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> PerusahaanExistsAsync(string name, string email)
        {
            return await _context.Perusahaans
                .AnyAsync(u => u.NamaPerusahaan == name || u.Email == email);
        }

        public async Task<string?> LoginAsync(LoginPerusahaanDTO loginPerusahaanDTO)
        {
            try
            {
                var perusahaan = await _context.Perusahaans.Where(u => u.Status != GeneralStatusData.Unactive && u.Status != GeneralStatusData.Delete)
                    .FirstOrDefaultAsync(u => u.NamaPerusahaan == loginPerusahaanDTO.NamaPerusahaan);

                if (perusahaan == null)
                    return null;

                // Verifikasi password
                if (!_enkripsiPassword.VerifyPassword(loginPerusahaanDTO.Password, perusahaan.Password))
                    return null; // Password salah

                var token = _jwtHelper.GenerateToken(perusahaan.NamaPerusahaan, perusahaan.Email, perusahaan.Id, perusahaan.Role);
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoginAsync Error] {ex}");
                throw new Exception("Terjadi kesalahan saat login", ex);
            }
        }

       
        public async Task<bool> RegisterAsync(RegisterPerusahaanDTO model)
        {
            // Mulai transaksi database
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var passwordHash = _enkripsiPassword.HashPassword(model.Password);

                var perusahaan = new Perusahaan
                {
                    NamaPerusahaan = model.NamaPerusahaan,
                    Email = model.Email,
                    Password = passwordHash,
                    Telepon = model.Telepon,
                    Alamat = model.Alamat,
                    Role = "company", // Role default
                    Kota = model.Kota,
                    Provinsi = model.Provinsi,
                    BidangUsaha = model.BidangUsaha,
                    TanggalBerdiri = model.TanggalBerdiri,
                    Status = GeneralStatus.GeneralStatusData.Active,
                    CreateAt = DateTime.Now
                };

                // Handle upload logo jika ada
                if (model.LogoPath != null && model.LogoPath.Length > 0)
                {
                    perusahaan.LogoPath = _imageHelper.Save(model.LogoPath, "upload");
                }

                _context.Perusahaans.Add(perusahaan);
                await _context.SaveChangesAsync();

                // Commit transaksi jika sukses
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[Error Register] {ex}");
                throw;
            }
        }

        public List<SelectListItem> Perusahaan()
        {
            var datas = _context.Perusahaans
                .Where(x => x.Status == GeneralStatus.GeneralStatusData.Active)
                .Select(x => new SelectListItem
                {
                    Text = x.NamaPerusahaan,
                    Value = x.Id.ToString()
                }).ToList();

            return datas;
        }

    }
}
