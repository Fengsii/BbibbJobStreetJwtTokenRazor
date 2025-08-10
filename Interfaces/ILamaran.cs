using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface ILamaran
    {
        public List<LamaranViewDTO> GetListLamaran();
        public Lamaran GetLamaranById(int id);
        Task<bool> AddLamaran(LamaranAddUpdateDTO lamaranAddUpdateDTO, int lowonganId);
        bool UpdateLamaran(LamaranAddUpdateDTO lamaranAddUpdateDTO);
        Task<byte[]> DownloadCvAsync(int lamaranId);
        public bool DeleteLamaran(int id);
        
    }
}
