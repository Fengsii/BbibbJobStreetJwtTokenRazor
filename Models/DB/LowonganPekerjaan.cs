using static BbibbJobStreetJwtToken.Models.StatusLowongan;

namespace BbibbJobStreetJwtToken.Models.DB
{
    public class LowonganPekerjaan
    {
        public int Id { get; set; }
        public string Logo { get; set; }
        public string Judul { get; set; }
        public string Deskripsi { get; set; }
        public string Posisi { get; set; }
        public DateTime TanggalDibuat { get; set; }
        public StatusLowonganPekerjaan status { get; set; }

        public int KategoriId { get; set; }
        public KategoriPekerjaan Kategori { get; set; }
        public int PerusahaanId { get; set; }
        public Perusahaan Perusahaan { get; set; }
        public ICollection<Lamaran> Lamarans { get; set; } = new List<Lamaran>();
        public ICollection<LowonganTersimpan> LowonganTersimpans { get; set; } = new List<LowonganTersimpan>();
    }
}
