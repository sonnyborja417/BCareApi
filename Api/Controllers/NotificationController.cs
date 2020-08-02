using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmsGateway;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Notification")]
    public class NotificationController : Controller
    {
        private SmsSender smsSender;

        public NotificationController(SmsSender smsSender)
        {
            this.smsSender = smsSender;
        }

        [HttpPost("SendSms")]
        public void SendSms([FromBody] Dictionary<string, string> messages)
        {
            foreach (var message in messages)
            {
                this.smsSender.Send(message.Key, message.Value);
            }
        }
    }
}