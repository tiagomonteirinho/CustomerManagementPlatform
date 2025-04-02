using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Helpers;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Back-office")]
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICompanyHelper _companyHelper;

        public ClientsController(IClientRepository clientRepository, ICompanyHelper companyHelper)
        {
            _clientRepository = clientRepository;
            _companyHelper = companyHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _clientRepository.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View(new ClientViewModel 
            {
                SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClientViewModel model)
        {
            model.SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>();

            if (!ModelState.IsValid)
            {
                ViewBag.Failure = "Could not create client.";
                return View();
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                var existingClientByEmail = await _clientRepository.GetByEmailAsync(model.Email);
                if (existingClientByEmail != null)
                {
                    ViewBag.Failure = "That email is already being used.";
                    return View(model);
                }
            }

            if (!string.IsNullOrEmpty(model.Tax))
            {
                var existingClientByTax = await _clientRepository.GetByTaxAsync(model.Tax);
                if (existingClientByTax != null)
                {
                    ViewBag.Failure = "That tax ID is already being used.";
                    return View(model);
                }
            }

            if (model.Companies == null || !model.Companies.Any())
            {
                ViewBag.Failure = "At least one company must be selected!";
                return View(model);
            }

            var client = new Client()
            {
                Name = model.Name,
                ContactPerson = model.ContactPerson,
                Email = model.Email,
                Phone = model.Phone,
                Tax = model.Tax,
                Companies = string.Join(", ", model.Companies ?? new List<string>())
            };

            await _clientRepository.CreateAsync(client);
            if (!await _clientRepository.ExistsAsync(client.Id))
            {
                ViewBag.Failure = "Could not create client.";
                return View(model);
            }

            ViewBag.Success = "Client created successfully!";
            return View(new ClientViewModel
            {
                SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>()
            });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            return View(client);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            var model = new ClientViewModel
            {
                Id = client.Id,
                Name = client.Name,
                ContactPerson = client.ContactPerson,
                Email = client.Email,
                Phone = client.Phone,
                Tax = client.Tax,
                Companies = string.IsNullOrEmpty(client.Companies) ? new List<string>() : client.Companies.Split(", ").ToList(),
                SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClientViewModel model)
        {
            model.SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>();

            if (!ModelState.IsValid)
            {
                ViewBag.Failure = "Could not update client.";
                return View(model);
            }

            var client = await _clientRepository.GetByIdAsync(model.Id);
            if (client == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            if (client.Name == model.Name && client.ContactPerson == model.ContactPerson && client.Email == model.Email && client.Phone == model.Phone && client.Tax == model.Tax
                && client.Companies == string.Join(", ", model.Companies))
            {
                ViewBag.Failure = "No changes were found.";
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Email) && model.Email != client.Email)
            {
                var existingClientByEmail = await _clientRepository.GetByEmailAsync(model.Email);
                if (existingClientByEmail != null)
                {
                    ViewBag.Failure = "That email is already being used.";
                    return View(model);
                }
            }

            if (!string.IsNullOrEmpty(model.Tax) && model.Tax != client.Tax)
            {
                var existingClientByTax = await _clientRepository.GetByTaxAsync(model.Tax);
                if (existingClientByTax != null)
                {
                    ViewBag.Failure = "That tax ID is already being used.";
                    return View(model);
                }
            }

            if (model.Companies == null || !model.Companies.Any())
            {
                ViewBag.Failure = "At least one company must be selected!";
                return View(model);
            }

            client.Name = model.Name;
            client.ContactPerson = model.ContactPerson;
            client.Email = model.Email;
            client.Phone = model.Phone;
            client.Tax = model.Tax;
            client.Companies = model.Companies != null ? string.Join(", ", model.Companies) : "";

            if (!await _clientRepository.UpdateAsync(client))
            {
                ViewBag.Failure = "Could not update client. This may be due to database constraints or the entity no longer existing.";
                return View(model);
            }

            ViewBag.Success = "Client updated successfully.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            if (!await _clientRepository.DeleteAsync(client))
            {
                ViewBag.Failure = "Could not delete client. This may be due to database constraints or the entity no longer existing.";
                return View("Edit", client);
            }

            ViewBag.Success = "Client updated successfully.";
            return RedirectToAction($"Index");
        }
    }
}
