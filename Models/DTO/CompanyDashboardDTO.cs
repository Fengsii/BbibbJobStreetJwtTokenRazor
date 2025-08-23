namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class CompanyDashboardDTO
    {
        public int CompanyId { get; set; }
        public int TotalLowongan { get; set; }
        public int PelamarAktif { get; set; }
        public int LamaranBaru { get; set; }
        public string StatistikBulanan { get; set; }
        public int LowonganTerbaru { get; set; }
        public int LamaranHariIni { get; set; }
        public int LamaranMinggIni { get; set; }
        public int LamaranDiterima { get; set; }
        public List<LowonganPekerjaanViewDTO> DaftarLowonganTerbaru { get; set; }
        public List<LamaranChartDTO> GrafikLamaran { get; set; }
    }
}
