using Microsoft.AspNetCore.Mvc;

using UaDetector.Parsers;

namespace UaDetector.Api.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [Route("")]
    public IActionResult Get()
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToArray().FirstOrDefault());

        var parser = new UaDetector();
        parser.TryParse(userAgent, headers, out var result);
        return Ok(result);
    }
}
