using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication1.Data;
using ShopWebApplication1.Models;

namespace ShopWebApplication1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MyshopContext _context;

        public OrdersController(MyshopContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var uid = new UserCheck().GetCurUserId();
            if (uid == 0) return NotFound();
            var user = await _context.Users.FindAsync(uid);
            if(user == null) return NotFound();
            ViewBag.User = user;
            var OrderList = await _context.Orders.Include(s => s.State).Include(u => u.User).Include(og => og.OrderGoods).Where(i => i.UserId == user.Id).ToListAsync();
            var OrderGoodsList = await _context.OrderGoods.Include(og => og.Good).ToListAsync();
            ViewBag.OrderList = OrderList;
            ViewBag.OrderGoodsList = OrderGoodsList;
            return View(OrderList);
        }

        // GET: Orders/Details
        public async Task<IActionResult> Details(int? id)
        {
            var uid = new UserCheck().GetCurUserId();
            if (uid == 0) return NotFound();
            var user = await _context.Users.FindAsync(uid);
            if (user == null) return NotFound();
            ViewBag.User = user;
            var order = await _context.Orders.Include(s => s.State).Include(u => u.User).Include(og => og.OrderGoods).Where(i => i.UserId == user.Id).FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.OrderGoods = await _context.OrderGoods.Include(g => g.Good).Where(i => i.OrderId == order.Id).ToListAsync();
            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Orders/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            var uid = new UserCheck().GetCurUserId();
            if (uid == 0) return NotFound();
            var user = await _context.Users.FindAsync(uid);
            if (user == null) return NotFound();
            ViewBag.User = user;
            var order = await _context.Orders.Include(s => s.State).Include(u => u.User).Include(og => og.OrderGoods).Where(i => i.UserId == user.Id).FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.OrderGoods = await _context.OrderGoods.Include(g => g.Good).Where(i => i.OrderId == order.Id).ToListAsync();
            ViewBag.StateId = new SelectList(_context.OrderStatuses, "Id", "Name", order.StateId);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Data,StateId")] Order order_in)
        {
            var uid = new UserCheck().GetCurUserId();
            if (uid == 0) return NotFound();
            var user = await _context.Users.FindAsync(uid);
            if (user == null) return NotFound();
            ViewBag.User = user;
            var order = await _context.Orders.Include(s => s.State).Include(u => u.User).Include(og => og.OrderGoods).Where(i => i.UserId == user.Id).FirstOrDefaultAsync(m => m.Id == id);
            if (order == null) return NotFound();

            if (id != order_in.Id) return NotFound();
            if (ModelState["StateId"].ValidationState == ModelValidationState.Valid)
            {
                order.StateId = order_in.StateId;
                _context.Update(order);
                await _context.SaveChangesAsync();
            }

            ViewBag.OrderGoods = await _context.OrderGoods.Include(g => g.Good).Where(i => i.OrderId == order.Id).ToListAsync();
            ViewBag.StateId = new SelectList(_context.OrderStatuses, "Id", "Name", order.StateId);
            return View(order);
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> MakeNewOrder()
        {
            var uid = new UserCheck().GetCurUserId();
            if (uid > 0)
            {
                var user = await _context.Users.FindAsync(uid);
                if (user != null)
                {
                    var BaskettList = await _context.Basketts.Where(d => d.UserId == uid).ToListAsync();
                    if (BaskettList != null)
                    {
                        var order = new Order { UserId = user.Id, Data = DateTime.Now, StateId = 1 };
                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();
                        foreach (var item in BaskettList)
                        {
                            _context.OrderGoods.Add(new OrderGood { GoodId = item.GoodsId, Quatity = item.Quantity, OrderId = order.Id });
                            await _context.SaveChangesAsync();
                        }
                        foreach (var item in BaskettList)
                        {
                            _context.Basketts.Remove(item);
                            await _context.SaveChangesAsync();
                        }
                        return RedirectToAction("Details", "Orders", new { id = order.Id });
                    }
                    return Problem("Кошик порожній.");
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
