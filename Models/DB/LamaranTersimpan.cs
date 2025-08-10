namespace BbibbJobStreetJwtToken.Models.DB
{
    public class LamaranTersimpan
    {
        public int Id { get; set; }
        public int PenggunaId { get; set; }
        public int LamaranId { get; set; }
        public string Status { get; set; }
        public string CatatanHR { get; set; }
        public DateTime TanggalTersimpan { get; set; }

        public User User { get; set; }
        public Lamaran Lamaran { get; set; }
    }
}
