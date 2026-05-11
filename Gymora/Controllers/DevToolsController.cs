using Microsoft.AspNetCore.Mvc;

namespace Gymora.Controllers
{
    public class DevToolsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;

        public DevToolsController(
            IWebHostEnvironment env,
            IHttpContextAccessor http)
        {
            _env = env;
            _http = http;
        }

        [HttpGet]
        public IActionResult QrCode()
        {
            if (!_env.IsDevelopment())
                return NotFound();

            var request = _http.HttpContext?.Request;
            var host = request?.Host.Host ?? "localhost";
            var port = request?.Host.Port ?? 5000;
            var url = $"http://{host}:{port}";

            ViewBag.AppUrl = url;
            return View();
        }
    }
}
