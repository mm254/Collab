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
    public class ArbeitsPaketController : Controller
    {
        private readonly ArbeitsPaketContext _context;

        public ArbeitsPaketController(ArbeitsPaketContext context)
        {
            _context = context;
        }

        // GET: ArbeitsPaket
        public async Task<IActionResult> Index()
        {
            return View(await _context.ArbeitsPaket.ToListAsync());
        }

        // GET: ArbeitsPaket/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: ArbeitsPaket/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ArbeitsPaket/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArbeitsPaketId,PaketName,Beschreibung,Mitglieder,Frist,Status")] ArbeitsPaket arbeitsPaket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(arbeitsPaket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(arbeitsPaket);
        }

        // GET: ArbeitsPaket/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: ArbeitsPaket/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArbeitsPaketId,PaketName,Beschreibung,Mitglieder,Frist,Status")] ArbeitsPaket arbeitsPaket)
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
                return RedirectToAction(nameof(Index));
            }
            return View(arbeitsPaket);
        }

        // GET: ArbeitsPaket/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: ArbeitsPaket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var arbeitsPaket = await _context.ArbeitsPaket.FindAsync(id);
            _context.ArbeitsPaket.Remove(arbeitsPaket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArbeitsPaketExists(int id)
        {
            return _context.ArbeitsPaket.Any(e => e.ArbeitsPaketId == id);
        }
    }
}
