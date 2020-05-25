using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IvA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            var roles = roleManager.Roles;
            return View(users);
        }

        public IActionResult ListUserRoles()
        {
            var users = userManager.Users;
            var roles = roleManager.Roles;
            //var userRoles = users.Join(roles);
            return View(users);
        }

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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            var existRole = await roleManager.RoleExistsAsync(role.RoleName);
            if (!existRole)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role.RoleName));
            }
            return View();
        }
    }
}