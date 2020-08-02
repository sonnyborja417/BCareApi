using System;
using System.Collections.Generic;

namespace SmsGateway.Models
{
    public partial class MessageLog
    {
        public int Id { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public int? StatusCode { get; set; }
        public string StatusText { get; set; }
        public string MessageTo { get; set; }
        public string MessageFrom { get; set; }
        public string MessageText { get; set; }
        public string MessageType { get; set; }
        public string MessageId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorText { get; set; }
        public string Gateway { get; set; }
        public int? MessageParts { get; set; }
        public string MessagePdu { get; set; }
        public string UserId { get; set; }
        public string UserInfo { get; set; }
        public string Connector { get; set; }
    }
}
