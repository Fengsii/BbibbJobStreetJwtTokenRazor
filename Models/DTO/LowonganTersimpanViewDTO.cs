using static BbibbJobStreetJwtToken.Models.StatusLowongan;

namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class LowonganTersimpanViewDTO
    {
        public int Id { get; set; }
        public string Posisi { get; set; }
        public string Deskripsi { get; set; }
        public DateTime TanggalDibuat { get; set; }
    }
}
