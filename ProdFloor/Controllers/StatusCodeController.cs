using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

namespace ProdFloor.Controllers
{
    public class StatusCodeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
    
        public StatusCodeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /<controller>/

        [HttpGet("/StatusCode/{statusCode}")]

        public IActionResult Index(int statusCode)

        {
            var reExecute = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            _logger.LogInformation($"Unexpected Status Code: {statusCode}, OriginalPath: {reExecute.OriginalPath}");
            return View(statusCode);
        }
    
    }
}
