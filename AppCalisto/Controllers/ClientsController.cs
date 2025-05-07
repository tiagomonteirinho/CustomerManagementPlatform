using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Back-office")]
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _clientRepository.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View(new ClientViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create client.";
                return View();
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                var existingClientByEmail = await _clientRepository.GetByEmailAsync(model.Email);
                if (existingClientByEmail != null)
                {
                    TempData["Failure"] = "That email is already being used.";
                    return View(model);
                }
            }

            if (!string.IsNullOrEmpty(model.Tax))
            {
                var existingClientByTax = await _clientRepository.GetByTaxAsync(model.Tax);
                if (existingClientByTax != null)
                {
                    TempData["Failure"] = "That tax ID is already being used.";
                    return View(model);
                }
            }

            var client = new Client()
            {
                Name = model.Name,
                ContactPerson = model.ContactPerson,
                Email = model.Email,
                Phone = model.Phone,
                Tax = model.Tax
            };

            await _clientRepository.CreateAsync(client);
            var createdClient = await _clientRepository.GetByIdAsync(client.Id);
            if (client == null)
            {
                TempData["Failure"] = "Could not create client.";
                return View(model);
            }

            TempData["Success"] = "Client created successfully!";
            return RedirectToAction("Create");
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
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            return View(new ClientViewModel
            {
                Id = client.Id,
                Name = client.Name,
                ContactPerson = client.ContactPerson,
                Email = client.Email,
                Phone = client.Phone,
                Tax = client.Tax
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update client.";
                return View(model);
            }

            var client = await _clientRepository.GetByIdAsync(model.Id);
            if (client == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            if (client.Name == model.Name && client.ContactPerson == model.ContactPerson && client.Email == model.Email && client.Phone == model.Phone && client.Tax == model.Tax)
            {
                TempData["Failure"] = "No changes were found.";
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Email) && model.Email != client.Email)
            {
                var existingClientByEmail = await _clientRepository.GetByEmailAsync(model.Email);
                if (existingClientByEmail != null)
                {
                    TempData["Failure"] = "That email is already being used.";
                    return View(model);
                }
            }

            if (!string.IsNullOrEmpty(model.Tax) && model.Tax != client.Tax)
            {
                var existingClientByTax = await _clientRepository.GetByTaxAsync(model.Tax);
                if (existingClientByTax != null)
                {
                    TempData["Failure"] = "That tax ID is already being used.";
                    return View(model);
                }
            }

            client.Name = model.Name;
            client.ContactPerson = model.ContactPerson;
            client.Email = model.Email;
            client.Phone = model.Phone;
            client.Tax = model.Tax;

            if (!await _clientRepository.UpdateAsync(client))
            {
                TempData["Failure"] = "Could not update client. This may be due to the entity being used by other entities or no longer existing.";
                return View(model);
            }

            TempData["Success"] = "Client updated successfully.";
            return RedirectToAction("Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            var existingClients = await _clientRepository.GetAllAsync();
            if (existingClients.Count == 1)
            {
                TempData["Failure"] = "There must be at least one client in the database.";
                return RedirectToAction("Edit", new { id });
            }

            if (!await _clientRepository.DeleteAsync(client))
            {
                TempData["Failure"] = "Could not delete client. This may be due to the entity being used by other entities or no longer existing.";
                return RedirectToAction("Edit", new { id });
            }

            TempData["Success"] = "Client deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
