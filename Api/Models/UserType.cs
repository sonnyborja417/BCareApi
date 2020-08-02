using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class UserType
    {
        public UserType()
        {
            User = new HashSet<User>();
        }

        public Guid PkUserTypeId { get; set; }
        public string Flag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<User> User { get; set; }
    }
}
