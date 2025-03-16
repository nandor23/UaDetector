using Microsoft.AspNetCore.Mvc;

using UADetector.Parsers;

namespace UADetector.Api.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    private readonly IOsParser _osParser;
    private readonly IClientParser _clientParser;


    public TestController(IOsParser osParser, IClientParser clientParser)
    {
        _osParser = osParser;
        _clientParser = clientParser;
    }

    [Route("")]
    public IActionResult Get()
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        _osParser.TryParse(userAgent, out var result);

        var headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToArray().FirstOrDefault());
        var clientHints = ClientHints.Create(headers);

        _clientParser.TryParse(userAgent, clientHints, out var client);

        return Ok(result);
    }
}
