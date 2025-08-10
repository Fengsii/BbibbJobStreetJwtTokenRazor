using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface ILowonganTersimpan
    {
        public List<LowonganTersimpanViewDTO> GetListLowonganTersimpan();
        public LowonganTersimpan GetLowonganTersimpanById(int id);
        Task<bool> AddLowonganTersimpan(LowonganTersimpanAddUpdateDTO dto, int lowonganId);
        public bool DeleteLowonganTersimpan(int id);
    }
}
