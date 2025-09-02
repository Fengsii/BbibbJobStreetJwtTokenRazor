using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface ILowonganPekerjaan
    {
        public IPagedList<LowonganPekerjaanViewDTO> GetListLowonganPekerjaanForSuperAdmin(int page, int pageSize, string searchTerm = "");
        public IPagedList<LowonganPekerjaanViewDTO> GetListLowonganPekerjaan(int page, int pageSize, string searchTerm = "");
        public LowonganPekerjaan GetLowonganPekerjaanById(int id);
        public bool AddLowonganPekerjaan(LowonganPekerjaanAddUpdateDTO request);
        public bool UpdateLowonganPekerjaan(LowonganPekerjaanAddUpdateDTO lowonganPekerjaanAddUpdateDTO);
        public bool DeleteLowonganPekerjaan(int id);
        public List<LowonganPekerjaanViewDTO> SearchLowonganPekerjaan(string keyword);
        public List<LowonganPekerjaanViewDTO> GetListLowonganPekerjaanForUser();
        public List<NotificationsDTO> GetNotificationsForUser(int userId);
        public bool DeleteNotifications(int id);
        public bool DeleteLowonganPekerjaanForSuperAdmin(int id);
    }
}
