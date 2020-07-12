using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IvA.Data;
using IvA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IvA.Controllers
{
    [Authorize(Roles ="Admin")]
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

        [Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListProjects()
        {
            return View(await _context.Projekte.ToListAsync());
        }

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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListUserProjects(string userId)
        {
            var roles =  _context.ProjectRoles.ToList().Where(user => user.UserId == userId);
            var user = await userManager.FindByIdAsync(userId);
            ViewBag.Name = user.UserName;
            return View(roles);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(string Id)
        {
            if(Id != null)
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