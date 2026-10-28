namespace WpfCctvMonitorApp.Models
{
    // Dto(Data Transfer Object)
    public class CctvResultDto
    {
        public double CoordX { get; set; } = 0;
        public double CoordY { get; set; } = 0;
        public int CctvType { get; set; } = 1;
        public string CctvFormat { get; set; } = "HLS";
        public string CctvName { get; set; } = "";
        public string CctvUrl { get; set; } = "";
    }
}
