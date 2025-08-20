using static BbibbJobStreetJwtToken.Models.StatusLowongan;

namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class LowonganPekerjaanAddUpdateDTO
    {
        public int Id { get; set; }
        public string Judul { get; set; }
        public string Alamat { get; set; }
        public string Deskripsi { get; set; }
        public string Posisi { get; set; }
        public int KategoriId { get; set; }
        //public int PerusahaanId { get; set; }
        public DateTime TanggalDibuat { get; set; }
        public StatusLowonganPekerjaan status { get; set; }
    }
}
