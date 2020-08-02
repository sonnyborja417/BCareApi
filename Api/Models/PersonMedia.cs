using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class PersonMedia
    {
        public Guid PkPersonMediaId { get; set; }
        public Guid FkPersonId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
