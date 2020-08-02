using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class Diagnosis
    {
        public Guid PkDiagnosis { get; set; }
        public Guid FkApointmentId { get; set; }
        public string Name { get; set; }

        public Apointment FkApointment { get; set; }
    }
}
