namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class DashboardAdminDTO
    {
        public int TotalUsers { get; set; }
        public int TotalCompanies { get; set; }
        public int TotalJobVacancies { get; set; }
        public int TotalActiveJobVacancies { get; set; }
        public int TotalClosedJobVacancies { get; set; }
        public int UsersThisWeek { get; set; }
        public int CompaniesThisWeek { get; set; }
        public int JobsThisWeek { get; set; }
        public decimal JobToUserRatio { get; set; }
        public List<MonthlyStatDTO> MonthlyStats { get; set; } = new List<MonthlyStatDTO>();
        public List<CategoryStatDTO> CategoryStats { get; set; } = new List<CategoryStatDTO>();
        public List<ActivityDTO> RecentActivities { get; set; } = new List<ActivityDTO>();
    }
}
