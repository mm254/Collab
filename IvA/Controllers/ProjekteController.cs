using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IvA.Data;
using IvA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace IvA.Controllers
{
    public class ProjekteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ProjektPaketeModel projektPaketeView;
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> _userManager;

        public ProjekteController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projekte
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projekte.ToListAsync());
        }

        // GET: Projekte/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                List<ArbeitsPaketModel> Pakete = _context.ArbeitsPaket.ToList();
                List<ProjekteModel> Projekte = _context.Projekte.ToList();
                List<ProjekteArbeitsPaketeViewModel> ProjektPakete = _context.ProjekteArbeitsPaketeViewModel.ToList();

                var projektDetails = from _projekte in Projekte
                                        where _projekte.ProjekteId == id
                                        join _projektPakete in ProjektPakete
                                        on _projekte.ProjekteId equals _projektPakete.ProjekteId into table1
                                        from _projektPakete in table1.ToList()
                                        join _pakete in Pakete
                                        on _projektPakete.ArbeitsPaketId equals _pakete.ArbeitsPaketId into table2
                                        from _pakete in table2.ToList()
                                        select new ProjektPaketeModel
                                        {
                                            Pakete = _pakete,
                                            Projekte = _projekte,
                                            ProjektPakete = _projektPakete
                                        };
                return View(projektDetails);
            }
        }

        // GET: Projekte/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreatePackage(int id)
        {
            return View(_context.Projekte.Find(id));
        }

        // POST: ArbeitsPaket/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePackage([Bind("ArbeitsPaketId,PaketName,Beschreibung,Mitglieder,Frist,Status")] ArbeitsPaketModel arbeitsPaket, int pId)
        {
            if (ModelState.IsValid)
            {
                arbeitsPaket.Status = "To do";
                _context.Add(arbeitsPaket);
                await _context.SaveChangesAsync();
                ProjekteArbeitsPaketeViewModel pp = new ProjekteArbeitsPaketeViewModel();
                pp.ProjekteId = pId;
                pp.ArbeitsPaketId = arbeitsPaket.ArbeitsPaketId;
                _context.Add(pp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(arbeitsPaket);
        }

        // POST: Projekte/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Projektname,Projektersteller,Mitglieder,Beschreibung,Deadline,Status")]  IvA.Models.ProjekteModel projekte)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projekte);

                projekte.ErstelltAm = DateTime.Now;
                projekte.Status = "To Do";
                projekte.Mitglieder = "";
                projekte.Projektersteller = this.User.Identity.Name;

                var newProject = await _context.SaveChangesAsync();
                
                // Dummy Paket verknüpfen
                ProjekteArbeitsPaketeViewModel projectPakage = new ProjekteArbeitsPaketeViewModel();
                projectPakage.ProjekteId = projekte.ProjekteId;
                projectPakage.ArbeitsPaketId = 8;
                _context.Add(projectPakage);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(projekte);
        }

        public async Task<IActionResult> AddUserToProject(String? nameInput)
        {
            if(nameInput != null)
            {
                IdentityUser newUser = await _userManager.FindByNameAsync(nameInput);
            }
            
            return NotFound();
        }

        public async Task<IActionResult> ProjectUserList()
        {
            List<ProjekteUserViewModel> projectUsers =  _context.ProjekteUserViewModel.ToList();
            return View(projectUsers);
        }

        // GET: Projekte/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IvA.Models.ProjekteModel projekte = await _context.Projekte.FindAsync(id);
            if (projekte == null)
            {
                return NotFound();
            }
            return View(projekte);
        }

        // POST: Projekte/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjekteId,Projektname,Projektersteller,ErstelltAm,Mitglieder,Beschreibung,Deadline,Status")]  IvA.Models.ProjekteModel projekte)
        {
          
            if (id != projekte.ProjekteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid && projekte.Projektersteller != null)
            {
                var owner = await _userManager.FindByNameAsync(projekte.Projektersteller);
                if (owner != null)
                {
                    try
                    {
                        _context.Update(projekte);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProjekteExists(projekte.ProjekteId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Hier gescheite Fehlermeldung einfügen wenn der User nicht existiert
                    return NotFound();
                }
            }
            return View(projekte);
        }

        // GET: Projekte/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projekte = await _context.Projekte
                .FirstOrDefaultAsync(m => m.ProjekteId == id);
            if (projekte == null)
            {
                return NotFound();
            }

            return View(projekte);
        }

        // POST: Projekte/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projekte = await _context.Projekte.FindAsync(id);
            if(projekte != null)
            {
                _context.Projekte.Remove(projekte);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        private bool ProjekteExists(int id)
        {
            return _context.Projekte.Any(e => e.ProjekteId == id);
        }
    }
}
