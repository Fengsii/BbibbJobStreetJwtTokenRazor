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

        public CompanyDashboardDTO GetDashboardData()
        {
            var perusahaanId = GetCurrentPerusahaanId();

            // Hitung total lowongan milik perusahaan
            var totalLowongan = _context.LowonganPekerjaans
                .Count(x => x.PerusahaanId == perusahaanId && x.status != StatusLowongan.StatusLowonganPekerjaan.Delete);


            // Ambil data lamaran sesuai perusahaan
            var lamarans = _context.Lamarans
                .Include(l => l.Lowongan)
                .Where(l => l.Lowongan.PerusahaanId == perusahaanId);

            // Hitung semua lamaran untuk perusahaan
            var pelamarAktif = lamarans.Count(l => l.Status == Models.StatusLamaran.DataStatusLamaran.Diproses);
            var lamaranBaru = lamarans.Count(l => l.TanggalDilamar.Date == DateTime.Today);
            var lamaranHariIni = lamaranBaru; // sama dengan di atas
            var lamaranMingguIni = lamarans.Count(l => l.TanggalDilamar >= DateTime.Today.AddDays(-7));
            var lamaranDiterima = lamarans.Count(l => l.Status == Models.StatusLamaran.DataStatusLamaran.Diterima);

            // Lowongan terbaru = jumlah lowongan yang dibuat dalam 1 jam terakhir oleh perusahaan yang sedang login
            var lowonganTerbaru = _context.LowonganPekerjaans
                .Where(x => x.PerusahaanId == perusahaanId
                         && x.TanggalDibuat >= DateTime.UtcNow.AddHours(-1)
                         && x.status != StatusLowongan.StatusLowonganPekerjaan.Delete)
                .Count();

            // Statistik bulanan (contoh: jumlah lamaran bulan ini)
            var statistikBulanan = lamarans.Count(l => l.TanggalDilamar.Month == DateTime.Now.Month).ToString();

            //Menampilkan 5 Lowongan Pekerjaan Terbaru
            var daftarLowonganTerbaru = _context.LowonganPekerjaans
                .Include(x => x.Perusahaan)
                .Where(x => x.PerusahaanId == perusahaanId
                         && x.status != StatusLowongan.StatusLowonganPekerjaan.Delete)
                .OrderByDescending(x => x.TanggalDibuat).Take(5)
                .Select(x => new LowonganPekerjaanViewDTO
                {
                    Id = x.Id, 
                    Logo = x.Logo,
                    Judul = x.Judul,
                    Alamat = x.Alamat,
                    Posisi = x.Posisi,
                    Deskripsi = x.Deskripsi,
                    KategoriId = x.KategoriId,
                    PerusahaanId = perusahaanId,
                    NamaPerusahaan = x.Perusahaan.NamaPerusahaan,
                    TanggalDibuat = x.TanggalDibuat,
                    status = x.status,  
                }).ToList();
           
            // -------- Grafik Harian (7 hari terakhir) --------

            var grafikLamaran = lamarans
                .Where(l => l.TanggalDilamar >= DateTime.Today.AddDays(-6))
                .GroupBy(l => l.TanggalDilamar.Date)
                .Select(g => new
                {
                    Tanggal = g.Key,
                    Jumlah = g.Count()
                })
                .ToList() // <-- eksekusi dulu di DB
                .Select(g => new LamaranChartDTO
                {
                    Tanggal = g.Tanggal.ToString("dd MMM"), // format di C#
                    Jumlah = g.Jumlah
                })
                .OrderBy(g => DateTime.ParseExact(g.Tanggal, "dd MMM", null))
                .ToList();



            return new CompanyDashboardDTO
            {
                CompanyId = perusahaanId,
                TotalLowongan = totalLowongan,
                PelamarAktif = pelamarAktif,
                LamaranBaru = lamaranBaru,
                StatistikBulanan = statistikBulanan,
                LowonganTerbaru = lowonganTerbaru,
                LamaranHariIni = lamaranHariIni,
                LamaranMinggIni = lamaranMingguIni,
                LamaranDiterima = lamaranDiterima,
                DaftarLowonganTerbaru = daftarLowonganTerbaru,
                GrafikLamaran = grafikLamaran
            };
        }
    }
 
}
