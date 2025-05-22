using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private readonly IItemRepository _itemRepository;

        public BudgetsController(IBudgetRepository budgetRepository, IProductRepository productRepository, IOrderRepository orderRepository, IUserRepository userRepository, IItemRepository itemRepository)
        {
            _budgetRepository = budgetRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });

            var budget = await _budgetRepository.GetByIdAsync(id.Value);
            if (budget == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });

            return View(new BudgetViewModel
            {
                Id = budget.Id,
                OrderId = budget.OrderId,
                Items = budget.Items.Select(i => new ItemViewModel
                {
                    Id = i.Id,
                    OrderId = budget.OrderId,
                    ProductId = i.ProductId,
                    Product = i.Product,
                    Quantity = i.Quantity
                }).ToList()
            });
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> AddProduct(int? budgetId)
        {
            if (budgetId == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });

            var budget = await _budgetRepository.GetByIdAsync(budgetId.Value);
            if (budget == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });

            return View(new ItemViewModel
            {
                OrderId = budget.OrderId,
                SelectableProducts = await _productRepository.GetAllAsync() ?? new List<Product>()
            });
        }

        [HttpPost, Authorize(Roles = "Technician"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SelectableProducts = await _productRepository.GetAllAsync() ?? new List<Product>();
                return View(model);
            }

            var budget = await _budgetRepository.GetByOrderIdAsync(model.OrderId);
            if (budget == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Budget" });

            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Product" });

            var existingItem = budget.Items.FirstOrDefault(i => i.ProductId == model.ProductId);
            if (existingItem == null)
            {
                var item = new Item
                {
                    BudgetId = budget.Id,
                    ProductId = model.ProductId,
                    Quantity = model.Quantity
                };

                await _itemRepository.CreateAsync(item);
            }
            else
            {
                existingItem.Quantity += model.Quantity;
                await _itemRepository.UpdateAsync(existingItem);
                return RedirectToAction("AddProduct", new { budgetId = budget.Id });
            }

            if (model.Quantity == 1)
                TempData["Success"] = $"1 unit of {product.Name} added successfully!";
            else
                TempData["Success"] = $"{model.Quantity} units of {product.Name} added successfully!";

            return RedirectToAction("AddProduct", new { budgetId = budget.Id });
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> IncreaseProductQuantity(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Item" });

            var item = await _itemRepository.GetByIdAsync(id.Value);
            if (item == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Item" });

            item.Quantity++;
            await _itemRepository.UpdateAsync(item);
            return RedirectToAction("Edit", new { id = item.BudgetId });
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> DecreaseProductQuantity(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Item" });

            var item = await _itemRepository.GetByIdAsync(id.Value);
            if (item == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Item" });

            item.Quantity = Math.Max(1, item.Quantity - 1);
            await _itemRepository.UpdateAsync(item);
            return RedirectToAction("Edit", new { id = item.BudgetId });
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> RemoveProduct(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Item" });

            var item = await _itemRepository.GetByIdAsync(id.Value);
            if (item == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Item" });

            var budgetId = item.BudgetId;

            if (!await _itemRepository.DeleteAsync(item))
                TempData["Failure"] = "Could not remove item.";

            return RedirectToAction("Edit", new { id = budgetId });
        }
    }
}
