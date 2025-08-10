using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static BbibbJobStreetJwtToken.Models.StatusLamaran;

namespace BbibbJobStreetJwtToken.Services
{
    public class LowonganTersimpanService : ILowonganTersimpan
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LowonganTersimpanService(ApplicationContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<LowonganTersimpanViewDTO> GetListLowonganTersimpan()
        {
            var userId = GetCurrentUserId();
            var data = _context.LowonganTersimpans.Include(y => y.Lowongan).Where(x => x.PenggunaId == userId)
                        .Select(x => new LowonganTersimpanViewDTO
                        {
                            Id = x.Id,
                            Posisi = x.Lowongan.Posisi,
                            Deskripsi = x.Lowongan.Deskripsi,
                            TanggalDibuat = x.Lowongan.TanggalDibuat,
                        }).ToList();

            return data;

        }

        public LowonganTersimpan GetLowonganTersimpanById(int id)
        {
            var data = _context.LowonganTersimpans.FirstOrDefault();
            if (data == null)
            {
                return new LowonganTersimpan();
            }

            return data;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
        public async Task<bool> AddLowonganTersimpan(LowonganTersimpanAddUpdateDTO dto, int lowonganId)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                throw new UnauthorizedAccessException("User belum login.");

            var lowongan = new LowonganTersimpan
            {
                PenggunaId = userId,
                LowonganId = lowonganId,
                TanggalDisimpan = DateTime.Now,
            };

            _context.LowonganTersimpans.Add(lowongan);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool DeleteLowonganTersimpan(int id)
        {
            var data = _context.LowonganTersimpans.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }

            _context.LowonganTersimpans.Remove(data);
            _context.SaveChanges();
            return true;
        }

    }
}
