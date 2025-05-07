using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Back-office, Technician")]
    public class BudgetsController : Controller
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public BudgetsController(IBudgetRepository budgetRepository, IProductRepository productRepository, IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _budgetRepository = budgetRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        [Authorize(Roles = "Back-office")]
        public async Task<IActionResult> Index()
        {
            return View(await _budgetRepository.GetAllAsync());
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> IndexTechnicianBudgets()
        {
            var technician = await _userRepository.GetByEmailAsync(User.Identity.Name);
            if (technician == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            return View(await _budgetRepository.GetByTechnicianAsync(technician));
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> Create(int? orderId)
        {
            if (orderId == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });
            }

            var order = await _orderRepository.GetByIdAsync(orderId.Value);
            if (order == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });
            }

            return View(new BudgetViewModel
            {
                OrderId = orderId.Value,
                SelectableProducts = _productRepository.GetAll() ?? new List<SelectListItem>()
            });
        }

        [Authorize(Roles = "Technician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create budget.";
                return View(new BudgetViewModel
                {
                    OrderId = model.OrderId,
                    SelectableProducts = _productRepository.GetAll() ?? new List<SelectListItem>()
                });
            }

            var budget = new Budget
            {
                OrderId = model.OrderId,
                Description = model.Description,
                Status = model.Status,
                BudgetProducts = new List<BudgetProduct>()
            };

            if (model.ProductIds != null && model.ProductIds.Any())
            {
                foreach (var productId in model.ProductIds)
                {
                    budget.BudgetProducts.Add(new BudgetProduct
                    {
                        ProductId = productId
                    });
                }
            }

            await _budgetRepository.CreateAsync(budget);
            var createdBudget = await _budgetRepository.GetByIdAsync(budget.Id);
            if (budget == null)
            {
                TempData["Failure"] = "Could not create budget.";
                return View(new BudgetViewModel
                {
                    OrderId = model.OrderId,
                    SelectableProducts = _productRepository.GetAll() ?? new List<SelectListItem>()
                });
            }

            TempData["Success"] = "Budget created successfully!";
            return RedirectToAction("Create", new { orderId = model.OrderId });
        }

        [Authorize(Roles = "Back-office")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });
            }

            var budget = await _budgetRepository.GetByIdAsync(id.Value);
            if (budget == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });
            }

            return View(budget);
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> DetailTechnicianBudget(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });
            }

            var budget = await _budgetRepository.GetByIdAsync(id.Value);
            if (budget == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });
            }

            if (budget.Order.Technician.Email != User.Identity.Name)
            {
                return RedirectToAction("Unauthorized401", "Errors");
            }

            return View(budget);
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });
            }

            var budget = await _budgetRepository.GetByIdAsync(id.Value);
            if (budget == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });
            }

            return View(new BudgetViewModel
            {
                Id = budget.Id,
                OrderId = budget.OrderId,
                Status = budget.Status,
                SelectableProducts = _productRepository.GetAll() ?? new List<SelectListItem>()
            });
        }

        [Authorize(Roles = "Technician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BudgetViewModel model)
        {
            var budget = await _budgetRepository.GetByIdAsync(model.Id);
            if (budget == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });
            }

            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update budget.";
                return View(new BudgetViewModel
                {
                    Id = budget.Id,
                    OrderId = budget.OrderId,
                    Status = budget.Status,
                    SelectableProducts = _productRepository.GetAll() ?? new List<SelectListItem>()
                });
            }

            if (budget.Description == model.Description && budget.Status == model.Status)
            {
                TempData["Failure"] = "No changes were found.";
                return View(new BudgetViewModel
                {
                    Id = budget.Id,
                    OrderId = budget.OrderId,
                    Status = budget.Status,
                    SelectableProducts = _productRepository.GetAll() ?? new List<SelectListItem>()
                });
            }

            budget.Description = model.Description;
            budget.Status = model.Status;

            if (!await _budgetRepository.UpdateAsync(budget))
            {
                TempData["Failure"] = "Could not update budget. This may be due to the entity being used by other entities or no longer existing.";
                return View(new BudgetViewModel
                {
                    Id = budget.Id,
                    OrderId = budget.OrderId,
                    Status = budget.Status,
                    SelectableProducts = _productRepository.GetAll() ?? new List<SelectListItem>()
                });
            }

            TempData["Success"] = "Budget updated successfully.";
            return RedirectToAction("Edit");
        }

        [Authorize(Roles = "Technician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var budget = await _budgetRepository.GetByIdAsync(id);
            if (budget == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });
            }

            var existingBudgets = await _budgetRepository.GetAllAsync();
            if (existingBudgets.Count == 1)
            {
                TempData["Failure"] = "There must be at least one budget in the database.";
                return RedirectToAction("Edit", new { id });
            }

            if (!await _budgetRepository.DeleteAsync(budget))
            {
                TempData["Failure"] = "Could not delete budget. This may be due to the entity being used by other entities or no longer existing.";
                return RedirectToAction("Edit", new { id });
            }

            TempData["Success"] = "Budget deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
