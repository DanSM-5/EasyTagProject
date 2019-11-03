using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EasyTagProject.Models
{
    public enum Type { Classroom, Office, Bathroom}
    public enum Status { Empty, InClass, Closed}
    public class Room
    {
        [Key]
        public int Id { get; set; }
        //[RegularExpression(@"^[A-Z][0-9]-\d{2}$", ErrorMessage = "Please enter a valid short name!!!")]
        public string Name { get; set; }
        [Required]
        public char Block { get; set; }
        [Required]
        [Range(1, 3, ErrorMessage = "Invalid Floor number")]
        public int Floor { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = "Invalid number")]
        public int Number { get; set; }
        [Required]
        public Type? Type { get; set; }
        [BindNever]
        public Status Status
        {
            get
            {
                DateTime time = DateTime.Now;


                if (Schedule.Appointments.Any(a => a.Start < time && a.End > time))
                {
                    return Status.InClass;
                }
                else
                {
                    return Status.Empty;
                }
            }
        }
        
        public string RoomCode {get; set;}  
        [BindNever]
        public ICollection<Item> Items { get; set; } = new List<Item>();
        [BindNever]
        public Schedule Schedule { get; set; } = new Schedule();
    }
}
