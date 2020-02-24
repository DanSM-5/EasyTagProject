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
        [Required]
        public RepeatTime PeriodType { get; set; }
        [Required]
        public int RepeatNumber { get; set; }
        public bool IncludeWeekends { get; set; }
        [HiddenInput]
        public string Date { get; set; }
    }
}
