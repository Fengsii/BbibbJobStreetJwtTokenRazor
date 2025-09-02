using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IPerusahaan
    {
        public IPagedList<CompanyViewDTO> GetCurrentCompanyForSuperAdmin(int page, int pageSize, string searchTerm = "");
        public List<CompanyViewDTO> GetCurrentCompanyForSuperAdmin();
        public bool UpdateCompanyForSuperAdmin(RegisterPerusahaanDTO dto);
        public CompanyViewDTO GetCurrentCompany();
        public List<CompanyViewDTO> GetListCompany();
        public Perusahaan GetCompanyById(int id);
        public bool UpdateCompany(RegisterPerusahaanDTO dto);
        Task<string?> LoginAsync(LoginPerusahaanDTO loginPerusahaanDTO);
        Task<bool> PerusahaanExistsAsync(string name, string email);
        Task<bool> RegisterAsync(RegisterPerusahaanDTO model);
        public List<SelectListItem> Perusahaan();
        public bool DeleteCompanyForSuperAdmin(int id);
        Task<bool> SendOtpAsync(string emailOrUsername);
        Task<bool> ResetPasswordWithOtpAsync(VerifyOtpAndResetPasswordDTO dto);
    }
}
