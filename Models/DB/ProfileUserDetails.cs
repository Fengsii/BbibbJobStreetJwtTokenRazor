namespace BbibbJobStreetJwtToken.Models.DB
{
    public class ProfileUserDetails
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public string Bio { get; set; }
        public string Tentang { get; set; }
        public string PengalamanKerja { get; set; }
        public string GambarPengalamanKerja { get; set; }
        public string Sertifikat { get; set; }
        public string Pendidikan { get; set; }
        public string Keahlian { get; set; }
        public string Minat { get; set; }

        public User User { get; set; }

    }
}
