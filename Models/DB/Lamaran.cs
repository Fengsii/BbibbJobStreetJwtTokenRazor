using static BbibbJobStreetJwtToken.Models.StatusLamaran;
using static BbibbJobStreetJwtToken.Models.StatusPendidikan;

namespace BbibbJobStreetJwtToken.Models.DB
{
    public class Lamaran
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LowonganId { get; set; }
        public string Nama { get; set; }
        public string Email { get; set; }
        public int NoHP { get; set; }
        public DataStatusPendidikan Pendidikan { get; set; }
        public int GajiSaatIni { get; set; }
        public int GajiDiharapkan { get; set; }
        public string CV { get; set; }
        public DataStatusLamaran Status { get; set; }
        public DateTime TanggalDilamar { get; set; }

        public User user { get; set; }
        public LowonganPekerjaan Lowongan { get; set; }
        public LamaranTersimpan LamaranTersimpan { get; set; }

    }
}
