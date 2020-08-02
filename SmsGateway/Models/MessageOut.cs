using System;
using System.Collections.Generic;

namespace SmsGateway.Models
{
    public partial class MessageOut
    {
        public int Id { get; set; }
        public string MessageTo { get; set; }
        public string MessageFrom { get; set; }
        public string MessageText { get; set; }
        public string MessageType { get; set; }
        public string Gateway { get; set; }
        public string UserId { get; set; }
        public string UserInfo { get; set; }
        public int? Priority { get; set; }
        public DateTime? Scheduled { get; set; }
        public int? ValidityPeriod { get; set; }
        public bool IsRead { get; set; }
        public bool IsSent { get; set; }
    }
}
