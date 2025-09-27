using CustomerManagementPlatform.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CustomerManagementPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, INotificationRepository notificationRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
