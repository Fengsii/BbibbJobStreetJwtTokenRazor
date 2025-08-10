using static BbibbJobStreetJwtToken.Models.GeneralStatus;

namespace BbibbJobStreetJwtToken.Models.DB
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string ProfileImage {  get; set; }
        public string CoverImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public GeneralStatusData Status { get; set; }

        public ProfileUserDetails ProfileUserDetails { get; set; }
        public ICollection<Lamaran> Lamarans { get; set; } = new List<Lamaran>();
        public ICollection<LowonganTersimpan> lowonganTersimpans { get; set; } = new List<LowonganTersimpan>();
        public ICollection<LamaranTersimpan> RiwayatLamaran { get; set; } = new List<LamaranTersimpan>();
    }
}
