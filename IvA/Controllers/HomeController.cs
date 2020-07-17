using IvA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IvA.Controllers
{
    // Die Controllermethoden geben jeweils die HTML-Datei für die Startseite zurück
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //gibt die Startseite zurück
        public IActionResult Index()
        {
            return View();
        }

        //Gibt das Impressum zurück
        public IActionResult Impressum()
        {
            return View();
        }

        // Seitenübergreifendes Fehler Handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
