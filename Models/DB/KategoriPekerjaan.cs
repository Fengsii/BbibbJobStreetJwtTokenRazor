namespace BbibbJobStreetJwtToken.Models.DB
{
    public class KategoriPekerjaan
    {
        public int Id { get; set; }
        public string NamaKategori { get; set; }
        public string Deskripsi { get; set; }

        public ICollection<LowonganPekerjaan> Lowongans { get; set; } = new List<LowonganPekerjaan>();    
    }
}
