using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication1.Data;
using ShopWebApplication1.Models;

namespace ShopWebApplication1.Controllers
{
    public class GoodsController : Controller
    {
        private readonly MyshopContext _context;

        public GoodsController(MyshopContext context)
        {
            _context = context;
        }

        // GET: Goods
        public async Task<IActionResult> Index(int? id)
        {
           if (id == null) return RedirectToAction("Index", "Categories");
           var name = _context.Categories.Where(c => c.Id == id).FirstOrDefault().Name;
           ViewBag.CatId = id;
           ViewBag.Cat = name;
           var goodsByCategory = _context.Goods.Where(b => b.CatId == id).Include(b => b.Cat);

           return View(await goodsByCategory.ToListAsync());
        }

        // GET: Goods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Goods == null)
            {
                return NotFound();
            }

            var good = await _context.Goods
                .Include(g => g.Cat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (good == null)
            {
                return NotFound();
            }

            return View(good);
        }

        // GET: Goods/Create
        public IActionResult Create( int categoryId)
        {
            ViewBag.CatId = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Goods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( int categoryId,  [Bind("Id,CatId,Barcode,Price,Quantity,Descrip,Name")] Good good)
        {
            ModelState["Cat"].ValidationState = ModelValidationState.Valid;

            if (ModelState.IsValid)
            {
                _context.Add(good);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new {id = good.CatId });
            }
            ViewBag.CatId = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // GET: Goods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Goods == null)
            {
                return NotFound();
            }

            var good = await _context.Goods.FindAsync(id);
            if (good == null)
            {
                return NotFound();
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", good.CatId);
            return View(good);
        }

        // POST: Goods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CatId,Barcode,Price,Quantity,Descrip,Name")] Good good)
        {
            if (id != good.Id) return NotFound();

            ModelState["Cat"].ValidationState = ModelValidationState.Valid;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(good);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = good.CatId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoodExists(good.Id)) return NotFound();
                    else throw;
                }
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", good.CatId);
            return View(good);
        }

        // GET: Goods/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Goods == null)
            {
                return NotFound();
            }

            var good = await _context.Goods
                .Include(g => g.Cat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (good == null)
            {
                return NotFound();
            }

            return View(good);
        }

        // POST: Goods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Goods == null)
            {
                return Problem("Entity set 'MyshopContext.Goods'  is null.");
            }
            var good = await _context.Goods.FindAsync(id);
            if (good != null)
            {
                _context.Goods.Remove(good);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = good.CatId });
            }
            return RedirectToAction("Delete", new { id = good.CatId });
        }

        private bool GoodExists(int id)
        {
          return (_context.Goods?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> AddToBaskett(int id)
        {
            if (_context.Goods == null) { return Problem("Entity set 'MyshopContext.Goods'  is null."); }
            var uid = new UserCheck().GetCurUserId();
            if (uid>0)
            {
                var user = await _context.Users.FindAsync(uid);
                var good = await _context.Goods.FindAsync(id);
                if (user != null)
                {
                    if (good != null)
                    {
                        var baskett = _context.Basketts.Where(p => p.UserId == user.Id).Where(x => x.GoodsId == good.Id).FirstOrDefault();
                        if (baskett != null)
                        {
                            baskett.Quantity += 1;
                            _context.Update(baskett);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            baskett = new Baskett { GoodsId = good.Id, UserId = user.Id, Quantity = 1 };
                            _context.Basketts.Add(baskett);
                            await _context.SaveChangesAsync();
                        }
                        return RedirectToAction("Index", "Basketts");
                    }
                }
                return RedirectToAction("Index", new { id = good.CatId });
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
