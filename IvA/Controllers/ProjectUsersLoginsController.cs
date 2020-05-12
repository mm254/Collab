using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IvA.Data;
using IvA.Models;

namespace IvA.Controllers
{
    public class ProjectUsersLoginsController : Controller
    {
        private readonly ProjectUserLoginContext _context;

        public ProjectUsersLoginsController(ProjectUserLoginContext context)
        {
            _context = context;
        }

        // GET: ProjectUsersLogins
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProjectUserLoginContexts.ToListAsync());
        }

        // GET: ProjectUsersLogins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUsersLogin = await _context.ProjectUserLoginContexts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectUsersLogin == null)
            {
                return NotFound();
            }

            return View(projectUsersLogin);
        }

        // GET: ProjectUsersLogins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProjectUsersLogins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,References,EnterpriseName,Mail,UserPassword")] ProjectUsersLogin projectUsersLogin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectUsersLogin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(projectUsersLogin);
        }

        // GET: ProjectUsersLogins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUsersLogin = await _context.ProjectUserLoginContexts.FindAsync(id);
            if (projectUsersLogin == null)
            {
                return NotFound();
            }
            return View(projectUsersLogin);
        }

        // POST: ProjectUsersLogins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,References,EnterpriseName,Mail,UserPassword")] ProjectUsersLogin projectUsersLogin)
        {
            if (id != projectUsersLogin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectUsersLogin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectUsersLoginExists(projectUsersLogin.Id))
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
            return View(projectUsersLogin);
        }

        // GET: ProjectUsersLogins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUsersLogin = await _context.ProjectUserLoginContexts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectUsersLogin == null)
            {
                return NotFound();
            }

            return View(projectUsersLogin);
        }

        // POST: ProjectUsersLogins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectUsersLogin = await _context.ProjectUserLoginContexts.FindAsync(id);
            _context.ProjectUserLoginContexts.Remove(projectUsersLogin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectUsersLoginExists(int id)
        {
            return _context.ProjectUserLoginContexts.Any(e => e.Id == id);
        }
    }
}
