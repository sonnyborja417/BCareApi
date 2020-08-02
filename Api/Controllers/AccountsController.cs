using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Accounts")]
    public class AccountsController : Controller
    {
        private bcareContext context;
        private IConfiguration configuration;
        private IDataProtectionProvider dataProtectionProvider;

        public AccountsController(bcareContext context, IConfiguration configuration, IDataProtectionProvider dataProtectionProvider)
        {
            this.context = context;
            this.configuration = configuration;
            this.dataProtectionProvider = dataProtectionProvider;
        }

        [HttpGet]
        public IActionResult Accounts([FromQuery] string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = string.Empty;
            }

            var users = this.context.User
                .Include(e => e.FkPerson)
                .Include(e => e.FkUserType)
                .Where(e =>  (e.Username.ToLower().StartsWith(filter.ToLower())) && (e.FkPerson.FullName.ToLower().StartsWith(filter.ToLower())))
                .ToList();

            return Ok(new ResponseResult<List<User>>
            {
                Status = ResponseResultStatus.Ok,
                Body = users
            });
        }

        [HttpGet("Employee")]
        public IActionResult Employee([FromQuery] string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = string.Empty;
            }
            
            var users = this.context.User
                .Where(e => e.FkPerson.FkPersonEntityType.Flag == "EMPLOYEE" &&
                        (e.FkPerson.FullName.ToLower().Contains(filter.ToLower()) ||
                            e.Username.ToLower().Contains(filter.ToLower())
                        )
                        )
                            //((e.Username.ToLower().StartsWith(filter.ToLower())) && 
                             //(e.FkPerson.FullName.ToLower().StartsWith(filter.ToLower()))))
                .Include(e => e.FkPerson).Include(e => e.FkUserType)
                .ToList();

            return Ok(new ResponseResult<List<User>>
            {
                Status = ResponseResultStatus.Ok,
                Body = users
            });
        }

        [HttpGet("Patient")]
        public IActionResult Patient()
        {
            var users = this.context.User
                .Where(e => e.FkPerson.FkPersonEntityType.Flag == "PATIENT")
                .Include(e => e.FkPerson).Include(e => e.FkUserType)
                .ToList();

            return Ok(new ResponseResult<List<User>>
            {
                Status = ResponseResultStatus.Ok,
                Body = users
            });
        }

        [HttpGet("{id}")]
        public IActionResult AccountInformation(Guid id)
        {
            var user = this.context.User
                .Include(e => e.FkPerson)
                .Include(e => e.FkUserType)
                .FirstOrDefault(e => e.PkUserId == id);

            if (user != null)
            {
                return Ok(new ResponseResult<User>
                {
                    Status = ResponseResultStatus.Ok,
                    Body = user
                });
            }

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Error,
                Message = "Account not found"
            });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody]User user, [FromQuery]string userTypeFlag = "PATIENT", [FromQuery]string personTypeFlag = "PATIENT")
        {
            if (this.context.User.Any(e => e.Username.ToLower() == user.Username.ToLower()))
            {
                return Ok(new ResponseResult<object>
                {
                    Status = ResponseResultStatus.Error,
                    Message = "Email Address Already in Use"
                });
            }

            var userType = this.context.UserType.FirstOrDefault(e => e.Flag == userTypeFlag);
            var personType = this.context.PersonEntityType.FirstOrDefault(e => e.Flag == personTypeFlag);

            if (userTypeFlag != null)
            {
                user.FkUserTypeId = userType.PkUserTypeId;
            }

            if (personType != null)
            {
                user.FkPerson.FkPersonEntityTypeId = personType.PkPersonEntityTypeId;
            }

            var authenticationTokens = this.GenerateToken(user.Username);

            user.Token = authenticationTokens.userToken;

            this.context.User.Add(user);
            this.context.SaveChanges();

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Ok,
                Body = new
                {
                    Id = user.PkUserId,
                    Token = authenticationTokens.jwtToken
                }
            });
        }

        [AllowAnonymous]
        [HttpPost("Login/Web")]
        public IActionResult LoginWeb([FromBody]User user)
        {
            if (this.context.User.Any(e => e.Username.ToLower() == user.Username.ToLower() && e.Password == user.Password && e.FkPerson.FkPersonEntityType.Flag == "EMPLOYEE"))
            {
                var account = this.context.User.First(e => 
                    e.Username.ToLower() == user.Username.ToLower() &&
                    e.Password == user.Password &&
                    e.FkPerson.FkPersonEntityType.Flag == "EMPLOYEE");

                var authenticationTokens = this.GenerateToken(user.Username);

                account.Token = authenticationTokens.userToken;

                this.context.SaveChanges();

                return Ok(new ResponseResult<object>
                {
                    Status = ResponseResultStatus.Ok,
                    Body = new
                    {
                        Id = account.PkUserId,
                        Token = authenticationTokens.jwtToken
                    }
                });
            }

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Error,
                Message = "Account Not Found"
            });
        }

        [AllowAnonymous]
        [HttpPost("Login/Mobile")]
        public IActionResult LoginMobile([FromBody]User user)
        {
            if (this.context.User.Any(e => 
                e.Username.ToLower() == user.Username.ToLower() && 
                e.Password == user.Password &&
                e.FkPerson.FkPersonEntityType.Flag == "PATIENT"))
            {
                var account = this.context.User.FirstOrDefault(e => e.Username.ToLower() == user.Username.ToLower() && e.Password == user.Password && e.FkPerson.FkPersonEntityType.Flag == "PATIENT");
                var authenticationTokens = this.GenerateToken(user.Username);

                account.Token = authenticationTokens.userToken;

                this.context.SaveChanges();

                return Ok(new ResponseResult<object>
                {
                    Status = ResponseResultStatus.Ok,
                    Body = new
                    {
                        Id = account.PkUserId,
                        Token = authenticationTokens.jwtToken
                    }
                });
            }

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Error,
                Message = "Account Not Found"
            });
        }

        [HttpPut]
        public IActionResult Update([FromBody]User user, [FromQuery]string userTypeFlag = "PATIENT", [FromQuery]string personTypeFlag = "PATIENT")
        {
            var userType = this.context.UserType.FirstOrDefault(e => e.Flag == userTypeFlag);
            var personType = this.context.PersonEntityType.FirstOrDefault(e => e.Flag == personTypeFlag);

            if (userTypeFlag != null)
            {
                user.FkUserTypeId = userType.PkUserTypeId;
            }

            if (personType != null)
            {
                user.FkPerson.FkPersonEntityTypeId = personType.PkPersonEntityTypeId;
            }

            user.Enabled = true;

            this.context.Update<User>(user);
            this.context.SaveChanges();

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Ok
            });
        }

        [HttpPut("Password")]
        public IActionResult UpdateUserPassword([FromBody]User user)
        {
            var currentUser = this.context.User.FirstOrDefault(e => e.PkUserId == user.PkUserId);

            if (currentUser != null)
            {
                currentUser.Password = user.Password;

                this.context.SaveChanges();
            }

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Ok
            });
        }

        private (string jwtToken, string userToken) GenerateToken(string email)
        {
            var key = configuration["Authentication:jwt:key"];
            var protector = this.dataProtectionProvider.CreateProtector(key);
            var userToken = Guid.NewGuid().ToString();
            var userEncryptedToken = protector.Protect(userToken);

            var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, email),
                    new Claim(JwtRegisteredClaimNames.Jti, userEncryptedToken)
                };

            var token = new JwtSecurityToken
            (
                issuer: configuration["Authentication:jwt:issuer"],
                audience: configuration["Authentication:jwt:audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(60),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), userToken);
        }
    }
}