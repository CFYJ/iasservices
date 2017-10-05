using IASServices.Models;
using JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace IASServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class ADAuthenticationController : Controller
    {
        private KontaktyContext _context;

        public ADAuthenticationController(KontaktyContext context)
        {
            _context = context;
        }

        

        // POST: api/ADAuthentication
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ADAuthentication/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPut]
        public bool IsAuthenticated([FromBody] ADUser user)
        {
            bool result = false;
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + "mf.gov.pl", user.Name, user.Password);
                object nativeObject = entry.NativeObject;
                result = true;
            }
            catch (DirectoryServicesCOMException) { }
            return result;
        }

        [HttpPut]
        public IActionResult JwtAuthenticate1([FromBody] ADUser user)
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + "mf.gov.pl", user.Name, user.Password);
                object nativeObject = entry.NativeObject;
                return Ok( new { Token = CreateToken(user) }); 
            }
            catch (DirectoryServicesCOMException) { return Unauthorized(); }
            
        }



        [HttpPut]
        public string JwtAuthenticate([FromBody] ADUser user)
        {
            string result = null;
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + "mf.gov.pl", user.Name, user.Password);
                object nativeObject = entry.NativeObject;
                result = CreateToken(user);
            }
            catch (DirectoryServicesCOMException) { }
            return result;
        }

        /// <summary>
        /// Create a Jwt with user information
        /// </summary>
        /// <param name="user"></param>        
        /// <returns></returns>
        public string CreateToken(ADUser user)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //var expiry = Math.Round((DateTime.UtcNow.AddHours(2) - unixEpoch).TotalSeconds);
            var expiry = Math.Round((DateTime.UtcNow.AddMinutes(10) - unixEpoch).TotalSeconds);
            var issuedAt = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
            var notBefore = Math.Round((DateTime.UtcNow.AddMonths(6) - unixEpoch).TotalSeconds);
            var userData = GetUserData(user);
            var role = GetUserRole(userData);

            var payload = new Dictionary<string, object>
            {
                {"name", user.Name},
                //{"userId", user.Id},
                {"role", role},
                {"userData", userData},
                //{"sub", user.Id},
                {"nbf", notBefore},
                {"iat", issuedAt},
                {"exp", expiry}
            };

            //var secret = ConfigurationManager.AppSettings.Get("jwtKey");
            const string apikey = "VeryCompl!c@teSecretKey";

            var token = JsonWebToken.Encode(payload, apikey, JwtHashAlgorithm.HS256);
            //var token = "";
            return token;
        }

        private string GetUserRole(Kontakty userData)
        {
            if (userData.Wydzial is null || userData.Stanowisko is null) return "User";
            if (userData.Wydzial.Equals("Wydzia³ Informatyki"))
                return "Admin";
            else if (userData.Stanowisko.Contains("kierownik"))
                return "Supervisor";            

            return "User";
        }

        //private string GetUserRole(ADUser user)
        //{
        //    var role = _context.Kontakty.Where(k => k.Login.Equals(user.Name)).Select(k => k.Stanowisko.Contains("kierownik"));
        //    return "";
        //}

        private Kontakty GetUserData(ADUser user)
            => _context.Kontakty.SingleOrDefault(k => k.Login.Equals(user.Name));
    }
}
