using Microsoft.AspNetCore.Mvc;

namespace UaDetector.Api.Controllers;

[ApiController]
public class UaDetectorController : ControllerBase
{
    private readonly IUaDetector _uaDetector;

    public UaDetectorController(IUaDetector uaDetector)
    {
        _uaDetector = uaDetector;
    }

    [HttpGet]
    [Route("ua-detector")]
    public IActionResult GetUserAgentInfo()
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var headers = Request.Headers.ToDictionary(
            h => h.Key,
            h => h.Value.ToArray().FirstOrDefault()
        );

        if (_uaDetector.TryParse(userAgent, headers, out var result))
        {
            return Ok(result);
        }

        return BadRequest("No matching user-agent information was found");
    }
}
