using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IvA.Data;
using Microsoft.AspNetCore.Mvc;

namespace IvA.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Chat()
        {
            return View();
        }
    }
}
