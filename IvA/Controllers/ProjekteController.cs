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
using System.Runtime.InteropServices;

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

        //--------------------------------------------------------------------------------------------------------------------
        //Der folgende Abschnitt beinhaltet alle Methoden, die für das Erstellen und BEarbeiten von Projekten benötigt werden.
        //--------------------------------------------------------------------------------------------------------------------

        // Listet alle verfügbaren Projekte auf
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projekte.ToListAsync());
        }

        // Listet die Projektdetails für ein spezifisches Projekt anhand der übergebenen ID auf. 
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

                var projektDetails =    from _projekte in Projekte
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

        //------------------------------ Projekt erstellen ---------------------------------

        // GET: Gibt die Html-Datei Projekte/Create zurück, welche die Eingabemaske für die Projekterstellung zur Verfügung stellt.
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreatePackage(int id)
        {
            return View(_context.Projekte.Find(id));
        }

        // Martin watt is hier mit?
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

        /* Erstellt ein Projekt und fügt dieses der Tabelle "Projekte" hinzu. Projektersteller ist immmer der aktuell eingeloggte User. 
         * Das Datum der projekterstellung wird automatisch aus der Betriebsystemszeit generiert. Der Projektestatus ist anfangs immer "To Do".
           Nach Erstellung des Projektes wird der Nutzer auf die Indexseite zurückgeleitet.*/

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

        //Martin?
        public async Task<IActionResult> AddUserToProject(String? nameInput)
        {
            if(nameInput != null)
            {
                IdentityUser newUser = await _userManager.FindByNameAsync(nameInput);
            }
            
            return NotFound();
        }

        //Martin??????
        public async Task<IActionResult> ProjectUserList()
        {
            List<ProjekteUserViewModel> projectUsers =  _context.ProjekteUserViewModel.ToList();
            return View(projectUsers);
        }

        ///------------------------------ Paket anpassen --------------------------------------

        // Gibt die Html-Datei Projekte/Edit anhand der übergebenen ProjektID zurück
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

        // Speichert die in der Html-Datei Projekte/Edit übergebenen Werte in die Tabelle "Projekte" und leitet den Nutzer danach autoamtisch auf die Projektindexseite zurück
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
                    return View("~/Views/Projekte/Fehlermeldung.cshtml");
                }
            }
            return View(projekte);
        }

        //------------------------------ Projekt löschen --------------------------------

        //GET: Gibt die Html-Datei für das löschen von Arbeitspaketen wieder
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

        // POST: Entfernt einen Eintrag aus der Tabelle "Projekte" anhand der übergeben ProjektID und leitet den Nutzer danach autoamtisch auf die Projektindexseite zurück
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

        //Prüft, ob ein Eintrag in der Tabelle "Projekte" mit der entsprechenden ID vorhanden ist.
        private bool ProjekteExists(int id)
        {
            return _context.Projekte.Any(e => e.ProjekteId == id);
        }


        //-------------------------------------------------------------------------------------------------------------------------
        //Der folgende Abschnitt beinhaltet alle Methoden, die für das Erstellen und Bearbeiten von Arbeitspaketen benötigt werden.
        //-------------------------------------------------------------------------------------------------------------------------

        //---------------------------- Paket erstellen ----------------------

        // Die Methode erstellt ein Arbeitspaket und ordnet dieses autoomatisch dem richtig Projekt zu. Nach erfolgreicher Erstellung wird der Nutzer auf die entsprechende Detailansicht des Projektes zurückgeleitet.
        public async Task<IActionResult> PaketErstellen ([Bind("ArbeitsPaketId,PaketName,Beschreibung,Mitglieder,Frist,Status")]ArbeitsPaketModel arbeitsPaket, ProjekteArbeitsPaketeViewModel papv, int pId)
        {
            if (ModelState.IsValid)
            {
                var ProId = RouteData.Values["id"];

                arbeitsPaket.Status = "To do";
                arbeitsPaket.ProjektId = Int32.Parse((string)ProId);
                _context.Add(arbeitsPaket);
                await _context.SaveChangesAsync();

                List<ArbeitsPaketModel> Pakete = _context.ArbeitsPaket.ToList();
                papv.ProjekteId = Int32.Parse((string)ProId);
                papv.ArbeitsPaketId = /*Int32.Parse(from p in Pakete select p.ArbeitsPaketId).ToString();*/ arbeitsPaket.ArbeitsPaketId;
                _context.Add(papv);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Projekte", new { id = ProId });
            }
            return View(arbeitsPaket);
        }

        //---------------------- PaketDetails ----------------------------------------

        // Gibt die Html-Datei PaketDetails anhand der übergebenen PaketID zurück
        public async Task<IActionResult> PaketDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arbeitsPaket = await _context.ArbeitsPaket.FirstOrDefaultAsync(m => m.ArbeitsPaketId == id);
            if (arbeitsPaket == null)
            {
                return NotFound();
            }

            return View(arbeitsPaket);
        }

        //------------------------------ Paket anpassen --------------------------------------

        // Gibt die Html-Datei PaketAnpassen anhand der übergebenen PaketID zurück
        public async Task<IActionResult> PaketAnpassen(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arbeitsPaket = await _context.ArbeitsPaket.FindAsync(id);
            if (arbeitsPaket == null)
            {
                return NotFound();
            }
            return View(arbeitsPaket);
        }
        // Speichert die in der Html-Datei PaketAnpassen übergebenen Werte in die Tabelle "ArbeitsPaket" und leitet den Nutzer danach autoamtisch auf die Projektdetailseite zurück
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaketAnpassen(int id, [Bind("ArbeitsPaketId,ProjektId,PaketName,Beschreibung,Mitglieder,Frist,Status")] ArbeitsPaketModel arbeitsPaket)
        {
            if (id != arbeitsPaket.ArbeitsPaketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arbeitsPaket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArbeitsPaketExists(arbeitsPaket.ArbeitsPaketId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Projekte", new { id = arbeitsPaket.ProjektId });
            }
            return View(arbeitsPaket);
        }

        //Prüft, ob ein Eintrag in der Tabelle "ArbeitsPaket" mit der entsprechenden ID vorhanden ist.
        private bool ArbeitsPaketExists(int id)
        {
            return _context.ArbeitsPaket.Any(e => e.ArbeitsPaketId == id);
        }
        
        //------------------- Arbeitspakete löschen --------------------

        //GET: Gibt die Html-Datei für das löschen von Arbeitspaketen wieder
        public async Task<IActionResult> PaketLöschenGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arbeitsPaket = await _context.ArbeitsPaket
                .FirstOrDefaultAsync(m => m.ArbeitsPaketId == id);
            if (arbeitsPaket == null)
            {
                return NotFound();
            }

            return View(arbeitsPaket);
        }
        // POST: Entfernt einen Eintrag aus der Tabelle "ArbeitsPaket" anhand der übergeben PaketID und leitet den Nutzer danach autoamtisch auf die Projektdetailseite zurück
        [HttpPost, ActionName("PaketLöschenGet")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaketLöschenPost(int id)
        {
            var arbeitsPaket = await _context.ArbeitsPaket.FindAsync(id);
            _context.ArbeitsPaket.Remove(arbeitsPaket);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Projekte", new { id = arbeitsPaket.ProjektId });
        }

    }
}
