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
using System.Dynamic;
using IvA.Validation;

namespace IvA.Controllers
{
    public class ProjekteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private Helper helper;

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
                    Packages = packages.ToList(),
                    Project = project,
                    ProjectUsers = userList,
                    ProjectProgress = percentages,
                    Roles = UserRoles
                };

                List<int> ProjektZeitBudget = (from P in Pakete where P.ProjektId == id select P.Zeitbudget).ToList();
                int Zeit = ProjektZeitBudget.Sum();
                ViewBag.Zeitbudget = Zeit;

                return View(pDetailModel);
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

                // Projektersteller zum Owner ernnen
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

        public IActionResult AddUser()
        {
            int RoutingID = Int32.Parse((string)RouteData.Values["id"]);
            ViewBag.RoutingID = RoutingID;

            return View();
        }

        public async Task<IActionResult> AddUserToProject([Bind("id,name")] AddUserModel userToProject)
        {
            var roles = _context.ProjectRoles.ToList().FindAll(i => i.ProjectId == userToProject.id);
            ProjectRoles owner = roles.Find(o => o.ProjectRole == "Owner");
            IdentityUser user = await _userManager.FindByIdAsync(owner.UserId);
            if (user.UserName != this.User.Identity.Name)
            {
                return (RedirectToAction("ErrorMessage", new { ID = 1 }));
            }

            if (userToProject.name != null)
            {
                IdentityUser newUser = await _userManager.FindByNameAsync(userToProject.name);
                if (newUser != null)
                {
                    ProjekteUserViewModel newUserInProject = new ProjekteUserViewModel() {
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

        //Martin??????
        public async Task<IActionResult> ProjectUserList(int id)
        {
            List<ProjekteUserViewModel> projectUsers =  _context.ProjekteUserViewModel.ToList();
            List<IdentityUser> users = new List<IdentityUser>();
            foreach(ProjekteUserViewModel u in projectUsers)
            {
                if(u.ProjekteId == id)
                {
                    string userId = u.UserId;
                    users.Add(await _userManager.FindByIdAsync(userId));
                }
            }
            return View(users);
        }

        public async Task<IActionResult> PackageUserList(int id)
        {
            List<PaketeUserViewModel> projectUsers = _context.PaketeUserViewModel.ToList();
            List<IdentityUser> packageUsers = new List<IdentityUser>();
            foreach(PaketeUserViewModel user in projectUsers)
            {
                if (user.ArbeitsPaketId == id)
                {
                    var member = await _userManager.FindByIdAsync(user.UserId);
                    packageUsers.Add(member);
                }
            }
            return View(packageUsers);
        }

        public async Task<IActionResult> AddUserToPackage(int id, string name)
        {
            
            if (name != null)
            {
                IdentityUser newUser = await _userManager.FindByNameAsync(name);
                if(newUser != null)
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

        public async Task<IActionResult> DeleteUserFromProject(string name, int id)
        {
            if (name != null)
            {
                IdentityUser user = await _userManager.FindByNameAsync(name);
                if (user != null)
                {
                    List<ProjekteUserViewModel> userList = _context.ProjekteUserViewModel.ToList().FindAll(i => i.ProjekteId == id);
                    ProjekteUserViewModel projectUser = userList.Find(n => n.UserId == user.Id);
                    if(projectUser != null)
                    {
                        _context.ProjekteUserViewModel.Remove(projectUser);
                        DeleteUserFromProjectRoles(name, id);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", "Projekte", new { id = id });
                    }
                }
            }
            return NotFound("Error beim entfernen eines Nutzers");
        }

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

        public async Task<IActionResult> ChangeProjectRole(string name, int id, string role)
        {
            ChangeUserProjectRole(name, id, role);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Projekte", new { id = id });
        }

        public async Task<IActionResult> ChangeOwnership(string newOwner, int id)
        {
            var loggedUser = await _userManager.GetUserAsync(this.User);
            ChangeUserProjectRole(loggedUser.Id, id, "Nutzer");
            ChangeUserProjectRole(newOwner, id, "Owner");
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Projekte", new { id = id });
        }

        ///------------------------------ Projekt anpassen --------------------------------------

        // Gibt die Html-Datei Projekte/Edit anhand der übergebenen ProjektID zurück
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roles = _context.ProjectRoles.ToList().FindAll(i => i.ProjectId == id);
            ProjectRoles owner = roles.Find(o => o.ProjectRole == "Owner");
            IdentityUser user = await _userManager.FindByIdAsync(owner.UserId);
            if (user.UserName != this.User.Identity.Name)
            {
                return (RedirectToAction("ErrorMessage", new {ID = 1 }));
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
                    return (RedirectToAction("ErrorMessage", new { ID = 4 })); ;
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

            List<ProjekteModel> Projekte = _context.Projekte.ToList();
            var EditValid = (from p in Projekte where p.ProjekteId == id select p.Projektersteller).FirstOrDefault();
            if (EditValid != this.User.Identity.Name)
            {
                return (RedirectToAction("ErrorMessage", new { ID = 2 })); ;
            }

            var projekte = await _context.Projekte
                .FirstOrDefaultAsync(m => m.ProjekteId == id);
            if (projekte == null)
            {
                return NotFound();
            }

            int RoutingID = Int32.Parse((string)RouteData.Values["id"]);
            ViewBag.RoutingID = RoutingID;

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
        public async Task<IActionResult> PaketErstellen ([Bind("ArbeitsPaketId,PaketName,Beschreibung,Mitglieder,Zeitbudget,Frist,Status")]ArbeitsPaketModel arbeitsPaket, ProjekteArbeitsPaketeViewModel papv, int pId)
        {
            if (ModelState.IsValid)
            {
                var ProId = RouteData.Values["id"];

                arbeitsPaket.Status = "To do";
                arbeitsPaket.ProjektId = Int32.Parse((string)ProId);

                var Projects = _context.Projekte.ToList();
                var Deadline = (from p in Projects where p.ProjekteId == Int32.Parse((string)ProId) select p.Deadline).FirstOrDefault();
                if (arbeitsPaket.Frist >= Deadline)
                {
                    return (RedirectToAction("ErrorMessage", new { ID = 3 })); ;
                }
                if (arbeitsPaket.Zeitbudget < 0)
                {
                    int ErrorID = 5;
                    return (RedirectToAction("ErrorMessage", new { ID = ErrorID })); ;
                }

                _context.Add(arbeitsPaket);
                await _context.SaveChangesAsync();

                List<ArbeitsPaketModel> Pakete = _context.ArbeitsPaket.ToList();
                papv.ProjekteId = Int32.Parse((string)ProId);
                papv.ArbeitsPaketId = arbeitsPaket.ArbeitsPaketId;
                _context.Add(papv);

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
            int RoutingID = Int32.Parse((string)RouteData.Values["id"]);
            ViewBag.RoutingID = RoutingID;

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

            var package = await _context.ArbeitsPaket.FirstOrDefaultAsync(m => m.ArbeitsPaketId == id);
            if (package == null)
            {
                return NotFound();
            }

            List<IdentityUser> userList = new List<IdentityUser>();
            var packageUsers =  _context.PaketeUserViewModel.ToList().FindAll(m => m.ArbeitsPaketId == package.ArbeitsPaketId);
            foreach(PaketeUserViewModel user in packageUsers)
            {
                userList.Add(await _userManager.FindByIdAsync(user.UserId));
            }

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

            List<ProjectRoles> UserRoles = _context.ProjectRoles.ToList();

            PackagesDetailModel packagesDetails = new PackagesDetailModel
            {
                ProjectUsers = projectUserList,
                Package = package,
                PackageUsers = userList,
                Roles = UserRoles
            };

            int RoutingID = packagesDetails.Package.ProjektId;
            ViewBag.RoutingID = RoutingID;

            return View(packagesDetails);
        }

        //------------------------------ Paket anpassen --------------------------------------

        // Gibt die Html-Datei PaketAnpassen anhand der übergebenen PaketID zurück
        public async Task<IActionResult> PaketAnpassen(int? id)
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
            List<ProjekteArbeitsPaketeViewModel> ProjektPakete = _context.ProjekteArbeitsPaketeViewModel.ToList();

            var arbeitsPaket = await _context.ArbeitsPaket.FindAsync(id);
            _context.ArbeitsPaket.Remove(arbeitsPaket);

            int tableID = (from table in ProjektPakete where table.ArbeitsPaketId == id select table.ProjekteArbeitsPaketeViewModelId).FirstOrDefault();
            var proPakViewModel = await _context.ProjekteArbeitsPaketeViewModel.FindAsync(tableID);

            _context.ProjekteArbeitsPaketeViewModel.Remove(proPakViewModel);

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
        */
        public async Task<IActionResult> ErrorMessage(int id) 
        {
            ErrorMessage message = new ErrorMessage();
            message.DynamicErrorMessage = Int32.Parse((string)RouteData.Values["id"]);

            return View(message);
        }

        // Mit Hilfe der Methode wird für ein spezifisches Arbeitspaket die verbrauchte Arbeitszeit eingetragen
        public async Task<IActionResult> PaketZeit(int? id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaketZeit(int id, [Bind("ArbeitsPaketId,ProjektId,PaketName,Beschreibung,Mitglieder,Zeitbudget,VerbrauchteZeit,Frist,Status")] ArbeitsPaketModel arbeitsPaket)
        {
            if (id != arbeitsPaket.ArbeitsPaketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                    var zeit = _context.ArbeitsPaket.AsNoTracking().Where(p => p.ArbeitsPaketId == id).FirstOrDefault();
                    arbeitsPaket.VerbrauchteZeit = arbeitsPaket.VerbrauchteZeit + zeit.VerbrauchteZeit;
                    
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
        public async void ChangeUserProjectRole(string userId, int projectId, string role)
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
        public async void DeleteUserFromProjectRoles(string userId, int projectId)
        {
            List<ProjectRoles> activeRoles = _context.ProjectRoles.ToList().FindAll(n => n.UserId == userId);
            activeRoles = activeRoles.FindAll(i => i.ProjectId == projectId);
            foreach(ProjectRoles role in activeRoles)
            {
                _context.ProjectRoles.Remove(role);
            }
            //await _context.SaveChangesAsync();
        }
    }
}
