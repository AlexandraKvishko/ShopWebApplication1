using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using ShopWebApplication1.Models;
using ShopWebApplication1.Data;

namespace ShopWebApplication1.Controllers
{
    [ApiController]
    [Route("api/Chart")]
    public class ChartController : ControllerBase
    {
        private readonly MyshopContext _context;

        public ChartController(MyshopContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var goods = _context.Goods.Include(g => g.Cat).ToList();
            List<object> data = new List<object>();
            data.Add(new[] { "Name", "Quantity" });

            foreach (var good in goods)
            {
                data.Add(new object[] { good.Name, good.Quantity });
            }

            return new JsonResult(data);
        }

        [HttpGet("JsonData1")]
        public JsonResult JsonData1()
        {
            var categories = _context.Categories.Include(c => c.Goods).ToList();
            List<object> data = new List<object>();
            data.Add(new[] { "Category", "Total Quantity" });

            foreach (var category in categories)
            {
                int totalQuantity = category.Goods.Sum(g => g.Quantity);
                data.Add(new object[] { category.Name, totalQuantity });
            }

            return new JsonResult(data);
        }

    }
}
