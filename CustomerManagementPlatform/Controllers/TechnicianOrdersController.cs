using CustomerManagementPlatform.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Controllers
{
    [Authorize(Roles = "Technician")]
    public class TechnicianOrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public TechnicianOrdersController(IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var technician = await _userRepository.GetByEmailAsync(User.Identity.Name);
            if (technician == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            return View(await _orderRepository.GetByTechnicianIdAsync(technician.Id));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            var order = await _orderRepository.GetByIdAsync(id.Value);
            if (order == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            if (order.Technician.Email != User.Identity.Name)
                return RedirectToAction("Unauthorized401", "Errors");

            return View(order);
        }
    }
}
