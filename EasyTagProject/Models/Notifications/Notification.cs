using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.Notifications
{
    public class Notification
    {
        [Key]
        [BindNever]
        public int Id { get; set; } = 0;
        public int RoomId { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeCreated { get; set; }
    }
}
