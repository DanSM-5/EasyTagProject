using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class Appointment : IComparable<Appointment>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]       
        public DateTime End { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Course { get; set; }
        public string Description { get; set; }
        public int ScheduleId { get; set; }
        public int RoomId { get; set; }
        [NotMapped]
        public string RoomCode { get; set; }

        public int CompareTo([AllowNull] Appointment other)
        {
            return Start.CompareTo(other.Start);
        }
    }
}