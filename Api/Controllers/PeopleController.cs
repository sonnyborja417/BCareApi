using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/People")]
    public class PeopleController : Controller
    {
        private bcareContext context;

        public PeopleController(bcareContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult People([FromQuery] string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = string.Empty;
            }

            var people = this.context.Person.Where(e => e.FkPersonEntityType.Flag == "PATIENT" &&
                     //&& e.FullName.ToLower().StartsWith(filter.ToLower())).ToList();
                     (e.FullName.ToLower().Contains(filter.ToLower()) ||
                            e.User.FirstOrDefault().Username.ToLower().Contains(filter.ToLower())
                        )).ToList();

            return Ok(new ResponseResult<List<Person>>
            {
                Status = ResponseResultStatus.Ok,
                Body = people
            });
        }

        [HttpGet("{id}")]
        public IActionResult Person(Guid id)
        {
            var person = this.context.Person.FirstOrDefault(e => e.PkPersonId == id);

            return Ok(new ResponseResult<Person>
            {
                Status = ResponseResultStatus.Ok,
                Body = person
            });
        }

        [HttpPut]
        public IActionResult Update([FromBody] Person person)
        {
            this.context.Person.Update(person);

            var user = this.context.User.FirstOrDefault(e => e.FkPersonId == person.PkPersonId);

            if (user != null)
            {
                user.Username = person.Email;
            }

            this.context.SaveChanges();

            return Ok(new ResponseResult<Person>
            {
                Status = ResponseResultStatus.Ok,
                Body = person
            });
        }
    }
}