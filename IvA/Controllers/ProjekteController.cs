﻿using System;
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

        public ProjekteController(ApplicationDbContext context)
        {
            _context = context;
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

            var projekte = await _context.Projekte
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projekte == null)
            {
                return NotFound();
            }

            return View(projekte);
        }

        // GET: Projekte/Create
        public IActionResult Create()
        {
            return View();
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
                projekte.Projektersteller = this.User.Identity.Name;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(projekte);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Projektname,Projektersteller,ErstelltAm,Mitglieder,Beschreibung,Deadline,Status")]  IvA.Models.ProjekteModel projekte)
        {
          
            if (id != projekte.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projekte);                   
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjekteExists(projekte.Id))
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
                .FirstOrDefaultAsync(m => m.Id == id);
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
            _context.Projekte.Remove(projekte);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjekteExists(int id)
        {
            return _context.Projekte.Any(e => e.Id == id);
        }
    }
}
