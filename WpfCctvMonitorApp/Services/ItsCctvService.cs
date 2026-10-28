using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using WpfCctvMonitorApp.Common;
using WpfCctvMonitorApp.Models;

namespace WpfCctvMonitorApp.Services
{
    public class ItsCctvService
    {
        private readonly HttpClient httpClient = new ();

        // TODO
        public async Task<CctvResponse> GetCctvListAsync(string apiUrl)
        {
            string json = await httpClient.GetStringAsync(apiUrl);

            var result = JsonConvert.DeserializeObject<CctvResponse>(json);

            if (result == null)
                return new CctvResponse();
            else
                return result;
        }

        public async Task<List<CctvResultDto>> GetBridgeApiAsync(CctvRequest request)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, AppCommon.baseUrl);

            req.Content = new StringContent(
                JsonConvert.SerializeObject(request),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.SendAsync(req);

            string json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<CctvResultDto>>(json);

            if (result == null) return new();
            else return result;
        }

    }
}
