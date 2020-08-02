using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Transport
{
    public class ResponseResult<T>
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public T Body { get; set; }
    }
}
