using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class PersonEntityType
    {
        public PersonEntityType()
        {
            Person = new HashSet<Person>();
        }

        public Guid PkPersonEntityTypeId { get; set; }
        public string Flag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Person> Person { get; set; }
    }
}
