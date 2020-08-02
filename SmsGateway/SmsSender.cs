using SmsGateway.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmsGateway
{
    public class SmsSender
    {
        private SMSServerContext context;

        public SmsSender(SMSServerContext context)
        {
            this.context = context;
        }

        public void Send(List<string> to, string message)
        {
            to.ForEach(reciever => 
            {
                this.Send(reciever, message);
            });
        }

        public void Send(string to, string message)
        {
            var reciever = to.StartsWith("+63") ? to : string.Format("+63{0}", to.Substring(1));

            this.context.MessageOut.Add(new MessageOut
            {
                MessageTo = reciever,
                MessageText = message,
                MessageType = "sms.automatic"
            });

            this.context.SaveChanges();
        }
    }
}
