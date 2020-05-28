using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IvA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IvA.Controllers
{
    //[Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, 
                                UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        /**
        public IActionResult ListUserRoles()
        {
            var users = userManager.Users.ToListAsync();
            var roles = roleManager.Roles.ToListAsync();
            
            var userRoles = from _projekte in Projekte
                                    join _projektPakete in ProjektPakete
                                    on _projekte.Id equals _projektPakete.Id into table1
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
            return View(userRoles);
        } **/

        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? Id)
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
        public async Task<IActionResult> Role(string? Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if(user != null)
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
            return RedirectToAction("ListUsers");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleModel role)
        {
            var existRole = await roleManager.RoleExistsAsync(role.Name);
            if (!existRole)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role.Name));
            }
            return View();
        }
    }
}