using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class Person
    {
        public Person()
        {
            ApointmentFkApprover = new HashSet<Apointment>();
            ApointmentFkCloser = new HashSet<Apointment>();
            ApointmentFkCreator = new HashSet<Apointment>();
            ApointmentFkPatient = new HashSet<Apointment>();
            User = new HashSet<User>();
        }

        public Guid PkPersonId { get; set; }
        public Guid FkPersonEntityTypeId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime? DoB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }

        public PersonEntityType FkPersonEntityType { get; set; }
        public ICollection<Apointment> ApointmentFkApprover { get; set; }
        public ICollection<Apointment> ApointmentFkCloser { get; set; }
        public ICollection<Apointment> ApointmentFkCreator { get; set; }
        public ICollection<Apointment> ApointmentFkPatient { get; set; }
        public ICollection<User> User { get; set; }
    }
}
