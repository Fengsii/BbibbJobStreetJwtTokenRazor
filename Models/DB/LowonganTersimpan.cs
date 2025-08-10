namespace BbibbJobStreetJwtToken.Models.DB
{
    public class LowonganTersimpan
    {
        public int Id { get; set; }
        public int PenggunaId { get; set; }
        public int LowonganId { get; set; }
        public DateTime TanggalDisimpan { get; set; }

        public User User { get; set; }
        public LowonganPekerjaan Lowongan { get; set; }
    }
}
