using BbibbJobStreetJwtToken.Helpers;
using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace BbibbJobStreetJwtToken.Services
{
    public class PerusahaanService : IPerusahaan
    {
        private readonly ApplicationContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IEnkripsiPassword _enkripsiPassword;

        public PerusahaanService(ApplicationContext applicationContext, JwtHelper jwtHelper, IEnkripsiPassword enkripsiPassword, IImageHelper imageHelper)
        {
            _context = applicationContext;
            _jwtHelper = jwtHelper;
            _enkripsiPassword = enkripsiPassword;
            _imageHelper = imageHelper;
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
