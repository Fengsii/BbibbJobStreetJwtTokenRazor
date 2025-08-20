using BbibbJobStreetJwtToken.Models.DB;
using static BbibbJobStreetJwtToken.Models.StatusLowongan;

namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class LowonganPekerjaanViewDTO
    {
        public int Id { get; set; }
        public string Logo { get; set; }
        public string Judul { get; set; }
        public string Alamat { get; set; }
        public string Posisi { get; set; }
        public string Deskripsi { get; set; }
        
       
        public string NamaKategori { get; set; }
        public int KategoriId { get; set; }
        public string NamaPerusahaan { get; set; }
        public int PerusahaanId { get; set; }
        public DateTime TanggalDibuat { get; set; }
        public StatusLowonganPekerjaan status { get; set; }

        public string TimeAgo
        {
            get
            {
                // Pastikan tanggal dari DB di-convert ke waktu lokal
                var createdLocal = DateTime.SpecifyKind(TanggalDibuat, DateTimeKind.Utc).ToLocalTime();

                var ts = DateTime.Now - createdLocal;

                if (ts.TotalMinutes < 1)
                    return "Baru saja";
                if (ts.TotalMinutes < 60)
                    return $"{(int)ts.TotalMinutes} menit lalu";
                if (ts.TotalHours < 24)
                    return $"{(int)ts.TotalHours} jam lalu";
                return $"{(int)ts.TotalDays} hari lalu";
            }
        }
    }
}
