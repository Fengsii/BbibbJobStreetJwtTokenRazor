using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface ILowonganPekerjaan
    {
        public List<LowonganPekerjaanViewDTO> GetListLowonganPekerjaan();
        public LowonganPekerjaan GetLowonganPekerjaanById(int id);
        public bool AddLowonganPekerjaan(LowonganPekerjaanAddUpdateDTO request);
        public bool UpdateLowonganPekerjaan(LowonganPekerjaanAddUpdateDTO lowonganPekerjaanAddUpdateDTO);
        public bool DeleteLowonganPekerjaan(int id);
        public List<SelectListItem> LowonganPekerjaan();
    }
}
