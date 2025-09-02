using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IKategoriPekerjaan
    {
        public IPagedList<KategoriPekerjaanDTO> GetListKategoriPekerjaan(int page, int pageSize, string searchTerm = "");
        public KategoriPekerjaan GetListKategoriPekerjaanById(int id);
        public bool UpdateKategoriPekerjaan(KategoriPekerjaanDTO kategoriPekerjaanDTO);
        public bool AddKategoriPekerjaaan(KategoriPekerjaanDTO kategoriPekerjaanDTO);
        public bool DeleteKategoriPekerjaan(int id);
        public List<SelectListItem> KategoriPekerjaan();
    }
}
