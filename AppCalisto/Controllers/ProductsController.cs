using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Back-office")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _productRepository.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create product.";
                return View();
            }

            var product = new Product()
            {
                Name = model.Name,
                BasePrice = model.BasePrice,
                TaxRate = model.TaxRate
            };

            await _productRepository.CreateAsync(product);
            var createdProduct = await _productRepository.GetByIdAsync(product.Id);
            if (createdProduct == null)
            {
                TempData["Failure"] = "Could not create product.";
                return View(model);
            }

            TempData["Success"] = "Product created successfully!";
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            return View(new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                BasePrice = product.BasePrice,
                TaxRate = product.TaxRate
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update product.";
                return View(model);
            }

            var product = await _productRepository.GetByIdAsync(model.Id);
            if (product == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            if (product.Name == model.Name && product.BasePrice == model.BasePrice && product.TaxRate == model.TaxRate)
            {
                TempData["Failure"] = "No changes were found.";
                return View(model);
            }

            product.Name = model.Name;
            product.BasePrice = model.BasePrice;
            product.TaxRate = model.TaxRate;

            if (!await _productRepository.UpdateAsync(product))
            {
                TempData["Failure"] = "Could not update product. This may be due to the entity being used by other entities or no longer existing.";
                return View(model);
            }

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            var existingProducts = await _productRepository.GetAllAsync();
            if (existingProducts.Count == 1)
            {
                TempData["Failure"] = "There must be at least one product in the database.";
                return RedirectToAction("Edit", new { id });
            }

            if (!await _productRepository.DeleteAsync(product))
            {
                TempData["Failure"] = "Could not delete product. This may be due to the entity being used by other entities or no longer existing.";
                return RedirectToAction("Edit", new { id });
            }

            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
