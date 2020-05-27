using IvA.Data;
using IvA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Controllers
{
    public class ProjekteArbeitsPaketeViewModelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjekteArbeitsPaketeViewModelController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: ArbeitsPaket
        public async Task<IActionResult> Index()
        { 
            return View(await _context.ProjekteArbeitsPaketeViewModel.ToListAsync());
        }

    }
}
