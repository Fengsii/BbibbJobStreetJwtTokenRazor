using BbibbJobStreetJwtToken.Helpers;
using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

        public PerusahaanService(ApplicationContext applicationContext, JwtHelper jwtHelper, IEnkripsiPassword enkripsiPassword, IImageHelper imageHelper, IHttpContextAccessor httpContextAccessor)
        {
            _context = applicationContext;
            _jwtHelper = jwtHelper;
            _enkripsiPassword = enkripsiPassword;
            _imageHelper = imageHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentPerusahaanId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
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

            var passwordHash = _enkripsiPassword.HashPassword(dto.Password);

            if (data == null)
            {
                return false;
            }

            // Handle upload logo jika ada
            if (dto.LogoPath != null && dto.LogoPath.Length > 0)
            {
                data.LogoPath = _imageHelper.Save(dto.LogoPath, "upload");
            }
            data.NamaPerusahaan = dto.NamaPerusahaan;
            data.Email = dto.Email;
            data.Password = passwordHash;
            data.Role = "company"; // Role default
            data.Telepon = dto.Telepon;
            data.Alamat = dto.Alamat;
            data.Kota = dto.Kota;
            data.Provinsi = dto.Provinsi;
            data.BidangUsaha = dto.BidangUsaha;
            data.TanggalBerdiri = dto.TanggalBerdiri;
            data.Status = GeneralStatus.GeneralStatusData.Active;
            data.UpdateAt = DateTime.Now;
        
            _context.Perusahaans.Update(data);
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
                var perusahaan = await _context.Perusahaans
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
