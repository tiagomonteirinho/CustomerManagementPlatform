using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Technician")]
    public class ObservationsController : Controller
    {
        private readonly IObservationRepository _observationRepository;
        private readonly IOrderRepository _orderRepository;

        public ObservationsController(IObservationRepository observationRepository, IOrderRepository orderRepository)
        {
            _observationRepository = observationRepository;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Observation" });

            var observation = await _observationRepository.GetByIdAsync(id.Value);
            if (observation == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Observation" });

            return View(new ObservationViewModel
            {
                Id = observation.Id,
                Description = observation.Description,
                OrderId = observation.OrderId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ObservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update observation.";
                return View(model);
            }

            var observation = await _observationRepository.GetByIdAsync(model.Id);
            if (observation == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Observation" });

            if (observation.Description == model.Description)
            {
                TempData["Failure"] = "No changes were found.";
                return View(model);
            }

            observation.Description = model.Description;

            if (!await _observationRepository.UpdateAsync(observation))
            {
                TempData["Failure"] = "Could not update observation. This may be due to the entity being used by other entities or no longer existing.";
                return View(model);
            }

            TempData["Success"] = "Observation updated successfully.";
            return RedirectToAction("Edit", new { id = observation.Id });
        }
    }
}
