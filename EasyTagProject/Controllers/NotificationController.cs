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
        //private Timer NotificationTimer { get; set; }
        //private ElapsedEventHandler ElapsedEvent { get; set; } = null;

        public NotificationController(INotificationConnection notificationRepo)
        {
            NotificationRepository = notificationRepo;
        }

        // GET: api/<controller>
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

        // GET api/<controller>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<controller>/add
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

                    //ElapsedEvent = delegate(object sender, ElapsedEventArgs args) { DeleteNotification(notification.Id); };
                    //ElapsedEvent = (sender, args) => DeleteNotification(notification.Id);

                    //NotificationTimer = new Timer(30 * 60 * 1000);
                    //NotificationTimer.AutoReset = false;
                    //NotificationTimer.Elapsed += ElapsedEvent;// (sender, args) => Deletion(notification.Id);
                    //NotificationTimer.Start();

                    return new OkObjectResult(new { Id = notification.Id });
                }
                catch (Exception)
                {
                    return BadRequest("Error in transactions");
                } 
            }

            return BadRequest("Wrong data received");
        }

        // PUT api/<controller>/5
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

        //[HttpPost("Keep")]
        //public async Task<IActionResult> PostKeepNotification([FromBody] int id)
        //{
        //    if (NotificationRepository.Notifications.Any(n => n.Id == id))
        //    {
        //        await KeepNotification(id);
        //    }
        //    return Ok();
        //}

        //private Task KeepNotification(int id)
        //{
        //    return Task.Run(() => { 
        //        if (NotificationRepository.Notifications.Any(n => n.Id == id))
        //        {
        //            NotificationRepository.Delete(id);
        //        }
        //    });
        //}

        //private void DeleteNotification(int id)
        //{
        //    if (NotificationRepository.Notifications.Any(n => n.Id == id))
        //    {
        //        NotificationRepository.Delete(id);
        //    }

        //    NotificationTimer.Elapsed -= ElapsedEvent;
        //    NotificationTimer.Dispose();
        //    ElapsedEvent = null;
        //}
    }
}
