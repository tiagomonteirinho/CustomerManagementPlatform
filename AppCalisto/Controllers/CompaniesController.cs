using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Data.Repositories;
using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CompaniesController : Controller
    {
        private readonly ICompanyRepository _companyRepository;

        public CompaniesController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _companyRepository.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View(new CompanyViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create company.";
                return View();
            }

            var existingCompanyByAbbreviation = await _companyRepository.GetByAbbreviationAsync(model.Abbreviation);
            if (existingCompanyByAbbreviation != null)
            {
                TempData["Failure"] = "That abbreviation is already being used.";
                return View(model);
            }

            var company = new Company()
            {
                Abbreviation = model.Abbreviation,
                Name = model.Name
            };

            await _companyRepository.CreateAsync(company);
            var createdCompany = await _companyRepository.GetByIdAsync(company.Id);
            if (company == null)
            {
                TempData["Failure"] = "Could not create company.";
                return View(model);
            }

            TempData["Success"] = "Company created successfully!";
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Company" });

            var company = await _companyRepository.GetByIdAsync(id.Value);
            if (company == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Company" });

            return View(company);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Company" });

            var company = await _companyRepository.GetByIdAsync(id.Value);
            if (company == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Company" });

            return View(new CompanyViewModel
            {
                Id = company.Id,
                Abbreviation = company.Abbreviation,
                Name = company.Name
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update company.";
                return View(model);
            }

            var company = await _companyRepository.GetByIdAsync(model.Id);
            if (company == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Company" });

            if (company.Abbreviation == model.Abbreviation && company.Name == model.Name)
            {
                TempData["Failure"] = "No changes were found.";
                return View(model);
            }

            if (model.Abbreviation != company.Abbreviation)
            {
                var existingCompanyByAbbreviation = await _companyRepository.GetByAbbreviationAsync(model.Abbreviation);
                if (existingCompanyByAbbreviation != null)
                {
                    TempData["Failure"] = "That abbreviation is already being used.";
                    return View(model);
                }
            }

            company.Abbreviation = model.Abbreviation;
            company.Name = model.Name;

            if (!await _companyRepository.UpdateAsync(company))
            {
                TempData["Failure"] = "Could not update company. This may be due to the entity being used by other entities or no longer existing.";
                return View(model);
            }

            TempData["Success"] = "Company updated successfully.";
            return RedirectToAction("Edit");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Company" });

            var existingCompanies = await _companyRepository.GetAllAsync();
            if (existingCompanies.Count == 1)
            {
                TempData["Failure"] = "There must be at least one company in the database.";
                return RedirectToAction("Edit", new { id });
            }

            if (!await _companyRepository.DeleteAsync(company))
            {
                TempData["Failure"] = "Could not delete company. This may be due to the entity being used by other entities or no longer existing.";
                return RedirectToAction("Edit", new { id });
            }

            TempData["Success"] = "Company deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
