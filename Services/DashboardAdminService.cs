using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.EntityFrameworkCore;
using static BbibbJobStreetJwtToken.Models.GeneralStatus;
using static BbibbJobStreetJwtToken.Models.StatusLowongan;

namespace BbibbJobStreetJwtToken.Services
{
    public class DashboardAdminService : IDashboardAdminService
    {
        private readonly ApplicationContext _context;

        public DashboardAdminService(ApplicationContext context)
        {
            _context = context;
        }


        public DashboardAdminDTO GetDashboardStats()
        {
            var dashboard = new DashboardAdminDTO();

            // Hitung total data
            dashboard.TotalUsers = _context.Users
                .Count(u => u.Status != GeneralStatusData.Delete && u.Id != 1);

            dashboard.TotalCompanies = _context.Perusahaans
                .Count(p => p.Status != GeneralStatusData.Delete);

            dashboard.TotalJobVacancies = _context.LowonganPekerjaans
                .Count(l => l.status != StatusLowonganPekerjaan.Delete);

            dashboard.TotalActiveJobVacancies = _context.LowonganPekerjaans
                .Count(l => l.status == StatusLowonganPekerjaan.Active);

            dashboard.TotalClosedJobVacancies = _context.LowonganPekerjaans
                .Count(l => l.status == StatusLowonganPekerjaan.Closed);

            // Hitung data minggu ini
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            dashboard.UsersThisWeek = _context.Users
                .Count(u => u.CreatedAt >= startOfWeek && u.Status != GeneralStatusData.Delete);

            dashboard.CompaniesThisWeek = _context.Perusahaans
                .Count(p => p.CreateAt >= startOfWeek && p.Status != GeneralStatusData.Delete);

            dashboard.JobsThisWeek = _context.LowonganPekerjaans
                .Count(l => l.TanggalDibuat >= startOfWeek && l.status != StatusLowonganPekerjaan.Delete);

            // Hitung rasio lowongan per user
            dashboard.JobToUserRatio = dashboard.TotalUsers > 0 ?
                Math.Round((decimal)dashboard.TotalJobVacancies / dashboard.TotalUsers, 2) : 0;

            // Data statistik bulanan (6 bulan terakhir)
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            dashboard.MonthlyStats = GetMonthlyStats(sixMonthsAgo);

            // Statistik kategori pekerjaan
            dashboard.CategoryStats = GetCategoryStats();

            // Aktivitas terbaru
            dashboard.RecentActivities = GetRecentActivities();

            return dashboard;
        }

        private List<ActivityDTO> GetRecentActivities()
        {
            var activities = new List<ActivityDTO>();

            // Ambil lowongan terbaru
            var recentJobs = _context.LowonganPekerjaans
                .Include(l => l.Perusahaan)
                .Where(l => l.status != StatusLowonganPekerjaan.Delete)
                .OrderByDescending(l => l.TanggalDibuat)
                .Take(5)
                .ToList();

            foreach (var job in recentJobs)
            {
                activities.Add(new ActivityDTO
                {
                    Icon = "fas fa-briefcase",
                    Title = $"Lowongan baru: {job.Judul}",
                    Time = job.TanggalDibuat.ToString("dd MMM yyyy")
                });
            }

            // Ambil perusahaan terdaftar terbaru
            var recentCompanies = _context.Perusahaans
                .Where(p => p.Status != GeneralStatusData.Delete)
                .OrderByDescending(p => p.CreateAt)
                .Take(3)
                .ToList();

            foreach (var company in recentCompanies)
            {
                activities.Add(new ActivityDTO
                {
                    Icon = "fas fa-building",
                    Title = $"Perusahaan bergabung: {company.NamaPerusahaan}",
                    Time = company.CreateAt.ToString("dd MMM yyyy")
                });
            }

            return activities.OrderByDescending(a => a.Time).Take(5).ToList();
        }


        public List<MonthlyStatDTO> GetMonthlyStats(DateTime startDate)
        {
            var monthlyStats = new List<MonthlyStatDTO>();

            for (int i = 0; i < 6; i++)
            {
                var currentMonth = DateTime.Now.AddMonths(-i);
                var monthName = currentMonth.ToString("MMM yyyy");

                var users = _context.Users
                    .Count(u => u.CreatedAt.Month == currentMonth.Month &&
                               u.CreatedAt.Year == currentMonth.Year &&
                               u.Status != GeneralStatusData.Delete);

                var companies = _context.Perusahaans
                    .Count(p => p.CreateAt.Month == currentMonth.Month &&
                               p.CreateAt.Year == currentMonth.Year &&
                               p.Status != GeneralStatusData.Delete);

                var jobVacancies = _context.LowonganPekerjaans
                    .Count(l => l.TanggalDibuat.Month == currentMonth.Month &&
                               l.TanggalDibuat.Year == currentMonth.Year &&
                               l.status != StatusLowongan.StatusLowonganPekerjaan.Delete);

                monthlyStats.Add(new MonthlyStatDTO
                {
                    Month = monthName,
                    Users = users,
                    Companies = companies,
                    JobVacancies = jobVacancies
                });
            }

            return monthlyStats.OrderBy(m => m.Month).ToList();
        }

        public List<CategoryStatDTO> GetCategoryStats()
        {
            return _context.KategoriPekerjaans
                .Include(k => k.Lowongans)
                .Where(k => k.Lowongans.Any(l => l.status != StatusLowonganPekerjaan.Delete)) // Filter kategori yang punya lowongan aktif
                .Select(k => new CategoryStatDTO
                {
                    CategoryName = k.NamaKategori,
                    JobCount = k.Lowongans.Count(l => l.status != StatusLowonganPekerjaan.Delete)
                })
                .OrderByDescending(c => c.JobCount)
                .Take(10)// Ambil 10 kategori teratas
                .ToList();
        }

       

    }
}
