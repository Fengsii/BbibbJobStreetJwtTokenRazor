using static BbibbJobStreetJwtToken.Models.GeneralStatus;

namespace BbibbJobStreetJwtToken.Models.DB
{
    public class Perusahaan
    {
        public int Id { get; set; }
        public string NamaPerusahaan { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Telepon { get; set; }
        public string Alamat { get; set; }
        public string Kota { get; set; }
        public string Provinsi { get; set; }
        public string BidangUsaha { get; set; } 
        public DateTime TanggalBerdiri { get; set; } 
        public GeneralStatusData Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string LogoPath { get; set; }


        // OTP Fields
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiredAt { get; set; }

        public ICollection<LowonganPekerjaan> Lowongans { get; set; } = new List<LowonganPekerjaan>();
    }
}
