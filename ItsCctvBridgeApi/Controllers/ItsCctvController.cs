using ItsCctvBridgeApi.Models;
using ItsCctvBridgeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ItsCctvBridgeApi.Controllers
{
    [ApiController]
    [Route("api/itscctv")]
    public class ItsCctvController : ControllerBase
    {
        private readonly ItsCctvService service;

        public ItsCctvController(ItsCctvService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> SearchCctv(CctvRequest request)
        {
            //var result = await service.GetCctvListAsync("testURL");
            var result = await service.GetCctvSearchAsync(request);

            return Ok(result);
        }

    }
}
