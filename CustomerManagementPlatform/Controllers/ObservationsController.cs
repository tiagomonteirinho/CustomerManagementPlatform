using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Data.Repositories;
using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Controllers
{
    [Authorize(Roles = "Technician")]
    public class ObservationsController : Controller
    {
        private readonly IObservationRepository _observationRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ObservationsController(IObservationRepository observationRepository, IOrderRepository orderRepository, IWebHostEnvironment webHostEnvironment)
        {
            _observationRepository = observationRepository;
            _orderRepository = orderRepository;
            _webHostEnvironment = webHostEnvironment;
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
                OrderId = observation.OrderId,
                ExistingImageUrls = observation.Images.Select(i => i.Url).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ObservationViewModel model)
        {
            var observation = await _observationRepository.GetByIdAsync(model.Id);
            if (observation == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Observation" });

            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update observation.";
                return View(new ObservationViewModel
                {
                    Id = observation.Id,
                    Description = observation.Description,
                    OrderId = observation.OrderId,
                    ExistingImageUrls = observation.Images.Select(i => i.Url).ToList()
                });
            }

            bool hasNewImages = model.ImageFiles != null && model.ImageFiles.Any();
            if (observation.Description == model.Description && !hasNewImages)
            {
                TempData["Failure"] = "No changes were found.";
                return View(new ObservationViewModel
                {
                    Id = observation.Id,
                    Description = observation.Description,
                    OrderId = observation.OrderId,
                    ExistingImageUrls = observation.Images.Select(i => i.Url).ToList()
                });
            }

            observation.Description = model.Description;

            foreach (var imageFile in model.ImageFiles)
            {
                var guid = Guid.NewGuid().ToString();
                var fileName = $"observation{observation.Id}_{guid}.jpg";

                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "observations", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                observation.Images.Add(new ObservationImage
                {
                    Url = $"/images/observations/{fileName}"
                });
            }

            if (!await _observationRepository.UpdateAsync(observation))
            {
                TempData["Failure"] = "Could not update observation. This may be due to the entity being used by other entities or no longer existing.";
                return View(new ObservationViewModel
                {
                    Id = observation.Id,
                    Description = observation.Description,
                    OrderId = observation.OrderId,
                    ExistingImageUrls = observation.Images.Select(i => i.Url).ToList()
                });
            }

            TempData["Success"] = "Observation updated successfully.";
            return RedirectToAction("Edit", new { id = observation.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int observationId, string imageUrl)
        {
            var observation = await _observationRepository.GetByIdAsync(observationId);
            if (observation == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Observation" });

            var image = observation.Images.FirstOrDefault(i => i.Url == imageUrl);
            if (image == null)
            {
                TempData["Failure"] = "Could not delete observation image. This may be due to the entity being used by other entities or no longer existing.";
                return RedirectToAction("Edit", new { id = observationId });
            }

            // Remove from collection.
            observation.Images.Remove(image);

            // Delete from database context.
            _observationRepository.DeleteImage(image);

            // Delete physical file.
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.Url.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            await _observationRepository.UpdateAsync(observation);
            return RedirectToAction("Edit", new { id = observationId });
        }
    }
}
