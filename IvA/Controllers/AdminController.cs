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
            return View(users);
        }

        public async Task Delete()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                var userId = await userManager.GetUserIdAsync(user);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
                }
            }
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