﻿using EasyTagProject.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    /*
     * Model class for Appointment objects
     */
    public class Appointment : IComparable<Appointment>, IDisposable
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]       
        public DateTime End { get; set; }
        [Required]
        public string Course { get; set; }
        [Required]
        public string Description { get; set; }
        public int ScheduleId { get; set; }
        public int RoomId { get; set; }
        [Required]
        public string UserName { get; set; }
        public string UserId { get; set; }
        [Timestamp]
        [HiddenInput]
        public byte[] RowVersion { get; set; }

        // Used for reference and validation
        [NotMapped]
        public string RoomCode { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public bool ErrorHappened { get; set; } = false;


        public int CompareTo([AllowNull] Appointment other)
        {
            return Start.CompareTo(other.Start);
        }

        public void Dispose(){}

        public Appointment Clone()
        {
            return new Appointment
            {
                Start = Start,
                End = End,
                Course = Course,
                Description = Description,
                ScheduleId = ScheduleId,
                RoomId = RoomId,
                UserName = UserName,
                UserId = UserId,
                RoomCode = RoomCode
            };
        }

        public bool InformationChanged(Appointment other)
        {
            return !(Start == other.Start &&
                   End == other.End &&
                   Course == other.Course &&
                   Description == other.Description &&
                   ScheduleId == other.ScheduleId &&
                   RoomId == other.RoomId &&
                   UserName == other.UserName);
        }
    }
}