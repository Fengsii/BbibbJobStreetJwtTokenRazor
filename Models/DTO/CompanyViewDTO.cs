using static BbibbJobStreetJwtToken.Models.GeneralStatus;

namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class CompanyViewDTO
    {
        public int PerusahaanId { get; set; }
        public string NamaPerusahaan { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Telepon { get; set; }
        public string Alamat { get; set; }
        public string Kota { get; set; }
        public string Provinsi { get; set; }
        public string BidangUsaha { get; set; }
        public GeneralStatusData Status { get; set; }
        public DateTime TanggalBerdiri { get; set; }
        public string LogoPath { get; set; }
    }
}
