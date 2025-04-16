using Microsoft.AspNetCore.Mvc;

namespace UaDetector.Api.Controllers;

[ApiController]
public class UaDetectorController : ControllerBase
{
    [HttpGet]
    [Route("ua-detector")]
    public IActionResult GetUserAgentInfo()
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToArray().FirstOrDefault());

        var parser = new UaDetector();
        parser.TryParse(userAgent, headers, out var result);
        return Ok(result);
    }
}
