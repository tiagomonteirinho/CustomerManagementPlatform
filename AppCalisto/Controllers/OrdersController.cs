using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Back-office, Technician")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;

        public OrdersController(IOrderRepository orderRepository, IClientRepository clientRepository, ICompanyRepository companyRepository, IServiceRepository serviceRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _companyRepository = companyRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
        }

        public IActionResult GetCompanyServices(int companyId)
        {
            var services = _companyRepository.GetServices(companyId);
            return Json(services);
        }

        [Authorize(Roles = "Back-office")]
        public async Task<IActionResult> Index()
        {
            return View(await _orderRepository.GetAllAsync());
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> IndexTechnicianOrders()
        {
            var technician = await _userRepository.GetByEmailAsync(User.Identity.Name);
            if (technician == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            return View(await _orderRepository.GetByTechnicianAsync(technician));
        }

        private async Task<OrderViewModel> BuildCreateOrderViewModelAsync(int clientId)
        {
            return new OrderViewModel
            {
                ClientId = clientId,
                Creation = DateTime.Now,
                Appointment = DateTime.Now,
                SelectableCompanies = _companyRepository.GetAll() ?? new List<SelectListItem>(),
                SelectableServices = _companyRepository.GetServices(0) ?? new List<SelectListItem>(),
                SelectableTechnicians = await _userRepository.GetAllByRoleAsync("Technician") ?? new List<SelectListItem>()
            };
        }

        [Authorize(Roles = "Back-office")]
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

            return View(await BuildCreateOrderViewModelAsync(clientId.Value));
        }

        [Authorize(Roles = "Back-office")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create order.";
                return View(await BuildCreateOrderViewModelAsync(model.ClientId));
            }

            var order = new Order()
            {
                Appointment = model.Appointment,
                IsUrgent = model.IsUrgent,
                Location = model.Location,
                Description = model.Description,
                ClientId = model.ClientId,
                ServiceId = model.ServiceId,
                TechnicianId = model.TechnicianId
            };

            await _orderRepository.CreateAsync(order);
            var createdOrder = await _orderRepository.GetByIdAsync(order.Id);
            if (order == null)
            {
                TempData["Failure"] = "Could not create order.";
                return View(await BuildCreateOrderViewModelAsync(model.ClientId));
            }

            TempData["Success"] = "Order created successfully!";
            return RedirectToAction("Create", new { clientId = model.ClientId });
        }

        [Authorize(Roles = "Back-office")]
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

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> DetailTechnicianOrder(int? id)
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

            if (order.Technician.Email != User.Identity.Name)
            {
                return RedirectToAction("Unauthorized401", "Errors");
            }

            return View(order);
        }

        private async Task<OrderViewModel> BuildEditOrderViewModelAsync(Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                Creation = order.Creation,
                Execution = order.Execution,
                Appointment = order.Appointment,
                IsUrgent = order.IsUrgent,
                Location = order.Location,
                Description = order.Description,
                ClientId = order.ClientId,
                CompanyId = order.Service.CompanyId,
                SelectableCompanies = _companyRepository.GetAll() ?? new List<SelectListItem>(),
                ServiceId = order.ServiceId,
                SelectableServices = _companyRepository.GetServices(order.Service.CompanyId) ?? new List<SelectListItem>(),
                TechnicianId = order.TechnicianId,
                SelectableTechnicians = await _userRepository.GetAllByRoleAsync("Technician") ?? new List<SelectListItem>()
            };
        }

        [Authorize(Roles = "Back-office")]
        public async Task<IActionResult> Edit(int? id)
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

            return View(await BuildEditOrderViewModelAsync(order));
        }

        [Authorize(Roles = "Back-office")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderViewModel model)
        {
            var order = await _orderRepository.GetByIdAsync(model.Id);
            if (order == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });
            }

            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update order.";
                return View(await BuildEditOrderViewModelAsync(order));
            }

            if (order.Execution == model.Execution && order.Appointment == model.Appointment && order.IsUrgent == model.IsUrgent && order.Location == model.Location && order.Description == model.Description
                && order.ServiceId == model.ServiceId && order.TechnicianId == model.TechnicianId)
            {
                TempData["Failure"] = "No changes were found.";
                return View(await BuildEditOrderViewModelAsync(order));
            }

            order.ClientId = model.ClientId;
            order.ServiceId = model.ServiceId;
            order.TechnicianId = model.TechnicianId;
            order.Execution = model.Execution;
            order.Appointment = model.Appointment;
            order.IsUrgent = model.IsUrgent;
            order.Location = model.Location;
            order.Description = model.Description;

            if (!await _orderRepository.UpdateAsync(order))
            {
                TempData["Failure"] = "Could not update order. This may be due to the entity being used by other entities or no longer existing.";
                return View(await BuildEditOrderViewModelAsync(order));
            };

            TempData["Success"] = "Order updated successfully.";
            return RedirectToAction("Edit", new { id = order.Id });
        }

        [Authorize(Roles = "Back-office")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });
            }

            var existingOrders = await _orderRepository.GetAllAsync();
            if (existingOrders.Count == 1)
            {
                TempData["Failure"] = "There must be at least one order in the database.";
                return RedirectToAction("Edit", new { id });
            }

            if (!await _orderRepository.DeleteAsync(order))
            {
                TempData["Failure"] = "Could not delete order. This may be due to the entity being used by other entities or no longer existing.";
                return RedirectToAction("Edit", new { id });
            }

            TempData["Success"] = "Order deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
