using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IvA.Data;
using IvA.Models;
using Microsoft.AspNetCore.Mvc;

namespace IvA.Controllers
{
    public class ProjektPaketeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjektPaketeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<ArbeitsPaketModel> Pakete = _context.ArbeitsPaket.ToList();
            List<ProjekteModel> Projekte = _context.Projekte.ToList();
            List<ProjekteArbeitsPaketeViewModel> ProjektPakete = _context.ProjekteArbeitsPaketeViewModel.ToList();

            var projektPaketeView = from _projekte in Projekte
                                    join _projektPakete in ProjektPakete 
                                    on _projekte.Id equals _projektPakete.Id into table1
                                 from _projektPakete in table1.ToList()
                                 join _pakete in Pakete 
                                 on _projektPakete.ArbeitsPaketId equals _pakete.ArbeitsPaketId into table2
                                 from _pakete in table2.ToList()
                                 select new ProjektPaketeModel
                                 {
                                     Pakete=_pakete,
                                     Projekte = _projekte,
                                     ProjektPakete = _projektPakete
                                 };
            return View(projektPaketeView);
        }
    }
}