using BbibbJobStreetJwtToken.Models.DB;
using static BbibbJobStreetJwtToken.Models.StatusLowongan;

namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class LowonganPekerjaanViewDTO
    {
        public int Id { get; set; }
        public string Logo { get; set; }
        public string Judul { get; set; }
        public string Posisi { get; set; }
        public string Deskripsi { get; set; }
        
       
        public string NamaKategori { get; set; }
        public int KategoriId { get; set; }
        public string NamaPerusahaan { get; set; }
        public int PerusahaanId { get; set; }
        public DateTime TanggalDibuat { get; set; }
        public StatusLowonganPekerjaan status { get; set; }
    }
}
