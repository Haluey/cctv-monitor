namespace ItsCctvBridgeApi.Common
{
    public static class AppCommon
    {
        public const string appName = "국가교통정보센터 CCTV 브릿지서비스";

        public const string baseUrl = "https://openapi.its.go.kr:9443/cctvInfo";

        // 대한민국 지도 영역
        // 위도 y
        //public static double MinY { get; set; } = 33.100000; 
        //public static double MaxY { get; set; } = 39.000000;

        //// 경도 x
        //public static double MinX { get; set; } = 126.000000;
        //public static double MaxX { get; set; } = 129.660000;

        //public static string BuildCctvApiUrl()
        //{
        //    return $"{baseUrl}" +
        //           $"?apiKey={ItsApiKey}" +
        //           $"&type={RoadType}" +
        //           $"&cctvType={CctvType}" +
        //           $"&minX={MinX}" +
        //           $"&maxX={MaxX}" +
        //           $"&minY={MinY}" +
        //           $"&maxY={MaxY}" +
        //           $"&getType={GetType}";
        //}

        //public static readonly string[] Regions =
        //{
        //    "-- 선택 --", // 인덱스 0은 사용안함
        //    "전국",
        //    "서울",
        //    "인천",
        //    "경기",
        //    "강원",
        //    "충북",
        //    "충남",
        //    "세종",
        //    "대전",
        //    "전북",
        //    "전남",
        //    "광주",
        //    "경북",
        //    "대구",
        //    "울산",
        //    "부산",
        //    "경남",
        //    "제주"
        //};

        //public static readonly Dictionary<string, GeoBound> RegionBounds = new()
        //{
        //    ["전국"] = new GeoBound(33.1, 39.0, 126.0, 129.7),
        //    ["서울"] = new GeoBound(minLat: 37.4133, maxLat: 37.7100, minLng: 126.7757, maxLng: 127.1600),
        //    ["인천"] = new GeoBound(37.0000, 37.9500, 124.6000, 126.8500),
        //    ["경기"] = new GeoBound(36.8900, 38.3000, 126.3700, 127.8500),
        //    ["강원"] = new GeoBound(37.0200, 38.6200, 127.0500, 129.3700),

        //    ["충북"] = new GeoBound(36.0000, 37.2500, 127.2500, 128.7300),
        //    ["충남"] = new GeoBound(35.9800, 37.0600, 126.1000, 127.6400),
        //    ["세종"] = new GeoBound(36.4000, 36.7500, 127.1500, 127.4500),
        //    ["대전"] = new GeoBound(36.1800, 36.5000, 127.2500, 127.5500),

        //    ["전북"] = new GeoBound(35.3000, 36.1700, 126.4300, 127.8800),
        //    ["전남"] = new GeoBound(33.9000, 35.5000, 125.0000, 127.9000),
        //    ["광주"] = new GeoBound(35.0000, 35.3000, 126.6500, 127.0500),

        //    ["경북"] = new GeoBound(35.6000, 37.5600, 128.0000, 130.9000),
        //    ["대구"] = new GeoBound(35.6090, 36.0100, 128.3490, 128.7780),
        //    ["울산"] = new GeoBound(35.3200, 35.7500, 129.0000, 129.4700),
        //    ["부산"] = new GeoBound(34.8799, 35.3959, 128.7384, 129.3729),
        //    ["경남"] = new GeoBound(34.5600, 35.9200, 127.5600, 129.3100),

        //    ["제주"] = new GeoBound(33.1000, 33.6000, 126.1000, 126.9500),
        //};

        // 글자 길이가 너무 길면 생략하는 메서드
        //public static string Ellipse(string text, int maxLength = 100)
        //{
        //    if (string.IsNullOrWhiteSpace(text))
        //    {
        //        return string.Empty;    // ""
        //    }

        //    if (text.Length <= maxLength)
        //    {
        //        return text;
        //    }

        //    return text[..maxLength] + "...";
        //}

        // CctvName 잘라서 분리메서드
        // string.Substring()으로 가능한 작업 -> 정규식은 간결하게 패턴타입 처리가능
        //public static (string cctvName, string roadName, string direction) ParseName(string originCctvName)
        //{
        //    if (string.IsNullOrWhiteSpace(originCctvName))
        //    {
        //        return ("", "", "");
        //    }

        //    // "[수도권제1순환선] 성남요금소 (서울)" 문자열을 정규식패턴으로 자르기
        //    Match match = Regex.Match(
        //        originCctvName,
        //        @"^\[(.*?)\]\s*(.*?)(?:\((.*?)\))?$"
        //    );

        //    if (!match.Success)
        //    {
        //        return (originCctvName, "", "");    // 패턴매칭 실패
        //    }

        //    var roadName = match.Groups[1].Value.Trim();
        //    var cctvName = match.Groups[2].Value.Trim();
        //    var direction = match.Groups[3].Value.Trim();

        //    return (cctvName, roadName, direction);     // Python tuple과 동일
        //}

    }
}
