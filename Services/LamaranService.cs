using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;
using static BbibbJobStreetJwtToken.Models.StatusLamaran;

namespace BbibbJobStreetJwtToken.Services
{
    public class LamaranService : ILamaran
    {
        public readonly ApplicationContext _context;
        private readonly IFileHelper _fileHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailHelper _emailHelper;
        public LamaranService(ApplicationContext applicationContext, IFileHelper fileHelper, IHttpContextAccessor httpContextAccessor, IEmailHelper emailHelper)
        {
            _context = applicationContext;
            _fileHelper = fileHelper;
            _httpContextAccessor = httpContextAccessor;
            _emailHelper = emailHelper;
        }

        ///==================== UNTUK PERUSAHAAN ================\\\
        private int GetCurrentPerusahaanId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
        
        public IPagedList<LamaranViewDTO> GetListLamaran(int page, int pageSize, string searchTerm = "")
        {
            var perusahaanId = GetCurrentPerusahaanId();
            if (perusahaanId == 0)
                return new List<LamaranViewDTO>().ToPagedList(page, pageSize);

            var query = _context.Lamarans
                .Include(u => u.user)
                .Include(l => l.Lowongan)
                .ThenInclude(p => p.Perusahaan)
                .Where(x => x.Lowongan.PerusahaanId == perusahaanId)
                .OrderByDescending(x => x.TanggalDilamar)
                .Select(x => new LamaranViewDTO
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    LowonganId = x.LowonganId,
                    Nama = x.Nama,
                    Email = x.Email,
                    NoHP = x.NoHP,
                    Pendidikan = x.Pendidikan,
                    GajiSaatIni = x.GajiSaatIni,
                    GajiDiharapkan = x.GajiDiharapkan,
                    CV = x.CV,
                    Status = x.Status,
                    TanggalDilamar = x.TanggalDilamar,
                });


            // Tambahkan filter pencarian

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x =>
                    x.Nama.Contains(searchTerm) ||
                    x.Email.Contains(searchTerm) ||
                    x.NoHP.Contains(searchTerm));
            }

            return query.ToPagedList(page, pageSize);
        }

       
        public Lamaran GetLamaranById(int id)
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.Lamarans.Include(l => l.Lowongan)
                .ThenInclude(p => p.Perusahaan)
                .Where(x => x.Lowongan.PerusahaanId == perusahaanId && x.Id == id)
                .FirstOrDefault();

            if (data == null)
            {
                return new Lamaran();
            }

            return data;
        }


        public bool UpdateLamaran(LamaranAddUpdateDTO lamaranAddUpdateDTO)
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.Lamarans
                .Include(l => l.Lowongan)
                .ThenInclude(p => p.Perusahaan)
                .FirstOrDefault(x => x.Lowongan.PerusahaanId == perusahaanId && x.Id == lamaranAddUpdateDTO.Id);

            if (data == null)
                return false;

            var oldStatus = data.Status;
            data.Status = lamaranAddUpdateDTO.Status;

            _context.Lamarans.Update(data);
            _context.SaveChanges();

            // hanya kirim kalau status berubah
            if (oldStatus != data.Status)
            {
                var perusahaan = data.Lowongan.Perusahaan;
                var lowongan = data.Lowongan;

                string subject = $"Update Status Lamaran - {perusahaan.NamaPerusahaan}";
                //string logoUrl = "/upload/" + Path.GetFileName(perusahaan.LogoPath); // pastikan ini URL/relative path yang bisa diakses
                string bodyHtml = "";

                if (data.Status == StatusLamaran.DataStatusLamaran.Diterima)
                {
                    bodyHtml = $@"
            <div style='font-family:Arial,sans-serif; color:#333; max-width:600px; margin:auto; border:1px solid #ddd; border-radius:8px; overflow:hidden;'>
                <div style='background:#f8f9fa; padding:20px; text-align:center;'>
                    <h2 style='color:#007bff;'>Selamat! Lamaran Anda Diterima 🎉</h2>
                </div>
                <div style='padding:20px;'>
                    <p>Halo <b>{data.Nama}</b>,</p>
                    <p>Dengan senang hati kami informasikan bahwa lamaran Anda untuk posisi <b>{lowongan.Posisi}</b> di <b>{perusahaan.NamaPerusahaan}</b> telah <span style='color:green;font-weight:bold;'>DITERIMA</span>.</p>
                    <p>Tim kami akan segera menghubungi Anda terkait proses berikutnya.</p>
                    <p>Terima kasih telah melamar di <b>{perusahaan.NamaPerusahaan}</b>.</p>
                    <p>Hormat kami,<br/><b>{perusahaan.NamaPerusahaan}</b></p>
                </div>
            </div>";
                }
                else if (data.Status == StatusLamaran.DataStatusLamaran.Ditolak)
                {
                    bodyHtml = $@"
            <div style='font-family:Arial,sans-serif; color:#333; max-width:600px; margin:auto; border:1px solid #ddd; border-radius:8px; overflow:hidden;'>
                <div style='background:#f8f9fa; padding:20px; text-align:center;'>
                    <h2 style='color:#dc3545;'>Lamaran Anda Ditolak</h2>
                </div>
                <div style='padding:20px;'>
                    <p>Halo <b>{data.Nama}</b>,</p>
                    <p>Terima kasih atas ketertarikan Anda pada posisi <b>{lowongan.Posisi}</b> di <b>{perusahaan.NamaPerusahaan}</b>.</p>
                    <p>Sayangnya, lamaran Anda <span style='color:red;font-weight:bold;'>DITOLAK</span> dan tidak dapat kami lanjutkan ke tahap berikutnya.</p>
                    <p>Kami sangat menghargai waktu dan minat Anda, semoga sukses dalam perjalanan karier Anda.</p>
                    <p>Hormat kami,<br/><b>{perusahaan.NamaPerusahaan}</b></p>
                </div>
            </div>";
                }

                if (!string.IsNullOrEmpty(bodyHtml))
                {
                    _emailHelper.SendStatusLamaranEmailAsync(data.Email, subject, bodyHtml);
                }
            }

            return true;
        }












        ///==================== UNTUK USER ================\\\
        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        public async Task<bool> AddLamaran(LamaranAddUpdateDTO dto, int lowonganId)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                throw new UnauthorizedAccessException("User belum login.");

            string fileName = null;
            if (dto.CV != null)
                fileName = await _fileHelper.UploadPdfAsync(dto.CV, "uploads/pdf");

            var lamaran = new Lamaran
            {
                UserId = userId,
                LowonganId = lowonganId,
                Nama = dto.Nama,
                Email = dto.Email,
                NoHP = dto.NoHP,
                Pendidikan = dto.Pendidikan,
                GajiSaatIni = dto.GajiSaatIni,
                GajiDiharapkan = dto.GajiDiharapkan,
                CV = fileName,
                Status = DataStatusLamaran.Diproses,
                TanggalDilamar = DateTime.Now
            };

            _context.Lamarans.Add(lamaran);
            await _context.SaveChangesAsync();
            return true;
        }

        ///=================================================\\\

        public async Task<byte[]> DownloadCvAsync(int lamaranId)
        {
            var lamaran = await _context.Lamarans.FirstOrDefaultAsync(l => l.Id == lamaranId);
            if (lamaran == null || string.IsNullOrEmpty(lamaran.CV))
                throw new FileNotFoundException("CV tidak ditemukan");

            return await _fileHelper.DownloadFileAsync(lamaran.CV, "uploads/pdf");
        }

        public bool DeleteLamaran(int id)
        {
            var perusahaanId = GetCurrentPerusahaanId();
            var data = _context.Lamarans.Include(l => l.Lowongan)
                .ThenInclude(p => p.Perusahaan)
                .Where(x => x.Lowongan.PerusahaanId == perusahaanId).FirstOrDefault(x => x.Id == id);

            if (data == null)
            {
                return false;
            }

            _context.Lamarans.Remove(data);
            _context.SaveChanges();
            return true;
        }

       
    }
}
