using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static BbibbJobStreetJwtToken.Models.StatusLamaran;

namespace BbibbJobStreetJwtToken.Services
{
    public class LamaranService : ILamaran
    {
        public readonly ApplicationContext _context;
        private readonly IFileHelper _fileHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LamaranService(ApplicationContext applicationContext, IFileHelper fileHelper, IHttpContextAccessor httpContextAccessor)
        {
            _context = applicationContext;
            _fileHelper = fileHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<LamaranViewDTO> GetListLamaran()
        {
            var data = _context.Lamarans.Select(x => new LamaranViewDTO
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

            }).ToList();
            return data;

        }

        public Lamaran GetLamaranById(int id)
        {
            var data = _context.Lamarans.FirstOrDefault();
            if (data == null)
            {
                return new Lamaran();
            }

            return data;
        }



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

            //bool sudahMelamar = await _context.Lamarans
            //    .AnyAsync(l => l.UserId == userId && l.LowonganId == lowonganId);

            //if (sudahMelamar)
            //    throw new InvalidOperationException("Anda sudah melamar lowongan ini sebelumnya.");

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

        public async Task<byte[]> DownloadCvAsync(int lamaranId)
        {
            var lamaran = await _context.Lamarans.FirstOrDefaultAsync(l => l.Id == lamaranId);
            if (lamaran == null || string.IsNullOrEmpty(lamaran.CV))
                throw new FileNotFoundException("CV tidak ditemukan");

            return await _fileHelper.DownloadFileAsync(lamaran.CV, "uploads/pdf");
        }


        public bool UpdateLamaran(LamaranAddUpdateDTO lamaranAddUpdateDTO)
        {
            var data = _context.Lamarans.FirstOrDefault(x => x.Id == lamaranAddUpdateDTO.Id);
            if (data == null)
            {
                return false;
            }


            data.Status = lamaranAddUpdateDTO.Status;

            _context.Lamarans.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteLamaran(int id)
        {
            var data = _context.Lamarans.FirstOrDefault(x => x.Id == id);

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
