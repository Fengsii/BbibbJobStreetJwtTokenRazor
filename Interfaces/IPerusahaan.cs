using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IPerusahaan
    {
        public CompanyViewDTO GetCurrentCompany();
        public List<CompanyViewDTO> GetListCompany();
        public Perusahaan GetCompanyById(int id);
        public bool UpdateCompany(RegisterPerusahaanDTO dto);
        Task<string?> LoginAsync(LoginPerusahaanDTO loginPerusahaanDTO);
        Task<bool> PerusahaanExistsAsync(string name, string email);
        Task<bool> RegisterAsync(RegisterPerusahaanDTO model);
        public List<SelectListItem> Perusahaan();
    }
}
