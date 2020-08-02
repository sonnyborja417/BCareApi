using System;
using System.Collections.Generic;

namespace Api.Models
{
    public class MessageModel
    {
        public MessageModel()
        {            
        }

        public Guid PkMessageId { get; set; } = new Guid();
        public string Message { get; set; }        
        public List<Guid> Contactlist { get; set; } 
    }
}
