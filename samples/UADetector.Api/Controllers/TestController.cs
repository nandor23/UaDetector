using Microsoft.AspNetCore.Mvc;

using UADetector.Parsers;

namespace UADetector.Api.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    private readonly IOsParser _osParser;


    public TestController(IOsParser osParser)
    {
        _osParser = osParser;
    }

    [Route("")]
    public IActionResult Get()
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        _osParser.TryParse(userAgent, out var result);

        return Ok(result);
    }
}
