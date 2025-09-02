using BbibbJobStreetJwtToken.Models.DTO;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IDashboardAdminService
    {
        public DashboardAdminDTO GetDashboardStats();
    }
}
