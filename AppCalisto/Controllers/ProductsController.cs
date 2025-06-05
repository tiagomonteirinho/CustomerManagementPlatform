using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Back-office")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IServiceRepository _serviceRepository;

        public ProductsController(IProductRepository productRepository, ICompanyRepository companyRepository, IServiceRepository serviceRepository)
        {
            _productRepository = productRepository;
            _companyRepository = companyRepository;
            _serviceRepository = serviceRepository;
        }
        public IActionResult GetCompanyServices(int companyId)
        {
            var services = _companyRepository.GetServices(companyId);
            return Json(services);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _productRepository.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View(new ProductViewModel
            {
                SelectableCompanies = _companyRepository.GetAll() ?? new List<SelectListItem>(),
                SelectableServices = _companyRepository.GetServices(0) ?? new List<SelectListItem>()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create product.";
                return View(new ProductViewModel
                {
                    SelectableCompanies = _companyRepository.GetAll() ?? new List<SelectListItem>(),
                    SelectableServices = _companyRepository.GetServices(0) ?? new List<SelectListItem>()
                });
            }

            var product = new Product()
            {
                ServiceId = model.ServiceId,
                Name = model.Name,
                BasePrice = model.BasePrice,
                TaxRate = model.TaxRate
            };

            await _productRepository.CreateAsync(product);
            var createdProduct = await _productRepository.GetByIdAsync(product.Id);
            if (createdProduct == null)
            {
                TempData["Failure"] = "Could not create product.";
                return View(new ProductViewModel
                {
                    SelectableCompanies = _companyRepository.GetAll() ?? new List<SelectListItem>(),
                    SelectableServices = _companyRepository.GetServices(0) ?? new List<SelectListItem>()
                });
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

        private ProductViewModel EditProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                CompanyId = product.Service.CompanyId,
                SelectableCompanies = _companyRepository.GetAll() ?? new List<SelectListItem>(),
                ServiceId = product.ServiceId,
                SelectableServices = _companyRepository.GetServices(product.Service.CompanyId) ?? new List<SelectListItem>(),
                Name = product.Name,
                BasePrice = product.BasePrice,
                TaxRate = product.TaxRate
            };
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            return View(EditProductViewModel(product));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.Id);
            if (product == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update product.";
                return View(EditProductViewModel(product));
            }

            if (product.ServiceId == model.ServiceId && product.Name == model.Name && product.BasePrice == model.BasePrice && product.TaxRate == model.TaxRate)
            {
                TempData["Failure"] = "No changes were found.";
                return View(EditProductViewModel(product));
            }

            product.ServiceId = model.ServiceId;
            product.Name = model.Name;
            product.BasePrice = model.BasePrice;
            product.TaxRate = model.TaxRate;

            if (!await _productRepository.UpdateAsync(product))
            {
                TempData["Failure"] = "Could not update product. This may be due to the entity being used by other entities or no longer existing.";
                return View(EditProductViewModel(product));
            }

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("Edit");
        }

        [HttpPost, ValidateAntiForgeryToken]
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
