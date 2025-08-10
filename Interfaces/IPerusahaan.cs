using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IPerusahaan
    {
        Task<string?> LoginAsync(LoginPerusahaanDTO loginPerusahaanDTO);
        Task<bool> PerusahaanExistsAsync(string name, string email);
        Task<bool> RegisterAsync(RegisterPerusahaanDTO model);
        public List<SelectListItem> Perusahaan();
    }
}
