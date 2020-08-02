using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class User
    {
        public Guid PkUserId { get; set; }
        public Guid FkUserTypeId { get; set; }
        public Guid FkPersonId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool? Enabled { get; set; }
        public string Token { get; set; }

        public Person FkPerson { get; set; }
        public UserType FkUserType { get; set; }
    }
}
