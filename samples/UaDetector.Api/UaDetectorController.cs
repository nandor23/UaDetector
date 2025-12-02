using Microsoft.AspNetCore.Mvc;

namespace UaDetector.Api;

[ApiController]
[Route("user-agent")]
public class UaDetectorController : ControllerBase
{
    private readonly IUaDetector _uaDetector;

    public UaDetectorController(IUaDetector uaDetector)
    {
        _uaDetector = uaDetector;
    }

    [HttpGet]
    [Route("detect")]
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

        return BadRequest("Unrecognized user agent");
    }

    [HttpGet]
    [Route("parse")]
    public IActionResult GetUserAgentInfo([FromQuery] string userAgent)
    {
        if (_uaDetector.TryParse(userAgent, out var result))
        {
            return Ok(result);
        }

        return BadRequest("Unrecognized user agent");
    }
}
