using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Data.Repositories;
using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Controllers
{
    [Authorize(Roles = "Back-office")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IObservationRepository _observationRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly INotificationRepository _notificationRepository;

        public OrdersController(IOrderRepository orderRepository, IClientRepository clientRepository, ICompanyRepository companyRepository, IServiceRepository serviceRepository, IUserRepository userRepository, IObservationRepository observationRepository, IBudgetRepository budgetRepository, INotificationRepository notificationRepository)
        {
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _companyRepository = companyRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _observationRepository = observationRepository;
            _budgetRepository = budgetRepository;
            _notificationRepository = notificationRepository;
        }

        public IActionResult GetCompanyServices(int companyId)
        {
            var services = _companyRepository.GetServices(companyId);
            return Json(services);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _orderRepository.GetAllAsync());
        }

        private async Task<OrderViewModel> BuildCreateOrderViewModelAsync(int clientId)
        {
            return new OrderViewModel
            {
                ClientId = clientId,
                Creation = DateTime.Now,
                SelectableCompanies = _companyRepository.GetAll() ?? new List<SelectListItem>(),
                SelectableServices = _companyRepository.GetServices(0) ?? new List<SelectListItem>(),
                SelectableTechnicians = await _userRepository.GetAllByRoleAsync("Technician") ?? new List<SelectListItem>()
            };
        }

        public async Task<IActionResult> Create(int? clientId)
        {
            if (clientId == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });

            var client = await _clientRepository.GetByIdAsync(clientId.Value);
            if (client == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Client" });

            return View(await BuildCreateOrderViewModelAsync(clientId.Value));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create order.";
                return View(await BuildCreateOrderViewModelAsync(model.ClientId));
            }

            var order = new Order()
            {
                IsUrgent = model.IsUrgent,
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

            var observation = new Observation { OrderId = order.Id };
            await _observationRepository.CreateAsync(observation);

            var budget = new Budget { OrderId = order.Id };
            await _budgetRepository.CreateAsync(budget);

            order.Observation = observation;
            order.Budget = budget;
            if (!await _orderRepository.UpdateAsync(order))
            {
                TempData["Failure"] = "Could not create order.";
                return View(await BuildCreateOrderViewModelAsync(model.ClientId));
            }

            var notification = new Notification
            {
                Title = "New order",
                Message = "A new order has been assigned to you.",
                UserId = order.TechnicianId
            };

            await _notificationRepository.CreateAsync(notification);
            notification.Action = $"<a href=\"{Url.Action("Details", "TechnicianOrders", new { id = order.Id })}\" class=\"btn btn-primary\">Take me there</a>";
            await _notificationRepository.UpdateAsync(notification);

            TempData["Success"] = "Order created successfully!";
            return RedirectToAction("Create", new { clientId = model.ClientId });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            var order = await _orderRepository.GetByIdAsync(id.Value);
            if (order == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            return View(order);
        }

        private async Task<OrderViewModel> BuildEditOrderViewModelAsync(Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                ClientId = order.ClientId,
                CompanyId = order.Service.CompanyId,
                SelectableCompanies = _companyRepository.GetAll() ?? new List<SelectListItem>(),
                ServiceId = order.ServiceId,
                SelectableServices = _companyRepository.GetServices(order.Service.CompanyId) ?? new List<SelectListItem>(),
                TechnicianId = order.TechnicianId,
                SelectableTechnicians = await _userRepository.GetAllByRoleAsync("Technician") ?? new List<SelectListItem>(),
                IsUrgent = order.IsUrgent,
                IsClosed = order.IsClosed,
                Status = order.Status,
                ClientDescription = order.ClientDescription
            };
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            var order = await _orderRepository.GetByIdAsync(id.Value);
            if (order == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            return View(await BuildEditOrderViewModelAsync(order));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderViewModel model)
        {
            var order = await _orderRepository.GetByIdAsync(model.Id);
            if (order == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update order.";
                return View(await BuildEditOrderViewModelAsync(order));
            }

            if (order.ServiceId == model.ServiceId && order.TechnicianId == model.TechnicianId
                && order.IsUrgent == model.IsUrgent && order.Status == model.Status && order.ClientDescription == model.ClientDescription)
            {
                TempData["Failure"] = "No changes were found.";
                return View(await BuildEditOrderViewModelAsync(order));
            }

            order.ClientId = model.ClientId;
            order.ServiceId = model.ServiceId;
            order.TechnicianId = model.TechnicianId;
            order.IsUrgent = model.IsUrgent;
            order.Status = model.Status;
            order.ClientDescription = model.ClientDescription;

            if (!await _orderRepository.UpdateAsync(order))
            {
                TempData["Failure"] = "Could not update order. This may be due to the entity being used by other entities or no longer existing.";
                return View(await BuildEditOrderViewModelAsync(order));
            };

            TempData["Success"] = "Order updated successfully.";
            return View(await BuildEditOrderViewModelAsync(order));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            var existingOrders = await _orderRepository.GetAllAsync();
            if (existingOrders.Count == 1)
            {
                TempData["Failure"] = "There must be at least one order in the database.";
                return RedirectToAction("Edit", new { id });
            }

            if (order.IsClosed)
            {
                TempData["Failure"] = "This order has already been closed, and cannot be deleted.";
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            if (order.IsClosed)
            {
                TempData["Failure"] = "This order had already been closed.";
                return RedirectToAction("Details", new { id });
            }

            order.IsClosed = true;

            if (!await _orderRepository.UpdateAsync(order))
            {
                TempData["Failure"] = "Could not close order. This may be due to the entity being used by other entities or no longer existing.";
                return RedirectToAction("Details", new { id });
            }

            TempData["Success"] = "Order closed successfully.";
            return RedirectToAction("Details", new { id });
        }
    }
}
