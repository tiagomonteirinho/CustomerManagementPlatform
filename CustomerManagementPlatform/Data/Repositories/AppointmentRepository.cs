using CustomerManagementPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public class AppointmentRepository : EntityRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;

        public AppointmentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _context.Appointments.AsNoTracking().Include(a => a.Order).Include(a => a.Technician).ToListAsync();
        }

        public async Task<Appointment> GetByIdAsync(int id)
        {
            return await _context.Appointments.Include(a => a.Order).Include(a => a.Technician).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Appointment>> GetByTechnicianIdAsync(string technicianId)
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Order)
                .Include(a => a.Technician)
                .Where(a => a.TechnicianId == technicianId).ToListAsync();
        }

        public async Task<bool> TechnicianHasConflictAsync(string technicianId, DateTime startTime, DateTime endTime, int? appointmentId)
        {
            if (appointmentId == null)
            {
                return await _context.Appointments.AnyAsync(a => a.TechnicianId == technicianId &&
                    ((startTime >= a.StartTime && startTime < a.EndTime) ||
                    (endTime > a.StartTime && endTime <= a.EndTime) ||
                    (startTime <= a.StartTime && endTime >= a.EndTime)));
            }

            return await _context.Appointments.AnyAsync(a => a.TechnicianId == technicianId && a.Id != appointmentId &&
                ((startTime >= a.StartTime && startTime < a.EndTime) ||
                (endTime > a.StartTime && endTime <= a.EndTime) ||
                (startTime <= a.StartTime && endTime >= a.EndTime)));
        }
    }
}
