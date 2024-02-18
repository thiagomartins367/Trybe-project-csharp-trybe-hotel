using Microsoft.AspNetCore.Mvc;

namespace TrybeHotel.Controllers;

[ApiController]
[Route("license")]
[ApiExplorerSettings(IgnoreApi = true)]
public class LicenseController : ControllerBase
{
    [HttpGet("mit")]
    public IActionResult Get()
    {
        var rootPath = Directory.GetCurrentDirectory();
        string licenseFilePath = Path.Combine(rootPath, "LICENSE");
        StreamReader sr = new(licenseFilePath);
        var content = sr.ReadToEnd();
        return Ok(content);
    }
}