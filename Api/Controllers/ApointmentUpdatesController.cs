using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Api.Hubs;
using Api.Models;
using Api.Models.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/ApointmentUpdates")]
    public class ApointmentUpdatesController : Controller
    {
        private bcareContext context;
        private IHubContext<NotificationHub> hubContext;

        public ApointmentUpdatesController(bcareContext context, IHubContext<NotificationHub> hubContext)
        {
            this.context = context;
            this.hubContext = hubContext;
        }
       

        [HttpPost]
        public IActionResult Create([FromBody] ApointmentUpdate apointmentUpdate)
        {
            apointmentUpdate.CreatedDate = DateTime.Now;
            this.context.ApointmentUpdate.Add(apointmentUpdate);
            this.context.SaveChanges();

            this.hubContext.Clients.All.SendAsync("AppointmentUpdateCreated", new
            {
                AppointmentUpdateId = apointmentUpdate.PkApointmentUpdateId,
                ApointmentId = apointmentUpdate.FkApointmentId
            });

            return Ok(new ResponseResult<Guid>
            {
                Status = ResponseResultStatus.Ok,
                Body = apointmentUpdate.PkApointmentUpdateId
            });
        }

        [HttpGet]
        public IActionResult GetList()
        {

            var updates = this.context.ApointmentUpdate
                .Include(e => e.FkApointment.FkPatient)
                .Where(e=>e.UpdateNote != null)
                .OrderByDescending(e=>e.CreatedDate)
                .ToList();

            return Ok(new ResponseResult<List<ApointmentUpdate>>
            {
                Status = ResponseResultStatus.Ok,
                Body = updates
            });
        }

    }


}