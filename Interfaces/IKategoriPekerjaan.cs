using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IKategoriPekerjaan
    {
        public List<KategoriPekerjaanDTO> GetListKategoriPekerjaan();
        public KategoriPekerjaan GetListKategoriPekerjaanById(int id);
        public bool UpdateKategoriPekerjaan(KategoriPekerjaanDTO kategoriPekerjaanDTO);
        public bool AddKategoriPekerjaaan(KategoriPekerjaanDTO kategoriPekerjaanDTO);
        public bool DeleteKategoriPekerjaan(int id);
        public List<SelectListItem> KategoriPekerjaan();
    }
}
