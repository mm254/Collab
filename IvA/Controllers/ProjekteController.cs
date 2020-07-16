using IvA.Data;
using IvA.Models;
using IvA.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Controllers
{
    // Controller der die
    [Authorize(Roles = "Admin,Nutzer")]
    public class ProjekteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private Helper helper;

        // Instanz der Datenbankklasse wird erstellt und per Dependency Injection zugewiesen.
        public ProjekteController(ApplicationDbContext context,
                                  UserManager<IdentityUser> userManager,
                                  SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            helper = new Helper();
        }

        //--------------------------------------------------------------------------------------------------------------------
        //Der folgende Abschnitt beinhaltet alle Methoden, die für das Erstellen und Bearbeiten von Projekten benötigt werden.
        //--------------------------------------------------------------------------------------------------------------------

        // Listet alle Projekte des Nutzers auf
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> Index()
        {
            var loggedUser = await _userManager.GetUserAsync(this.User);
            List<ProjekteUserViewModel> UserProjects = _context.ProjekteUserViewModel.ToList();
            var Projects = _context.Projekte.ToList();
            var projectList = from _projects in Projects
                              join _userProjects in UserProjects
                              on _projects.ProjekteId equals _userProjects.ProjekteId
                              where _userProjects.UserId == loggedUser.Id
                              select new ProjekteModel
                              {
                                  ProjekteId = _projects.ProjekteId,
                                  Beschreibung = _projects.Beschreibung,
                                  Deadline = _projects.Deadline,
                                  Mitglieder = _projects.Mitglieder,
                                  Projektname = _projects.Projektname,
                                  ErstelltAm = _projects.ErstelltAm,
                                  Status = _projects.Status,
                                  Projektersteller = _projects.Projektersteller
                              };
            return View(projectList);
        }

        // Listet die Projektdetails für ein spezifisches Projekt anhand der übergebenen ID auf. 
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                // Laden aller nötigen Tabellen aus der Datenbank
                List<ArbeitsPaketModel> Pakete = _context.ArbeitsPaket.ToList();
                List<ProjekteModel> Projekte = _context.Projekte.ToList();
                List<ProjekteArbeitsPaketeViewModel> ProjektPakete = _context.ProjekteArbeitsPaketeViewModel.ToList();
                List<ProjekteUserViewModel> Users = _context.ProjekteUserViewModel.ToList();
                List<ProjectRoles> UserRoles = _context.ProjectRoles.ToList();

                // Liste an IdentityUser der Projektmitglieder wird erstellt
                List<IdentityUser> userList = new List<IdentityUser>();
                foreach (ProjekteUserViewModel user in Users)
                {
                    if (user.ProjekteId == id)
                    {
                        userList.Add(await _userManager.FindByIdAsync(user.UserId));
                    }
                }

                // Überprüfen ob Nutzer Teil des Projektes oder Admin ist und die Detailansicht sehen darf
                var loggedInUser = await _userManager.GetUserAsync(this.User);
                bool isAdmin = await _userManager.IsInRoleAsync(loggedInUser, "Admin");
                if (!userList.Contains(loggedInUser) && !isAdmin)
                {
                    return (RedirectToAction("ErrorMessage", new { ID = 5 }));
                }

                // Schnitt aus drei Tabellen um alle zu einem Projekt zugehörigen Pakete zu erhalten
                var packages = from _projekte in Projekte
                               where _projekte.ProjekteId == id
                               join _projektPakete in ProjektPakete
                               on _projekte.ProjekteId equals _projektPakete.ProjekteId into table1

                               from _projektPakete in table1.ToList()
                               join _pakete in Pakete
                               on _projektPakete.ArbeitsPaketId equals _pakete.ArbeitsPaketId into table2

                               from _pakete in table2.ToList()
                               select new ArbeitsPaketModel
                               {
                                   ArbeitsPaketId = _pakete.ArbeitsPaketId,
                                   ProjektId = _pakete.ProjektId,
                                   PaketName = _pakete.PaketName,
                                   Beschreibung = _pakete.Beschreibung,
                                   Mitglieder = _pakete.Mitglieder,
                                   Frist = _pakete.Frist,
                                   Status = _pakete.Status
                               };
                ProjekteModel project = Projekte.Find(m => m.ProjekteId == id);

                // Anhand der Liste der Pakete werden drei Prozentwerte ermittelt die den Projektfortschritt wiedergeben
                var percentages = helper.CalculatePercentages(packages.ToList());


                // Erstellen des finalen Models
                ProjekteDetailModel pDetailModel = new ProjekteDetailModel
                {
                    Packages = packages.ToList(), // Alle dem Projekt zugeörigen Pakete
                    Project = project,            // Das Projekt mit den Details
                    ProjectUsers = userList,      // Liste an Mitgliedern
                    ProjectProgress = percentages, // Werte für den Fortschrittsbalken
                    Roles = UserRoles           // Rollen der Mitglieder
                };

                // Aus allen Paketen werden die Angaben zu geplanten sowie bereits verbuchten Stunden addiert und mittels ViewBag an den View weitergeleitet
                List<int> listTimeBudget = (from P in Pakete where P.ProjektId == id select P.Zeitbudget).ToList();
                List<int> listTimeUsed = (from P in Pakete where P.ProjektId == id select P.VerbrauchteZeit).ToList();
                int timeBudget = listTimeBudget.Sum();
                int timeUsed = listTimeUsed.Sum();
                ViewBag.timeBudget = timeBudget;
                ViewBag.timeUsed = timeUsed;

                return View(pDetailModel);
            }
        }

        //------------------------------ Projekt erstellen ---------------------------------

        // GET: Gibt die Html-Datei Projekte/Create zurück, welche die Eingabemaske für die Projekterstellung zur Verfügung stellt.
        [Authorize(Roles = "Admin,Nutzer")]
        public IActionResult Create()
        {
            return View();
        }

        /* Erstellt ein Projekt und fügt dieses der Tabelle "Projekte" hinzu. Projektersteller ist immmer der aktuell eingeloggte User. 
         * Das Datum der projekterstellung wird automatisch aus der Betriebsystemszeit generiert. Der Projektestatus ist anfangs immer "To Do".
           Nach Erstellung des Projektes wird der Nutzer auf die Indexseite zurückgeleitet.*/

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> Create([Bind("Id,Projektname,Projektersteller,Mitglieder,Beschreibung,Deadline,Status")]  IvA.Models.ProjekteModel projekte)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projekte);

                //Status, Erstellungdatum, Mitglieder und Projektersteller werden automatisch gesetzt
                projekte.ErstelltAm = DateTime.Now;
                projekte.Status = "To Do";
                projekte.Mitglieder = "";
                projekte.Projektersteller = this.User.Identity.Name;

                await _context.SaveChangesAsync();

                // Projektersteller als Mitglied setzen
                var userClaim = this.User;
                var loggedUser = await _userManager.GetUserAsync(userClaim);
                ProjekteUserViewModel firstMember = new ProjekteUserViewModel
                {
                    ProjekteId = projekte.ProjekteId,
                    UserId = loggedUser.Id
                };
                _context.Add(firstMember);

                // Projektersteller zum Owner ernennen
                ProjectRoles owner = new ProjectRoles()
                {
                    ProjectId = projekte.ProjekteId,
                    ProjectRole = "Owner",
                    UserId = loggedUser.Id
                };
                _context.Add(owner);

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Projekte", new { id = projekte.ProjekteId });
            }
            return View(projekte);
        }

        // Gibt die HTML-Datei für die Nutzer hinzufügen Ansicht zurück
        [Authorize(Roles = "Admin,Nutzer")]
        public IActionResult AddUser()
        {
            // RoutingID, um auf die Projektdetailseite zurückzugelangen
            int RoutingID = Int32.Parse((string)RouteData.Values["id"]);
            ViewBag.RoutingID = RoutingID;

            return View();
        }

        //Ordnet einen Nutzer einem Projekt zu
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> AddUserToProject([Bind("id,name")] AddUserModel userToProject)
        {
            // Die Methode steht nur Projektownern oder Admins zur Verfügung, sonst erscheit eine Fehlermeldung
            var roles = _context.ProjectRoles.ToList().FindAll(i => i.ProjectId == userToProject.id);
            ProjectRoles owner = roles.Find(o => o.ProjectRole == "Owner");
            IdentityUser user = await _userManager.FindByIdAsync(owner.UserId);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (user.UserName != this.User.Identity.Name && !isAdmin)
            {
                return (RedirectToAction("ErrorMessage", new { ID = 7 }));
            }

            // Die Zuorndung eines Nutzers zu einem Projekt wird in der Tabelle ProjekteUserViewModel gespeichert
            if (userToProject.name != null)
            {
                IdentityUser newUser = await _userManager.FindByNameAsync(userToProject.name);
                if (newUser != null)
                {
                    ProjekteUserViewModel newUserInProject = new ProjekteUserViewModel()
                    {
                        ProjekteId = userToProject.id,
                        UserId = newUser.Id
                    };
                    _context.Add(newUserInProject);

                    ChangeUserProjectRole(newUser.Id, userToProject.id, "Nutzer");

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Projekte", new { id = userToProject.id });

                }
                else
                {
                    return (RedirectToAction("ErrorMessage", new { ID = 6 }));
                }
            }
            return NotFound("Error beim Hinzufügen eines Projekts");
        }

        //Fügt einem Arbeitspaket einen Nutzer hinzu. Der Nutzer muss einen Nutzeraccount besitzen
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> AddUserToPackage(int id, string name)
        {

            if (name != null)
            {
                IdentityUser newUser = await _userManager.FindByNameAsync(name);
                if (newUser != null)
                {
                    PaketeUserViewModel newMember = new PaketeUserViewModel
                    {
                        ArbeitsPaketId = id,
                        UserId = newUser.Id
                    };
                    _context.Add(newMember);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("PaketDetails", "Projekte", new { id = id });
                }
            }
            return NotFound("Fehler beim Hinzufügen");
        }

        // Die Methode entfernt einen Nutzer aus einem Projekt
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> DeleteUserFromProject(string name, int id)
        {
            if (name != null)
            {
                IdentityUser user = await _userManager.FindByNameAsync(name);
                if (user != null)
                {
                    List<ProjekteUserViewModel> userList = _context.ProjekteUserViewModel.ToList().FindAll(i => i.ProjekteId == id);
                    ProjekteUserViewModel projectUser = userList.Find(n => n.UserId == user.Id);
                    if (projectUser != null)
                    {
                        // Überprüfen ob die zu entfernen Person der Owner ist. Wenn ja kommt eine Fehlermeldung
                        var projectRoles = _context.ProjectRoles.ToList().Where(x => x.ProjectId == id);
                        var owner = projectRoles.Where(x => x.ProjectRole == "Owner").First();
                        var ownerUser = await _userManager.FindByIdAsync(owner.UserId);
                        if (!ownerUser.UserName.Equals(name))
                        {
                            _context.ProjekteUserViewModel.Remove(projectUser);
                            DeleteUserFromProjectRoles(name, id);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Details", "Projekte", new { id = id });
                        }
                        else
                        {
                            return (RedirectToAction("ErrorMessage", new { ID = 8 }));
                        }
                    }
                }
            }
            return NotFound("Error beim entfernen eines Nutzers");
        }

        // Die Methode entfernt einen Nutzer aus einem Arbeitspaket
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> DeleteUserFromPackage(string name, int id)
        {
            if (name != null)
            {
                IdentityUser user = await _userManager.FindByNameAsync(name);
                if (user != null)
                {
                    List<PaketeUserViewModel> userList = _context.PaketeUserViewModel.ToList().FindAll(i => i.ArbeitsPaketId == id);
                    PaketeUserViewModel packageUser = userList.Find(n => n.UserId == user.Id);
                    if (packageUser != null)
                    {
                        _context.PaketeUserViewModel.Remove(packageUser);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("PaketDetails", "Projekte", new { id = id });
                    }
                }
            }
            return NotFound("Error beim entfernen eines Nutzers");
        }

        //Die Methode ermöglicht das Ändern der Rollenverteilung innerhalb eines Projektes
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> ChangeProjectRole(string name, int id, string role)
        {
            ChangeUserProjectRole(name, id, role);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Projekte", new { id = id });
        }

        //Die Methode ermöglicht das wechseln des Projektowners
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> ChangeOwnership(string newOwner, int id)
        {
            var projectRoles = _context.ProjectRoles.ToList().Where(x => x.ProjectId == id);
            var owner = projectRoles.Where(x => x.ProjectRole == "Owner").First();
            var oldOwner = await _userManager.FindByIdAsync(owner.UserId);
            ChangeUserProjectRole(oldOwner.Id, id, "Nutzer");
            ChangeUserProjectRole(newOwner, id, "Owner");
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Projekte", new { id = id });
        }

        ///------------------------------ Projekt anpassen --------------------------------------

        // Gibt die Html-Datei Projekte/Edit anhand der übergebenen ProjektID zurück
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Die Bearbeitenfunktion für Projekte steht nur Admins und Projektowner zur Verfügung
            var roles = _context.ProjectRoles.ToList().FindAll(i => i.ProjectId == id);
            ProjectRoles owner = roles.Find(o => o.ProjectRole == "Owner");
            IdentityUser user = await _userManager.FindByIdAsync(owner.UserId);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            // Fehlermeldung, falls der User kein Admin oder Projektowner ist
            if (user.UserName != this.User.Identity.Name && !isAdmin)
            {
                return (RedirectToAction("ErrorMessage", new { ID = 1 }));
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
        [Authorize(Roles = "Admin,Nutzer")]
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
                    return RedirectToAction("Details", "Projekte", new { id = id });
                }
                else
                {
                    return (RedirectToAction("ErrorMessage", new { ID = 4 })); ;
                }
            }
            return View(projekte);
        }

        //------------------------------ Projekt löschen --------------------------------

        //GET: Gibt die Html-Datei für das löschen von Arbeitspaketen wieder
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<ProjekteModel> Projekte = _context.Projekte.ToList();
            var EditValid = (from p in Projekte where p.ProjekteId == id select p.Projektersteller).FirstOrDefault();

            var projekte = await _context.Projekte
                .FirstOrDefaultAsync(m => m.ProjekteId == id);
            if (projekte == null)
            {
                return NotFound();
            }

            //RoutingID, um von der Löschenfunktion zurück zur Projektdetailansicht zurückzugelangen
            int RoutingID = Int32.Parse((string)RouteData.Values["id"]);
            ViewBag.RoutingID = RoutingID;

            return View(projekte);
        }

        // POST: Entfernt einen Eintrag aus der Tabelle "Projekte" anhand der übergeben ProjektID und leitet den Nutzer danach autoamtisch auf die Projektübersichtsseite zurück
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Nutzer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projekte = await _context.Projekte.FindAsync(id);
            if (projekte != null)
            {
                //Löscht das spezifische Projekt
                _context.Projekte.Remove(projekte);

                //Löscht alle Arbeitspakete, die einem Projekt zugeordnet sind
                List<ArbeitsPaketModel> Pakete = _context.ArbeitsPaket.ToList();
                List<ArbeitsPaketModel> PaketLöschen = (from P in Pakete where P.ProjektId == id select P).ToList();

                _context.ArbeitsPaket.RemoveRange(PaketLöschen);

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
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> PaketErstellen([Bind("ArbeitsPaketId,PaketName,Beschreibung,Mitglieder,Zeitbudget,Frist,Status")]ArbeitsPaketModel arbeitsPaket, ProjekteArbeitsPaketeViewModel ProPaViewMo, int pId)
        {
            if (ModelState.IsValid)
            {
                // EInem Arbeitspaket wird die ProjektID anhand der in der URL übergebenen ID zugeordnet
                var ProId = RouteData.Values["id"];

                arbeitsPaket.Status = "To do";
                arbeitsPaket.ProjektId = Int32.Parse((string)ProId);

                //Deadline eines spezifischen Projektes auswählen
                var Projects = _context.Projekte.ToList();
                var Deadline = (from p in Projects where p.ProjekteId == Int32.Parse((string)ProId) select p.Deadline).FirstOrDefault();

                //Fehlermeldung, wenn die Frist eines Arbeitspaketes später als die Deadline des zugeordneten Projektes gewählt wird
                if (arbeitsPaket.Frist >= Deadline)
                {
                    return (RedirectToAction("ErrorMessage", new { ID = 3 })); ;
                }
                //Fehlermeldung, wenn das Zeitbudget eines Arbeitspaketes negativ gewählt wird
                if (arbeitsPaket.Zeitbudget < 0)
                {
                    int ErrorID = 5;
                    return (RedirectToAction("ErrorMessage", new { ID = ErrorID })); ;
                }

                //Angabe über ein erstelltes Arbeitspaket persitent in der Tabele ArbeitsPaket speichern
                _context.Add(arbeitsPaket);
                await _context.SaveChangesAsync();

                // Einem neuen Arbeitspaket wird die passende ProjektID zugeordnet, ProjektID und ArbeitsPaketID werden als Zuordnung in der Tabelle ProjekteArbeitsPaketViewModel gespeichert
                List<ArbeitsPaketModel> Pakete = _context.ArbeitsPaket.ToList();
                ProPaViewMo.ProjekteId = Int32.Parse((string)ProId);
                ProPaViewMo.ArbeitsPaketId = arbeitsPaket.ArbeitsPaketId;
                _context.Add(ProPaViewMo);

                // Ersteller des Pakets wird als erstes Mitglied eingetragen
                var currentUser = this.User;
                string currentUserId = _userManager.GetUserId(currentUser);
                PaketeUserViewModel newMember = new PaketeUserViewModel
                {
                    ArbeitsPaketId = arbeitsPaket.ArbeitsPaketId,
                    UserId = currentUserId
                };
                _context.Add(newMember);

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Projekte", new { id = ProId });
            }

            //RoutingID wird an die View übergeben, um auf die Projektdetailansicht zurückzugelangen
            int RoutingID = Int32.Parse((string)RouteData.Values["id"]);
            ViewBag.RoutingID = RoutingID;

            return View(arbeitsPaket);
        }

        //---------------------- PaketDetails ----------------------------------------

        // Gibt die Html-Datei PaketDetails anhand der übergebenen PaketID zurück
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> PaketDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.ArbeitsPaket.FirstOrDefaultAsync(m => m.ArbeitsPaketId == id);
            if (package == null)
            {
                return NotFound();
            }

            // Liste der zum Paket zugewiesenen Nutzer wird generiert
            List<IdentityUser> userList = new List<IdentityUser>();
            var packageUsers = _context.PaketeUserViewModel.ToList().FindAll(m => m.ArbeitsPaketId == package.ArbeitsPaketId);
            foreach (PaketeUserViewModel user in packageUsers)
            {
                userList.Add(await _userManager.FindByIdAsync(user.UserId));
            }

            // Liste der Nutzer, die Teil des Projekts sind aber (noch) nicht zum Paket geordnet sind
            List<IdentityUser> projectUserList = new List<IdentityUser>();
            var projectUsers = _context.ProjekteUserViewModel.ToList().FindAll(a => a.ProjekteId == package.ProjektId);
            foreach (ProjekteUserViewModel users in projectUsers)
            {
                IdentityUser projectUser = await _userManager.FindByIdAsync(users.UserId);
                if (!userList.Contains(projectUser))
                {
                    projectUserList.Add(projectUser);
                }
            }

            // Laden der projektinternen Rollen
            List<ProjectRoles> UserRoles = _context.ProjectRoles.ToList();

            PackagesDetailModel packagesDetails = new PackagesDetailModel
            {
                ProjectUsers = projectUserList,
                Package = package,
                PackageUsers = userList,
                Roles = UserRoles
            };

            // ID für das Routing zurück auf die Paketdetailansicht
            int RoutingID = packagesDetails.Package.ProjektId;
            ViewBag.RoutingID = RoutingID;

            return View(packagesDetails);
        }

        //------------------------------ Paket anpassen --------------------------------------

        // Gibt die Html-Datei PaketAnpassen anhand der übergebenen PaketID zurück
        [Authorize(Roles = "Admin,Nutzer")]
        public IActionResult PaketAnpassen(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arbeitsPaket = _context.ArbeitsPaket.AsNoTracking().Where(p => p.ArbeitsPaketId == id).FirstOrDefault();
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
        [Authorize(Roles = "Admin,Nutzer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaketAnpassen(int id, [Bind("ArbeitsPaketId,ProjektId,PaketName,Beschreibung,Mitglieder,Zeitbudget,VerbrauchteZeit,Frist,Status")] ArbeitsPaketModel arbeitsPaket)
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
        [Authorize(Roles = "Admin,Nutzer")]
        private bool ArbeitsPaketExists(int id)
        {
            return _context.ArbeitsPaket.Any(e => e.ArbeitsPaketId == id);
        }

        //------------------- Arbeitspakete löschen --------------------

        //GET: Gibt die Html-Datei für das löschen eines spezfischen Arbeitspaketes wieder
        [Authorize(Roles = "Admin,Nutzer")]
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
        // POST: Entfernt einen Eintrag aus der Tabelle "ArbeitsPaket" anhand der übergeben PaketID und leitet den Nutzer danach automatisch auf die Projektdetailseite zurück
        [HttpPost, ActionName("PaketLöschenGet")]
        [Authorize(Roles = "Admin,Nutzer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaketLöschenPost(int id)
        {
            List<ProjekteArbeitsPaketeViewModel> ProjektPakete = _context.ProjekteArbeitsPaketeViewModel.ToList();

            //Entfernt das ausgewählte Arbeitspaket
            var arbeitsPaket = await _context.ArbeitsPaket.FindAsync(id);
            _context.ArbeitsPaket.Remove(arbeitsPaket);

            //Entfernt die Zuordnung eines pakets zu einem Projekt aus der Tabele ProjektePaketeViewModel
            int tableID = (from table in ProjektPakete where table.ArbeitsPaketId == id select table.ProjekteArbeitsPaketeViewModelId).FirstOrDefault();
            var proPakViewModel = await _context.ProjekteArbeitsPaketeViewModel.FindAsync(tableID);

            _context.ProjekteArbeitsPaketeViewModel.Remove(proPakViewModel);

            //Änderungen an der Datenbank speichern
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Projekte", new { id = arbeitsPaket.ProjektId });
        }

        /* 
        Action-Methode, welche per Redirect von einer anderen Methode im entsprechenden Fehlerfall aufgerufen wird. Es wird ein spezifische Zahl als Fehlercode übergeben.
        Abhängig vom Fehlercode wird eine spezifische Fehlermeldung ausgegeben 
        Fehlercode 1: Versuch, Projektdetails anzupassen, obwohl man nicht der Project-Owner ist.
        Fehlercode 2: Versuch, ein Projekt zu löschen, obwohl man nicht der Project-Owner ist.
        Fehlercode 3: Versuch, ein Arbeitspaket zu erstellen, dessen Frist nach Ablauf der Projektdeadline liegt.
        Fehlercode 4: Ein nicht existierender Projectowner wird einem Projekt zugewiesen.
        Fehlercode 5: Das Zeitbudget für ein Arbeitspaket darf nicht negativ sein
        Fehlercode 6: Ein Nutzer wurde nicht gefunden
        Fehlercode 7: Fehlende Berechtigung, um einen Nutzer zu einem Projekt hinzuzufügen
        Fehlercode 8: Die Verbrauchte Arbeitszeit für ein Arbeitspaket darf nicht negativ sein
        Fehlercode 9: Der Owner darf nicht aus dem Projekt entfernt werden
        */
        public IActionResult ErrorMessage(int id)
        {
            ErrorMessage message = new ErrorMessage();
            message.DynamicErrorMessage = Int32.Parse((string)RouteData.Values["id"]);

            return View(message);
        }

        // Mit Hilfe der Methode wird die Eingabemaske für die verbrauchte Arbeitszeit eines  spezifischen Arbeitspaketes aufgerufen
        [Authorize(Roles = "Admin,Nutzer")]
        public IActionResult PaketZeit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arbeitsPaket = _context.ArbeitsPaket.AsNoTracking().Where(p => p.ArbeitsPaketId == id).FirstOrDefault();
            if (arbeitsPaket == null)
            {
                return NotFound();
            }
            arbeitsPaket.VerbrauchteZeit = 0;
            return View(arbeitsPaket);
        }

        // Mit Hilfe der Methode wird die eingetragene Arbeitszeit für ein spezifisches Arbeitspaket persistent in der Tabelle ArbeitsPaket gespeichert
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Nutzer")]
        public async Task<IActionResult> PaketZeit(int id, [Bind("ArbeitsPaketId,ProjektId,PaketName,Beschreibung,Mitglieder,Zeitbudget,VerbrauchteZeit,Frist,Status")] ArbeitsPaketModel arbeitsPaket)
        {
            if (id != arbeitsPaket.ArbeitsPaketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //Die gespeicherte Arbeitszeit ist die Sume aus bisher verbrauchter Arbeitszeit + neuer verbrauchter Arbeitszeit.
                // Wird die Zeit korrigiert, kann sie nicht unter 0 sinken.
                try
                {

                    var zeit = _context.ArbeitsPaket.AsNoTracking().Where(p => p.ArbeitsPaketId == id).FirstOrDefault();
                    arbeitsPaket.VerbrauchteZeit = arbeitsPaket.VerbrauchteZeit + zeit.VerbrauchteZeit;
                    if (arbeitsPaket.VerbrauchteZeit < 0)
                    {
                        return (RedirectToAction("ErrorMessage", new { ID = 8 }));
                    }

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

                return RedirectToAction("PaketDetails", "Projekte", new { id });
            }

            return View(arbeitsPaket);
        }

        // Weißt einem Nutzer eine neue Rolle innerhalb eines Projektes zu
        public void ChangeUserProjectRole(string userId, int projectId, string role)
        {
            DeleteUserFromProjectRoles(userId, projectId);
            ProjectRoles newRole = new ProjectRoles()
            {
                ProjectId = projectId,
                ProjectRole = role,
                UserId = userId
            };
            _context.Add(newRole);
        }

        // Entfernt alle Rollen die einem User in einem Projekt zugewiesen sind
        public void DeleteUserFromProjectRoles(string userId, int projectId)
        {
            List<ProjectRoles> activeRoles = _context.ProjectRoles.ToList().FindAll(n => n.UserId == userId);
            activeRoles = activeRoles.FindAll(i => i.ProjectId == projectId);
            foreach (ProjectRoles role in activeRoles)
            {
                _context.ProjectRoles.Remove(role);
            }
        }
    }
}
