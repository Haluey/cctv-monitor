namespace ItsCctvBridgeApi.Models
{
    public class CctvRequest
    {
        // Type 클래스 키워드명 -> RoadType으로 변경
        public string RoadType { get; set; } = "ex";    // 고속도로 기본선택
        
        public int CctvType { get; set; } = 1;  // 1:HLS, 2:mp4, 3:img, 4:HLS(https), 5:mp4(https)
       
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

        // GetType() 메서드명 -> GetRetType으로 변경
        public string GetRetType { get; set; } = "json";
    }
}
