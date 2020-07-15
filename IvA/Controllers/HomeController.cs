using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IvA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IvA.Controllers
{
    // Die Controllermethoden geben jeweils die HTML-Datei für die Startseite zurück
    public class HomeController : Controller { 

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

        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Verwaltung()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
