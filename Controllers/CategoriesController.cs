using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication1.Data;
using ShopWebApplication1.Models;
using DocumentFormat.OpenXml.VariantTypes;
using System.Numerics;

namespace ShopWebApplication1.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly MyshopContext _context;

        public CategoriesController(MyshopContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
              return _context.Categories != null ? 
                          View(await _context.Categories.ToListAsync()) :
                          Problem("Entity set 'MyshopContext.Categories'  is null.");
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index", "Goods", new{id = id});
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'MyshopContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //

        

            public ActionResult ExportToExcel()
            {
                using (XLWorkbook workbook = new XLWorkbook())

                {   // Отримати дані з бази даних
                    var categories = _context.Categories.Include(c => c.Goods).ToList();

                    // Створити новий Excel 
                    var worksheet = workbook.Worksheets.Add("Категорії");

                    // Додати заголовки стовпців
                    worksheet.Cell(1, 1).Value = "КатегоріяId";
                    worksheet.Cell(1, 2).Value = "Назва категорії";
                    worksheet.Cell(1, 3).Value = "Кількість товарів";

                     // Додати дані з бази даних
                    for (int i = 0; i < categories.Count; i++)
                    {
                        var category = categories[i];
                        worksheet.Cell(i + 2, 1).Value = category.Id;
                        worksheet.Cell(i + 2, 2).Value = category.Name;
                        worksheet.Cell(i + 2, 3).Value = category.Goods.Count;
                    }

                    // Зберегти файл Excel
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                        {
                            FileDownloadName = $"categories.xlsx"
                        };
                    }
                }
            }
        public async Task<IActionResult> ImportFromExcel(IFormFile fileExcel)
        {
            if (fileExcel != null && fileExcel.Length > 0)
            {
                using (var stream = fileExcel.OpenReadStream())
                {
                    try
                    {
                        XLWorkbook workbook = new XLWorkbook(stream);
                        var worksheet = workbook.Worksheet(1);

                        var categories = new List<Category>();

                        var row = 2;
                        while (!worksheet.Cell(row, 1).IsEmpty())
                        {
                            var categoryName = worksheet.Cell(row, 1).GetValue<string>();

                            // Проверяем наличие категории с таким же названием
                            var existingCategory = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
                            if (existingCategory != null)
                            {
                                // Категория с таким названием уже существует, пропускаем добавление
                                row++;
                                continue;
                            }

                            var category = new Category { Name = categoryName };
                            categories.Add(category);

                            row++;
                        }

                        // Сохраняем категории в базе данных
                        await _context.Categories.AddRangeAsync(categories);
                        await _context.SaveChangesAsync();

                        // Возвращаем пользователю список категорий с добавленными данными
                        var allCategories = _context.Categories.ToList();
                        return View("Index", allCategories);
                    }
                    catch (Exception ex)
                    {
                        // Обработка ошибок при чтении файла
                        return RedirectToAction("Index", new { importSuccess = "При импорте произошла ошибка: " + ex.Message });
                    }
                }
            }

            // Возвращаем ошибку, если файл не выбран
            return RedirectToAction("Index", new { importSuccess = "Вы не выбрали файл для импорта." });
        }










    }

}












