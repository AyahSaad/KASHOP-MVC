using KASHOP.Data;
using KASHOP.Models;
using KASHOP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KASHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public IActionResult Index()
        {
            //var products = context.Products.ToList();
            var products = context.Products.Include(p=>p.category).ToList();
            var productsVm = new List<ProductsViewModel>();

            foreach(var item in products)
            {
                var vm = new ProductsViewModel
                {
                    Id= item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageUrl=$"{Request.Scheme}://{Request.Host}/images/{item.Image}",
                    CategoryName= item.category.Name

                };
                productsVm.Add(vm);

            }
            return View(productsVm);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = context.Categories.ToList();
            return View(new Product());
        }

        [ValidateAntiForgeryToken]
        public IActionResult Store(Product request,IFormFile file)
        {
            ViewBag.Categories = context.Categories.ToList();
            ModelState.Remove("File");
            if (!ModelState.IsValid)
            {
                return View("Create", request);

            }

            if (file==null || file.Length==0)
            {
                ModelState.AddModelError("Image", "Please Upload an image");
                return View("Create", request);
            }

            var allowedExtensions = new[] { ".jpg", ".webp" };
            var extension= Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("Image", "Only jpg, webp files are allowed");
                return View("Create", request);
            }

            if (file.Length> 2*1024*1024)
            {
                ModelState.AddModelError("Image", "Image size must be less than 2MB");
                return View("Create", request);
            }

            var fileName = Guid.NewGuid().ToString();
                fileName+= Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),@"wwwroot\images",fileName);
            
            using (var stream= System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

                request.Image=fileName;
                context.Products.Add(request);
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            var product = context.Products.Find(id);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", product.Image);
            System.IO.File.Delete(filePath);    
            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET:
        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = context.Products.Find(id);
            ViewBag.Categories = context.Categories.ToList();
            if (product == null)
            {
                return NotFound();
            }
            return View("Edit", product);
        }

        // POST:
        [HttpPost]
        public IActionResult Update(Product request, IFormFile? file)
        {
            var product = context.Products.Find(request.Id);
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Quantity = request.Quantity;
            product.CategoryId = request.CategoryId;

            if (file != null && file.Length > 0)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", product.Image);
                System.IO.File.Delete(oldFilePath);

                var fileName = Guid.NewGuid().ToString(); 
                fileName += Path.GetExtension(file.FileName); 
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                product.Image = fileName;
            }

            context.SaveChanges();
            return RedirectToAction("Index");
        }

    }

}
