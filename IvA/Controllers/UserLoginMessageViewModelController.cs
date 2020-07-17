using IvA.Data;
using IvA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Controllers
{
    //Der Controller diente für Entwicklungs- und Debuggingzwecke
    public class UserLoginMessageViewModelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserLoginMessageViewModelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserLoginMessageViewModel
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserLoginMessageViewModel.ToListAsync());
        }

        // GET: UserLoginMessageViewModel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLoginMessageViewModel = await _context.UserLoginMessageViewModel
                .FirstOrDefaultAsync(m => m.UserLoginMessageViewModelID == id);
            if (userLoginMessageViewModel == null)
            {
                return NotFound();
            }

            return View(userLoginMessageViewModel);
        }

        // GET: UserLoginMessageViewModel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserLoginMessageViewModel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserLoginMessageViewModelID,UserLoginID,MessageID")] UserLoginMessageViewModel userLoginMessageViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userLoginMessageViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userLoginMessageViewModel);
        }

        // GET: UserLoginMessageViewModel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLoginMessageViewModel = await _context.UserLoginMessageViewModel.FindAsync(id);
            if (userLoginMessageViewModel == null)
            {
                return NotFound();
            }
            return View(userLoginMessageViewModel);
        }

        // POST: UserLoginMessageViewModel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserLoginMessageViewModelID,UserLoginID,MessageID")] UserLoginMessageViewModel userLoginMessageViewModel)
        {
            if (id != userLoginMessageViewModel.UserLoginMessageViewModelID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userLoginMessageViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserLoginMessageViewModelExists(userLoginMessageViewModel.UserLoginMessageViewModelID))
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
            return View(userLoginMessageViewModel);
        }

        // GET: UserLoginMessageViewModel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLoginMessageViewModel = await _context.UserLoginMessageViewModel
                .FirstOrDefaultAsync(m => m.UserLoginMessageViewModelID == id);
            if (userLoginMessageViewModel == null)
            {
                return NotFound();
            }

            return View(userLoginMessageViewModel);
        }

        // POST: UserLoginMessageViewModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userLoginMessageViewModel = await _context.UserLoginMessageViewModel.FindAsync(id);
            _context.UserLoginMessageViewModel.Remove(userLoginMessageViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserLoginMessageViewModelExists(int id)
        {
            return _context.UserLoginMessageViewModel.Any(e => e.UserLoginMessageViewModelID == id);
        }
    }
}
