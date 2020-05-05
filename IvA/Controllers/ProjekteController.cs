using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IvA.Controllers
{
    public class ProjekteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}