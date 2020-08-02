using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class ApointmentUpdate
    {
        public ApointmentUpdate()
        {
            
        }

        public Guid PkApointmentUpdateId { get; set; }
        public Guid FkCreatorId { get; set; }
        public Guid FkApointmentId { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public string UpdateNote { get; set; }

        public Apointment FkApointment { get; set; }
    }
}
