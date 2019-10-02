using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyTagProject.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Room LeftRoom { get; set; }
        public Room RightRoom { get; set; }
        public bool Availability { get; set; }
        public ICollection<Item> Items {get; set; }
        public Schedule Schedule { get; set; }
    }
}
