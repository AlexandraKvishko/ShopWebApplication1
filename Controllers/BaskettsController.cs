using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication1.Data;
using ShopWebApplication1.Models;

namespace ShopWebApplication1.Controllers
{
    public class BaskettsController : Controller
    {
        private readonly MyshopContext _context;

        public BaskettsController(MyshopContext context)
        {
            _context = context;
        }

        // GET: Basketts
        public async Task<IActionResult> Index()
        {
            var uid = new UserCheck().GetCurUserId();
            if (uid == 0) return RedirectToAction("Index", "Categories");
            var user = await _context.Users.FindAsync(uid);
            ViewBag.User = user;
            var BaskettList = await _context.Basketts.Include(b => b.Goods).Include(b => b.User).Where(d => d.UserId == uid).ToListAsync();
            return View(BaskettList);
        }

        // GET: Basketts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Basketts == null)
            {
                return NotFound();
            }

            var baskett = await _context.Basketts
                .Include(b => b.Goods)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baskett == null)
            {
                return NotFound();
            }

            return View(baskett);
        }

        // GET: Basketts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Basketts == null) return NotFound();
            var baskett = await _context.Basketts
                .Include(b => b.Goods)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baskett == null) return NotFound();
            return View(baskett);
        }

        // POST: Basketts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GoodsId,UserId,Quantity")] Baskett baskett_in)
        {
            if (id != baskett_in.Id) return NotFound();
            var baskett = await _context.Basketts
                .Include(b => b.Goods)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baskett == null) return NotFound();
            if (ModelState["Quantity"].ValidationState == ModelValidationState.Valid)
            {
                baskett.Quantity = baskett_in.Quantity; 
                _context.Update(baskett);
                await _context.SaveChangesAsync();
            }
            return View(baskett);
        }

        // GET: Basketts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Basketts == null)
            {
                return NotFound();
            }

            var baskett = await _context.Basketts
                .Include(b => b.Goods)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baskett == null)
            {
                return NotFound();
            }
            ViewBag.GoodName = baskett.Goods.Name;
            return View(baskett);
        }

        // POST: Basketts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Basketts == null)
            {
                return Problem("Entity set 'MyshopContext.Basketts'  is null.");
            }
            var baskett = await _context.Basketts.FindAsync(id);
            if (baskett != null)
            {
                _context.Basketts.Remove(baskett);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaskettExists(int id)
        {
            return (_context.Basketts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
