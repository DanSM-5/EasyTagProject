using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public interface IAppointmentRepository
    {
        /// <summary>
        /// Returns the collection of appointments in database
        /// </summary>
        IQueryable<Appointment> Appointments { get; }
        /// <summary>
        /// Save an appointment in databes.
        /// </summary>
        /// <param name="appointment">
        /// Appointment to be saved
        /// </param>
        /// <returns></returns>
        Task SaveAsync(Appointment appointment);
        /// <summary>
        /// Delete an appointment by the specified Id
        /// </summary>
        /// <param name="id">
        /// Id od the appointment to be deleted
        /// </param>
        /// <returns></returns>
        Task<Appointment> DeleteAsync(int id);

        Task SaveRangeAsync(IEnumerable<Appointment> appointments);
    }
}
