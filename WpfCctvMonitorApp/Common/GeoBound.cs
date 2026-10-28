namespace WpfCctvMonitorApp.Common
{
    public class GeoBound
    {
        public double MinLat { get; }   // MinY 에 할당
        public double MaxLat { get; }   // MaxY 에 할당

        public double MinLng { get; }   // MinX 에 할당
        public double MaxLng { get; }   // MaxX 에 할당

        // 파라미터 할당 생성자
        public GeoBound(double minLat, double maxLat, double minLng, double maxLng)
        {
            MinLat = minLat;
            MaxLat = maxLat;
            MinLng = minLng;
            MaxLng = maxLng;
        }
    }
}
