using Microsoft.AspNetCore.Mvc;

using UADetector.Parsers;

namespace UADetector.Api.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    private readonly IOsParser _osParser;
    private readonly IBrowserParser _browserParser;

    public TestController(IOsParser osParser, IBrowserParser browserParser)
    {
        _osParser = osParser;
        _browserParser = browserParser;
    }

    [Route("")]
    public IActionResult Get()
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        _osParser.TryParse(userAgent, out var os);

        var headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToArray().FirstOrDefault());
        _browserParser.TryParse(userAgent, out var browser);

        return Ok(browser);
    }
}
