using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class Apointment
    {
        public Apointment()
        {
            Diagnosis = new HashSet<Diagnosis>();
            ApointmentUpdates = new HashSet<ApointmentUpdate>();
        }

        public Guid PkApointmentId { get; set; }
        public Guid FkCreatorId { get; set; }
        public Guid FkPatientId { get; set; }
        public Guid? FkApproverId { get; set; }
        public Guid? FkCloserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApointmentDate { get; set; }
        public string PatientSymptoms { get; set; }
        public string DoctorsNotes { get; set; }
        public bool IsClosed { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public bool IsPatientPending { get; set; }

        public Person FkApprover { get; set; }
        public Person FkCloser { get; set; }
        public Person FkCreator { get; set; }
        public Person FkPatient { get; set; }
        public ICollection<Diagnosis> Diagnosis { get; set; }

        public string Diagnosy { get; set; }
        public DateTime? ClosingDate { get; set; }

        public ICollection<ApointmentUpdate> ApointmentUpdates { get; set; }
    }
}
