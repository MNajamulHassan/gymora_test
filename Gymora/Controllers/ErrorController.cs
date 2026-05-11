using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymora.Controllers
{
public class ErrorController : Controller
{
    [AllowAnonymous]
    public IActionResult Index(int? code)
    {
        var status = code ?? 500;
        Response.StatusCode = status;
        return View("~/Views/Shared/Error.cshtml");
    }
}
}
