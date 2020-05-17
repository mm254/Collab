using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IvA.Controllers
{
    public class ProjekteController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}