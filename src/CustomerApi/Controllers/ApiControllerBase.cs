using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers;

[Route("[controller]")]
[ApiController]
public class ApiControllerBase : ControllerBase
{
    private readonly ILogger _logger;
    protected ILogger Logger => _logger;

    public ApiControllerBase(ILogger logger)
    {
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }
}
