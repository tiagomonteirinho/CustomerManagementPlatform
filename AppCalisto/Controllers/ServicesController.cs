using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ICompanyRepository _companyRepository;

        public ServicesController(IServiceRepository serviceRepository, ICompanyRepository companyRepository)
        {
            _serviceRepository = serviceRepository;
            _companyRepository = companyRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _serviceRepository.GetAllAsync());
        }

        public async Task<IActionResult> Create(int? companyId)
        {
            if (companyId == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Company" });
            }

            var company = await _companyRepository.GetByIdAsync(companyId.Value);
            if (company == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Company" });
            }

            return View(new ServiceViewModel
            {
                Company = company,
                CompanyId = companyId.Value
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create service.";
                return View(model);
            }

            var existingServiceByAbbreviation = await _serviceRepository.GetByAbbreviationAsync(model.Abbreviation);
            if (existingServiceByAbbreviation != null)
            {
                TempData["Failure"] = "That abbreviation is already being used.";
                return View(model);
            }

            var service = new Service()
            {
                Abbreviation = model.Abbreviation,
                Name = model.Name,
                Company = model.Company,
                CompanyId = model.CompanyId
            };

            await _serviceRepository.CreateAsync(service);
            var createdService = await _serviceRepository.GetByIdAsync(service.Id);
            if (service == null)
            {
                TempData["Failure"] = "Could not create service.";
                return View(model);
            }

            TempData["Success"] = "Service created successfully!";
            return RedirectToAction("Create", new { companyId = model.CompanyId });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Service" });
            }

            var service = await _serviceRepository.GetByIdAsync(id.Value);
            if (service == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Service" });
            }

            return View(service);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Service" });
            }

            var service = await _serviceRepository.GetByIdAsync(id.Value);
            if (service == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Service" });
            }

            return View(new ServiceViewModel
            {
                Id = service.Id,
                Abbreviation = service.Abbreviation,
                Name = service.Name,
                CompanyId = service.CompanyId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update service.";
                return View(model);
            }

            var service = await _serviceRepository.GetByIdAsync(model.Id);
            if (service == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Service" });
            }

            if (service.Abbreviation == model.Abbreviation && service.Name == model.Name)
            {
                TempData["Failure"] = "No changes were found.";
                return View(model);
            }

            if (model.Abbreviation != service.Abbreviation)
            {
                var existingServiceByAbbreviation = await _serviceRepository.GetByAbbreviationAsync(model.Abbreviation);
                if (existingServiceByAbbreviation != null)
                {
                    TempData["Failure"] = "That abbreviation is already being used.";
                    return View(model);
                }
            }

            service.Abbreviation = model.Abbreviation;
            service.Name = model.Name;

            if (!await _serviceRepository.UpdateAsync(service))
            {
                TempData["Failure"] = "Could not update service. This may be due to the entity being used by other entities or no longer existing.";
                return View(model);
            }

            TempData["Success"] = "Service updated successfully.";
            return RedirectToAction("Edit", new { id = service.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Service" });
            }

            var existingServices = await _serviceRepository.GetAllAsync();
            if (existingServices.Count == 1)
            {
                TempData["Failure"] = "There must be at least one service in the database.";
                return RedirectToAction("Edit", new { id });
            }

            if (!await _serviceRepository.DeleteAsync(service))
            {
                TempData["Failure"] = "Could not delete service. This may be due to the entity being used by other entities or no longer existing.";
                return RedirectToAction("Edit", new { id });
            }

            TempData["Success"] = "Service deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
