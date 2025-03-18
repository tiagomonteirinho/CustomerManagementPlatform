using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CustomerManagementPlatform.Controllers
{
    public class ErrorsController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string title, string message) // Error view for deletion, update or unhandled exceptions.
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Title = title,
                Message = message
            });
        }

        public IActionResult Unauthorized401() // Error view for unauthorized or forbidden access.
        {
            return View();
        }

        [Route("/NotFound404")]
        public IActionResult NotFound404(string entityName) // Not found error view for unknown pages or specific entities.
        {
            return View("NotFound404", entityName);
        }
    }
}
