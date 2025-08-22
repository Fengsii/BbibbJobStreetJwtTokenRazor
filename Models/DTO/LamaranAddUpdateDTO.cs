using static BbibbJobStreetJwtToken.Models.StatusLamaran;
using static BbibbJobStreetJwtToken.Models.StatusPendidikan;

namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class LamaranAddUpdateDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LowonganId { get; set; }
        public string Nama { get; set; }
        public string Email { get; set; }
        public string NoHP { get; set; }
        public DataStatusPendidikan Pendidikan { get; set; }
        public int GajiSaatIni { get; set; }
        public int GajiDiharapkan { get; set; }
        public IFormFile CV { get; set; }
        public DataStatusLamaran Status { get; set; }

    }
}
