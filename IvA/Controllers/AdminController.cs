using IvA.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Controllers
{
    // Controller für Seiten des Adminpanels
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(RoleManager<IdentityRole> roleManager,
                                UserManager<IdentityUser> userManager,
                                ApplicationDbContext context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            _context = context;
        }

        // Lädt eine Liste mit allen Nutzer der Anwendung und gibt diese an den View.
        [Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        // Lädt eine Liste mit allen Projekten der Anwendung und gibt diese an einen View.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListProjects()
        {
            return View(await _context.Projekte.ToListAsync());
        }

        // Löscht den Account eines Nutzers.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                var id = await userManager.GetUserIdAsync(user);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{id}'.");
                }
            }

            return RedirectToAction("ListUsers");
        }

        // Alle Projekte die einem Nutzer zugeordnet sind werden geladen und mit der jeweiligen Rolle des Nutzers an den View geliefert.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListUserProjects(string userId)
        {
            var roles = _context.ProjectRoles.ToList().Where(user => user.UserId == userId);
            var user = await userManager.FindByIdAsync(userId);
            ViewBag.Name = user.UserName;
            return View(roles);
        }

        // Die seitenübergreifende Rolle einer Person wird von Nutzer zu Admin oder umgekehrt geändert.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(string Id)
        {
            if (Id != null)
            {
                var user = await userManager.FindByIdAsync(Id);
                if (user != null)
                {
                    if (await userManager.IsInRoleAsync(user, "Nutzer"))
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                        if (await userManager.IsInRoleAsync(user, "Admin"))
                        {
                            await userManager.RemoveFromRoleAsync(user, "Nutzer");
                        }
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user, "Nutzer");
                        if (await userManager.IsInRoleAsync(user, "Nutzer"))
                        {
                            await userManager.RemoveFromRoleAsync(user, "Admin");
                        }
                    }
                }
            }
            return RedirectToAction("ListUsers");
        }
    }
}