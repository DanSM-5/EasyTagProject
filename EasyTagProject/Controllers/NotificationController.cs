using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using EasyTagProject.Models.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EasyTagProject.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        public INotificationConnection NotificationRepository { get; }

        public NotificationController(INotificationConnection notificationRepo)
        {
            NotificationRepository = notificationRepo;
        }

        // GET: api/<controller>/id/date
        [HttpGet("{id}/{date}")]
        public async Task<IActionResult> Get(int id, DateTime date)
        {
            if (id == 0 || date == default) 
            {
                return BadRequest("Missing Data");
            }
            else
            {
                return new OkObjectResult(new { existNotification = await NotificationRepository.Notifications.AnyAsync(n => n.Date.Date == date.Date && n.RoomId == id) }); 
            }
        }

        // POST api/<controller>/set
        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] Notification notification)
        {
            if (notification != null)
            {
                notification.Id = 0;
                try
                {
                    if (NotificationRepository.Notifications.Any(n => n.RoomId == notification.RoomId && n.Date == notification.Date))
                    {
                        return BadRequest("Notification alredy set");
                    }
                    await NotificationRepository.Create(notification);

                    return new OkObjectResult(new { Id = notification.Id });
                }
                catch (Exception)
                {
                    return BadRequest("Error in transactions");
                } 
            }

            return BadRequest("Wrong data received");
        }

        // POST api/<controller>/unset
        [HttpPost("unset")]
        public async Task<IActionResult> Post([FromBody] int id)
        {
            if (id != 0)
            {
                try
                {
                    await NotificationRepository.Delete(id);
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest("Error in transaction");
                }
            }
            return BadRequest("Invalid value");
        }

        // POST api/<controller>/keep
        [HttpPost("keep")]
        public async Task<IActionResult> PostKeepNotification([FromBody] int id)
        {
            try
            {
                if (NotificationRepository.Notifications.Any(n => n.Id == id))
                {
                    await NotificationRepository.UpdateTimeCreated(id);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Notification not found!");
            }
        }
    }
}
