using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Back-office, Technician")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public AppointmentsController(IAppointmentRepository appointmentRepository, IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        [Authorize(Roles = "Back-office")]
        public async Task<IActionResult> Index()
        {
            return View(await _appointmentRepository.GetAllAsync());
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> IndexTechnicianAppointments()
        {
            var technician = await _userRepository.GetByEmailAsync(User.Identity.Name);
            if (technician == null) 
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            return View(await _appointmentRepository.GetByTechnicianIdAsync(technician.Id));
        }

        public async Task<IActionResult> Create(int? orderId)
        {
            if (orderId == null) 
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            var order = await _orderRepository.GetByIdAsync(orderId.Value);
            if (order == null) 
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            return View(new AppointmentViewModel
            {
                OrderId = orderId.Value,
                Order = order,
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create appointment.";
                return View(model);
            }

            var order = await _orderRepository.GetByIdAsync(model.OrderId);
            if (order == null) 
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Order" });

            if (model.EndTime <= model.StartTime)
            {
                TempData["Failure"] = "The field EndTime must be greater than StartTime.";
                return View(model);
            }

            if (await _appointmentRepository.TechnicianHasConflictAsync(order.TechnicianId, model.StartTime, model.EndTime, null))
            {
                TempData["Failure"] = "This technician already has an appointment during the selected time span.";
                return View(model);
            }

            var appointment = new Appointment()
            {
                Location = model.Location,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                OrderId = model.OrderId,
                Order = model.Order,
                TechnicianId = order.TechnicianId,
                Technician = order.Technician
            };

            await _appointmentRepository.CreateAsync(appointment);
            var createdAppointment = await _appointmentRepository.GetByIdAsync(appointment.Id);
            if (createdAppointment == null)
            {
                TempData["Failure"] = "Could not create appointment.";
                return View(model);
            }

            TempData["Success"] = "Appointment created successfully!";
            return RedirectToAction("Create", new { orderId = model.OrderId });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) 
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Appointment" });

            var appointment = await _appointmentRepository.GetByIdAsync(id.Value);
            if (appointment == null) 
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Appointment" });

            return View(appointment);
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> DetailTechnicianAppointment(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Appointment" });

            var appointment = await _appointmentRepository.GetByIdAsync(id.Value);
            if (appointment == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Appointment" });

            if (appointment.Technician.Email != User.Identity.Name)
                return RedirectToAction("Unauthorized401", "Errors");

            return View(appointment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Appointment" });

            var appointment = await _appointmentRepository.GetByIdAsync(id.Value);
            if (appointment == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Appointment" });

            return View(new AppointmentViewModel
            {
                Id = appointment.Id,
                OrderId = appointment.OrderId,
                Order = appointment.Order,
                TechnicianId = appointment.TechnicianId,
                Technician = appointment.Technician,
                Location = appointment.Location,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update appointment.";
                return View(model);
            }

            var appointment = await _appointmentRepository.GetByIdAsync(model.Id);
            if (appointment == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Appointment" });

            if (model.EndTime <= model.StartTime)
            {
                TempData["Failure"] = "The field EndTime must be greater than StartTime.";
                return View(model);
            }

            if (await _appointmentRepository.TechnicianHasConflictAsync(model.TechnicianId, model.StartTime, model.EndTime, model.Id))
            {
                TempData["Failure"] = "This technician already has an appointment during the selected time span.";
                return View(model);
            }

            if (appointment.Location == model.Location && appointment.StartTime == model.StartTime && appointment.EndTime == model.EndTime)
            {
                TempData["Failure"] = "No changes were found.";
                return View(model);
            }

            appointment.Location = model.Location;
            appointment.StartTime = model.StartTime;
            appointment.EndTime = model.EndTime;

            if (!await _appointmentRepository.UpdateAsync(appointment))
            {
                TempData["Failure"] = "Could not update appointment. This may be due to the entity being used by other entities or no longer existing.";
                return View(model);
            }

            TempData["Success"] = "Appointment updated successfully.";
            return RedirectToAction("Edit", new { id = appointment.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Appointment" });

            var existingAppointments = await _appointmentRepository.GetAllAsync();
            if (existingAppointments.Count == 1)
            {
                TempData["Failure"] = "There must be at least one appointment in the database.";
                return RedirectToAction("Edit", new { id });
            }

            if (!await _appointmentRepository.DeleteAsync(appointment))
            {
                TempData["Failure"] = "Could not delete appointment. This may be due to the entity being used by other entities or no longer existing.";
                return RedirectToAction("Edit", new { id });
            }

            TempData["Success"] = "Appointment deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
