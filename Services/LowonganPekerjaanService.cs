using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static BbibbJobStreetJwtToken.Models.GeneralStatus;

namespace BbibbJobStreetJwtToken.Services
{
    public class LowonganPekerjaanService : ILowonganPekerjaan
    {
        private readonly ApplicationContext _context;

        public LowonganPekerjaanService(ApplicationContext context)
        {
            _context = context;
        }

        public List<LowonganPekerjaanViewDTO> GetListLowonganPekerjaan()
        {
            var data = _context.LowonganPekerjaans.Include(y => y.Kategori).Include(y => y.Perusahaan).Where(x => x.status != StatusLowongan.StatusLowonganPekerjaan.Delete).Select(x => new LowonganPekerjaanViewDTO
            {
                Id = x.Id,
                Logo = !string.IsNullOrEmpty(x.Perusahaan.LogoPath) ? "/" + x.Perusahaan.LogoPath : null,
                Judul = x.Judul,
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
            var data = _context.LowonganPekerjaans.Where(x => x.Id == id && x.status != StatusLowongan.StatusLowonganPekerjaan.Delete).FirstOrDefault();
            if (data == null)
            {
                return new LowonganPekerjaan();
            }

            return data;
        }

        public bool AddLowonganPekerjaan(LowonganPekerjaanAddUpdateDTO request)
        {
            // Ambil LogoPath dari Perusahaan berdasarkan PerusahaanId
            var perusahaan = _context.Perusahaans.FirstOrDefault(p => p.Id == request.PerusahaanId);

            if (perusahaan == null)
            {
                // Handle jika perusahaan tidak ditemukan
                return false;
            }

            var data = new LowonganPekerjaan();


            data.Judul = request.Judul;
            data.Logo = perusahaan.LogoPath;
            data.Posisi = request.Posisi;
            data.Deskripsi = request.Deskripsi;
            data.TanggalDibuat = request.TanggalDibuat;
            data.status = request.status;
            data.KategoriId = request.KategoriId;
            data.PerusahaanId = request.PerusahaanId;

            _context.LowonganPekerjaans.Add(data);
            _context.SaveChanges();
            return true;

        }

        public bool UpdateLowonganPekerjaan(LowonganPekerjaanAddUpdateDTO lowonganPekerjaanAddUpdateDTO)
        {
            var data = _context.LowonganPekerjaans.FirstOrDefault(x => x.Id == lowonganPekerjaanAddUpdateDTO.Id);
            if (data == null)
            {
                return false;
            }

           
            data.Judul = lowonganPekerjaanAddUpdateDTO.Judul;
            data.Posisi = lowonganPekerjaanAddUpdateDTO.Posisi;
            data.Deskripsi = lowonganPekerjaanAddUpdateDTO .Deskripsi;
            data.TanggalDibuat = lowonganPekerjaanAddUpdateDTO.TanggalDibuat;
            data.status = lowonganPekerjaanAddUpdateDTO.status;
            data.KategoriId = lowonganPekerjaanAddUpdateDTO.KategoriId;
            data.PerusahaanId = lowonganPekerjaanAddUpdateDTO.PerusahaanId;

            _context.LowonganPekerjaans.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteLowonganPekerjaan(int id)
        {
            var data = _context.LowonganPekerjaans.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }

            data.status = StatusLowongan.StatusLowonganPekerjaan.Delete;
            _context.SaveChanges();
            return true;
        }

        public List<SelectListItem> LowonganPekerjaan()
        {
            var datas = _context.LowonganPekerjaans
                .Where(x => x.status == StatusLowongan.StatusLowonganPekerjaan.Active)
                .Select(x => new SelectListItem
                {
                    Text = x.Judul,
                    Value = x.Id.ToString()
                }).ToList();

            return datas;
        }

    }
}
