using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BbibbJobStreetJwtToken.Services
{
    public class CompanyDashboardService : ICompanyDashboard
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyDashboardService(ApplicationContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentPerusahaanId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        public CompanyDashboard GetDashboardData()
        {
            var perusahaanId = GetCurrentPerusahaanId();

            // Hitung total lowongan milik perusahaan
            var totalLowongan = _context.LowonganPekerjaans
                .Count(x => x.PerusahaanId == perusahaanId && x.status != StatusLowongan.StatusLowonganPekerjaan.Delete);

            // Hitung semua lamaran untuk perusahaan
            var lamarans = _context.Lamarans
                .Include(l => l.Lowongan)
                .Where(l => l.Lowongan.PerusahaanId == perusahaanId);

                //.Include(l => l.Lowongan)
                //.Where(l => l.Lowongan.PerusahaanId == perusahaanId);

            var pelamarAktif = lamarans.Count(l => l.Status == Models.StatusLamaran.DataStatusLamaran.Diproses);
            var lamaranBaru = lamarans.Count(l => l.TanggalDilamar.Date == DateTime.Today);
            var lamaranHariIni = lamaranBaru; // sama dengan di atas
            var lamaranMingguIni = lamarans.Count(l => l.TanggalDilamar >= DateTime.Today.AddDays(-7));
            var lamaranDiterima = lamarans.Count(l => l.Status == Models.StatusLamaran.DataStatusLamaran.Diterima);

            // Lowongan terbaru (ambil Id atau jumlah dalam 7 hari terakhir misalnya)
            var lowonganTerbaru = _context.LowonganPekerjaans
                .Where(x => x.PerusahaanId == perusahaanId)
                .OrderByDescending(x => x.TanggalDibuat)
                .Select(x => x.Id)
                .FirstOrDefault();

            // Statistik bulanan (contoh: jumlah lamaran bulan ini)
            var statistikBulanan = lamarans.Count(l => l.TanggalDilamar.Month == DateTime.Now.Month).ToString();

            return new CompanyDashboard
            {
                CompanyId = perusahaanId,
                TotalLowongan = totalLowongan,
                PelamarAktif = pelamarAktif,
                LamaranBaru = lamaranBaru,
                StatistikBulanan = statistikBulanan,
                LowonganTerbaru = lowonganTerbaru,
                LamaranHariIni = lamaranHariIni,
                LamaranMinggIni = lamaranMingguIni,
                LamaranDiterima = lamaranDiterima
            };
        }
    }
 
}
