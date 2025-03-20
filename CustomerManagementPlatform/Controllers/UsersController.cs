using CustomerManagementPlatform.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            foreach (var user in users)
            {
                user.Roles = await _userRepository.GetUserRolesAsync(user);
            }

            return View(users);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            user.Roles = await _userRepository.GetUserRolesAsync(user);

            return View(user);
        }
    }
}
