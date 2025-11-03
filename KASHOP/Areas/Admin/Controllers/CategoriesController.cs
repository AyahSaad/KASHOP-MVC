using KASHOP.Data;
using KASHOP.Models;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {

         ApplicationDbContext context= new ApplicationDbContext();
        public IActionResult Index()
        {
            var cats = context.Categories.ToList();
            return View(cats);
        }

        public IActionResult Create()
        {
            return View(new Category());
        }

        [ValidateAntiForgeryToken]
        public IActionResult Store(Category request)
        {
            if (!ModelState.IsValid)
            {
                return View("Create",request);

            }
            context.Categories.Add(request);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        // GET:
        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("Edit", category);
        }

        // POST:
        [HttpPost]
        public IActionResult Update(Category request)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = context.Categories.Find(request.Id);
                if (existingCategory == null)
                {
                    return NotFound();
                }
                existingCategory.Name = request.Name;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Edit", request);
        }
        public IActionResult Remove(int id)
        {
            var cats = context.Categories.Find(id);
            context.Categories.Remove(cats);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
