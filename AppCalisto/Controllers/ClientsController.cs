using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "PT Informática, Global Eletrik, Eficaz")]
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _clientRepository.GetAllAsync();
            return View(model);
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

        public IActionResult Create()
        {
            return View(new ClientViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not create client.";
                return View();
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
            if (!await _clientRepository.ExistsAsync(client.Id))
            {
                ViewBag.ErrorMessage = "Could not create client.";
                return View(model);
            }

            ViewBag.SuccessMessage = "Client created successfully!";
            return View();
        }
    }
}
