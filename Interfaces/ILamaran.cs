using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using X.PagedList;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface ILamaran
    {
        public IPagedList<LamaranViewDTO> GetListLamaran(int page, int pageSize, string searchTerm = "");
        public Lamaran GetLamaranById(int id);
        Task<bool> AddLamaran(LamaranAddUpdateDTO lamaranAddUpdateDTO, int lowonganId);
        bool UpdateLamaran(LamaranAddUpdateDTO lamaranAddUpdateDTO);
        Task<byte[]> DownloadCvAsync(int lamaranId);
        public bool DeleteLamaran(int id);
        
    }
}
