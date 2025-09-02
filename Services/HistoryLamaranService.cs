using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static BbibbJobStreetJwtToken.Models.StatusLamaran;

namespace BbibbJobStreetJwtToken.Services
{
    public class HistoryLamaranService : IHistoryLamaran
    {
        public readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HistoryLamaranService(ApplicationContext applicationContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = applicationContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
        public List<HistoryLamaranDTO> GetListHistoryLowongan()
        {
            var userId = GetCurrentUserId();
            var data = _context.Lamarans.Include(y => y.Lowongan).ThenInclude(p => p.Perusahaan).Where(x => x.UserId == userId && !x.IsDeleted)
                        .Select(x => new HistoryLamaranDTO
                        {
                            Id = x.Id,
                            Logo = "/upload/" + Path.GetFileName(x.Lowongan.Perusahaan.LogoPath),
                            Posisi = x.Lowongan.Posisi,
                            Deskripsi = x.Lowongan.Deskripsi,
                            TanggalDilamar = x.TanggalDilamar,
                            Status = x.Status,
                        }).ToList();

            return data;

        }

        public bool DeletelamaranTersimpan(int id)
        {
            var lamaran = _context.Lamarans.FirstOrDefault(x => x.Id == id);
            if (lamaran != null)
            {
                lamaran.IsDeleted = true; // Tandai sebagai terhapus
                _context.SaveChanges();
                return true;
            }
            return false;
        }


    }
}
