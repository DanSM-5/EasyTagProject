using ExpressiveAnnotations.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.ViewModels
{
    public class RepeatAppViewModel
    {
        [Required(ErrorMessage = "Invalid Appointment")]
        //[UIHint("Hidden")]
        [HiddenInput]
        public string Code { get; set; }
        [Required(ErrorMessage = "Invalid Appointment")]
        //[UIHint("Hidden")]
        [HiddenInput]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please select a period")]
        public RepeatTime PeriodType { get; set; }
        [Required(ErrorMessage = "You have to repeat at least once")]
        public int RepeatNumber { get; set; }
        [Required]
        public bool IncludeWeekends { get; set; }
        [HiddenInput]
        public string Date { get; set; }
    }
}
