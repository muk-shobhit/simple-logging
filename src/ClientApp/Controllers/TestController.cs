using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
       
    private readonly ILogger<TestController> logger;

    public TestController(ILogger<TestController> logger)
    {
        this.logger = logger;
    }

    [HttpGet]
    public ActionResult Get()
    {
        logger.LogInformation("Invoked test API");
        return Ok("Success");
    }
}