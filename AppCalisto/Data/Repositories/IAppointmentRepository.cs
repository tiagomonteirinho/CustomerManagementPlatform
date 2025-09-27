using CustomerManagementPlatform.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public interface IAppointmentRepository : IEntityRepository<Appointment>
    {
        Task<List<Appointment>> GetAllAsync();

        Task<Appointment> GetByIdAsync(int id);

        Task<List<Appointment>> GetByTechnicianIdAsync(string technicianId);

        Task<bool> TechnicianHasConflictAsync(string technicianId, DateTime startTime, DateTime endTime, int? appointmentId);
    }
}
