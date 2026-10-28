using ItsCctvBridgeApi.Models;
using Newtonsoft.Json;

namespace ItsCctvBridgeApi.Services
{
    public class ItsCctvService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;  // appsetting.json 사용

        public ItsCctvService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }

        // HACK : 예전 WPF 메서드
        public async Task<CctvResponse> GetCctvListAsync(string apiUrl)
        {
            string json = await httpClient.GetStringAsync(apiUrl);

            var result = JsonConvert.DeserializeObject<CctvResponse>(json);

            if (result == null)
                return new CctvResponse();
            else
                return result;
        }

        // 웹서비스용으로 변경된 새로운 메서드
        public async Task<List<CctvResultDto>> GetCctvSearchAsync(CctvRequest request)
        {
            var apiKey = configuration["ItsOpenApi:ApiKey"];    // appsetting.json 키 받아옴

            var url = $"?apiKey={apiKey}" +
                        $"&type={request.RoadType}" +
                        $"&cctvType={request.CctvType}" +
                        $"&minX={request.MinX}" +
                        $"&maxX={request.MaxX}" +
                        $"&minY={request.MinY}" +
                        $"&maxY={request.MaxY}" +
                        $"&getType={request.GetRetType}";

            string json = await httpClient.GetStringAsync(url);

            var result = JsonConvert.DeserializeObject<CctvResponse>(json);

            if (result == null)
                return new ();
            
            // LINQ 방식을 안쓰면 for문으로 직접처리
            // OpenAPI 서비스 결과 json에서
            // roadsectionid, cctvresolution, filecreatetime 속성 제거하고 새로 json생성
            return result.Response.Data.Select( x => new CctvResultDto
                    {
                        CctvName = x.CctvName,
                        CoordX = Convert.ToDouble(x.CoordX),
                        CoordY = Convert.ToDouble(x.CoordY),
                        CctvType = Convert.ToInt32(x.CctvType),
                        CctvFormat = x.CctvFormat,
                        CctvUrl = x.CctvUrl
                    }).ToList();
        }

    }
}
