using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static BbibbJobStreetJwtToken.Models.GeneralStatus;

namespace BbibbJobStreetJwtToken.Services
{
    public class LowonganPekerjaanService : ILowonganPekerjaan
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LowonganPekerjaanService(ApplicationContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        private int GetCurrentPerusahaanId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        public List<LowonganPekerjaanViewDTO> GetListLowonganPekerjaan()
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.LowonganPekerjaans
                .Include(y => y.Kategori)
                .Include(y => y.Perusahaan)
                .Where(x => x.status != StatusLowongan.StatusLowonganPekerjaan.Delete &&
                           x.PerusahaanId == perusahaanId)
                .Select(x => new LowonganPekerjaanViewDTO
                {
                    Id = x.Id,
                    Logo = !string.IsNullOrEmpty(x.Perusahaan.LogoPath) ? "/" + x.Perusahaan.LogoPath : null,
                    Judul = x.Judul,
                    Alamat = x.Alamat,
                    Posisi = x.Posisi,
                    Deskripsi = x.Deskripsi,
                    NamaKategori = x.Kategori.NamaKategori,
                    NamaPerusahaan = x.Perusahaan.NamaPerusahaan,
                    TanggalDibuat = x.TanggalDibuat,
                    status = x.status,
                }).ToList();
            return data;

        }

        public List<LowonganPekerjaanViewDTO> GetListLowonganPekerjaanForUser()
        {
            var data = _context.LowonganPekerjaans.Include(y => y.Kategori).Include(y => y.Perusahaan).Where(x => x.status != StatusLowongan.StatusLowonganPekerjaan.Delete).Select(x => new LowonganPekerjaanViewDTO
            {
                Id = x.Id,
                Logo = !string.IsNullOrEmpty(x.Perusahaan.LogoPath) ? "/" + x.Perusahaan.LogoPath : null,
                Judul = x.Judul,
                Alamat = x.Alamat,
                Posisi = x.Posisi,
                Deskripsi = x.Deskripsi,
                NamaKategori = x.Kategori.NamaKategori,
                NamaPerusahaan = x.Perusahaan.NamaPerusahaan,
                TanggalDibuat = x.TanggalDibuat,
                status = x.status,

            }).ToList();
            return data;

        }


        public LowonganPekerjaan GetLowonganPekerjaanById(int id)
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.LowonganPekerjaans
                .Where(x => x.Id == id && 
                          x.status != StatusLowongan.StatusLowonganPekerjaan.Delete &&
                          x.PerusahaanId == perusahaanId)
                .FirstOrDefault();

            return data ?? new LowonganPekerjaan();
        }


        //============================== BATASAN =========================\\

        public bool AddLowonganPekerjaan(LowonganPekerjaanAddUpdateDTO request)
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var perusahaan = _context.Perusahaans.FirstOrDefault(x => x.Id == perusahaanId);

            if (perusahaan == null)
            {
                return false;
            }

            var data = new LowonganPekerjaan
            {
                Judul = request.Judul,
                Logo = perusahaan.LogoPath,
                Alamat = request.Alamat,
                Posisi = request.Posisi,
                Deskripsi = request.Deskripsi,
                TanggalDibuat = DateTime.UtcNow,
                status = request.status,
                KategoriId = request.KategoriId,
                PerusahaanId = perusahaanId
            };

            _context.LowonganPekerjaans.Add(data);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateLowonganPekerjaan(LowonganPekerjaanAddUpdateDTO lowonganPekerjaanAddUpdateDTO)
        {

            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.LowonganPekerjaans
                .FirstOrDefault(x => x.Id == lowonganPekerjaanAddUpdateDTO.Id &&
                                   x.PerusahaanId == perusahaanId);

            if (data == null)
            {
                return false;
            }

            data.Judul = lowonganPekerjaanAddUpdateDTO.Judul;
            data.Posisi = lowonganPekerjaanAddUpdateDTO.Posisi;
            data.Alamat = lowonganPekerjaanAddUpdateDTO.Alamat;
            data.Deskripsi = lowonganPekerjaanAddUpdateDTO.Deskripsi;
            data.TanggalDibuat = lowonganPekerjaanAddUpdateDTO.TanggalDibuat;
            data.status = lowonganPekerjaanAddUpdateDTO.status;
            data.KategoriId = lowonganPekerjaanAddUpdateDTO.KategoriId;

            _context.LowonganPekerjaans.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteLowonganPekerjaan(int id)
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.LowonganPekerjaans
                .FirstOrDefault(x => x.Id == id && x.PerusahaanId == perusahaanId);

            if (data == null)
            {
                return false;
            }

            data.status = StatusLowongan.StatusLowonganPekerjaan.Delete;
            _context.SaveChanges();
            return true;
        }

        public List<NotificationsDTO> GetNotificationsForUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return new List<NotificationsDTO>();

            var data = _context.LowonganPekerjaans
                .Include(l => l.Perusahaan)
                .Where(l => EF.Functions.Like(l.Posisi.ToLower(), "%" + user.Posisi.ToLower() + "%")
                    && l.TanggalDibuat >= DateTime.UtcNow.AddDays(-7) && !l.IsDeleted)
                .OrderByDescending(l => l.TanggalDibuat)
                .Select(l => new NotificationsDTO
                {   
                    Id = l.Id,
                    Logo = "/upload/" + Path.GetFileName(l.Perusahaan.LogoPath),
                    Posisi = l.Posisi,
                    Deskripsi = l.Deskripsi,
                    NamaPerusahaan = l.Perusahaan.NamaPerusahaan,
                    TanggalDibuat = l.TanggalDibuat
                })
                .ToList();

            return data;
        }

        public bool DeleteNotifications(int id)
        {
            var lowongan = _context.LowonganPekerjaans.FirstOrDefault(x => x.Id == id);
            if (lowongan != null)
            {
                lowongan.IsDeleted = true; // Tandai sebagai terhapus
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<LowonganPekerjaanViewDTO> SearchLowonganPekerjaan(string keyword)
        {
            // Convert keyword to lowercase for case-insensitive search
            var searchTerm = keyword?.ToLower() ?? string.Empty;

            var query = _context.LowonganPekerjaans
                .Include(y => y.Kategori)
                .Include(y => y.Perusahaan)
                .Where(x => x.status != StatusLowongan.StatusLowonganPekerjaan.Delete)
                .AsQueryable();

            // Jika ada keyword, lakukan pencarian
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x =>
                    x.Posisi.ToLower().Contains(searchTerm) ||
                    x.Perusahaan.NamaPerusahaan.ToLower().Contains(searchTerm) ||
                    x.Kategori.NamaKategori.ToLower().Contains(searchTerm) ||
                    x.Alamat.ToLower().Contains(searchTerm)
                );
            }

            var result = query.Select(x => new LowonganPekerjaanViewDTO
            {
                Id = x.Id,
                Logo = !string.IsNullOrEmpty(x.Perusahaan.LogoPath) ? "/" + x.Perusahaan.LogoPath : null,
                Judul = x.Judul,
                Alamat = x.Alamat,
                Posisi = x.Posisi,
                Deskripsi = x.Deskripsi,
                NamaKategori = x.Kategori.NamaKategori,
                NamaPerusahaan = x.Perusahaan.NamaPerusahaan,
                TanggalDibuat = x.TanggalDibuat,
                status = x.status,
            }).ToList();

            return result;
        }


    }
}
