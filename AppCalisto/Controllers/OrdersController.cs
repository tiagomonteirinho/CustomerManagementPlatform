using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Back-office")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;

        public OrdersController(IOrderRepository orderRepository, IClientRepository clientRepository)
        {
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _orderRepository.GetAllAsync());
        }

        public async Task<IActionResult> Create(int? clientId)
        {
            if (clientId == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            var client = await _clientRepository.GetByIdAsync(clientId.Value);
            if (client == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });
            }

            return View(new OrderViewModel
            {
                ClientId = clientId.Value,
                SelectableCompanies = _clientRepository.GetCompanies(clientId).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            model.SelectableCompanies = _clientRepository.GetCompanies(model.ClientId).ToList();

            if (!ModelState.IsValid)
            {
                ViewBag.Failure = "Could not create order.";
                return View();
            }

            if (!string.IsNullOrEmpty(model.Number))
            {
                var existingOrderByNumber = await _orderRepository.GetByNumberAsync(model.Number);
                if (existingOrderByNumber != null)
                {
                    ViewBag.Failure = "That number is already being used.";
                    return View(model);
                }
            }

            var order = new Order()
            {
                Number = model.Number,
                Appointment = model.Appointment,
                IsUrgent = model.IsUrgent,
                Type = model.Type,
                Location = model.Location,
                Description = model.Description,
                Status = model.Status,
                ClientId = model.ClientId,
                Client = model.Client,
                Company = model.Company
            };

            await _orderRepository.CreateAsync(order);
            if (!await _orderRepository.ExistsAsync(order.Id))
            {
                ViewBag.Failure = "Could not create order.";
                return View(model);
            }

            ViewBag.Success = "Order created successfully!";
            return View(new OrderViewModel
            {
                ClientId = model.ClientId,
                SelectableCompanies = _clientRepository.GetCompanies(model.ClientId).ToList()
            });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });
            }

            var order = await _orderRepository.GetByIdAsync(id.Value);
            if (order == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });
            }

            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var order = await _orderRepository.GetByIdAsync(id.Value);
            if (order == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });
            }

            return View(new OrderViewModel
            {
                Id = order.Id,
                Number = order.Number,
                Execution = order.Execution,
                Appointment = order.Appointment,
                IsUrgent = order.IsUrgent,
                Type = order.Type,
                Location = order.Location,
                Description = order.Description,
                Status = order.Status,
                ClientId = order.ClientId,
                Company = order.Company,
                SelectableCompanies = _clientRepository.GetCompanies(order.ClientId).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OrderViewModel model)
        {
            model.SelectableCompanies = _clientRepository.GetCompanies(model.ClientId).ToList();

            if (!ModelState.IsValid)
            {
                ViewBag.Failure = "Could not update order.";
                return View(model);
            }

            var order = await _orderRepository.GetByIdAsync(model.Id);
            if (order == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });
            }

            if (order.Number == model.Number && order.Execution == model.Execution && order.Appointment == model.Appointment && order.IsUrgent == model.IsUrgent && order.Type == model.Type && order.Location == model.Location && order.Description == model.Description && order.Status == model.Status && order.Company == model.Company)
            {
                ViewBag.Failure = "No changes were found.";
                return View(model);
            }

            if (model.Number != order.Number)
            {
                var existingOrderByNumber = await _orderRepository.GetByNumberAsync(model.Number);
                if (existingOrderByNumber != null)
                {
                    ViewBag.Failure = "That number is already being used.";
                    return View(model);
                }
            }

            order.Number = model.Number;
            order.Execution = model.Execution;
            order.Appointment = model.Appointment;
            order.IsUrgent = model.IsUrgent;
            order.Type = model.Type;
            order.Location = model.Location;
            order.Description = model.Description;
            order.Status = model.Status;
            order.Company = model.Company;

            if (!await _orderRepository.UpdateAsync(order))
            {
                ViewBag.Failure = "Could not update order. This may be due to database constraints or the entity no longer existing.";
                return View(model);
            }

            ViewBag.Success = "Order updated successfully.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });
            }

            if (!await _orderRepository.DeleteAsync(order))
            {
                ViewBag.Failure = "Could not delete order. This may be due to database constraints or the entity no longer existing.";
                return View("Edit", order);
            }

            ViewBag.Success = "Order deleted successfully.";
            return RedirectToAction($"Index");
        }
    }
}
