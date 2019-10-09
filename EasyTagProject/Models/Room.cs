using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyTagProject.Models
{
    public enum Type { Classroom, Office, Bathroom}
    public enum Status { Empty, InClass, Closed}
    public class Room
    {
        [Key]
        public int Id { get; set; }
        //[RegularExpression(@"^[A-Z][0-9]-\d{2}$", ErrorMessage = "Please enter a valid short name!!!")]
        [Required]
        public string Name { get; set; }
        public char Block { get; set; }
        public int Floor { get; set; }
        public int Number { get; set; }
        public Type Type { get; set; }
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
        public Room LeftRoom { get; set; }
        public Room RightRoom { get; set; }
        public bool Availability { get; set; }
        public ICollection<Item> Items {get; set; }
        public Schedule Schedule { get; set; }
    }
}
