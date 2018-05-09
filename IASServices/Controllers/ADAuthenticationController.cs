using IASServices.Models;
using JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Newtonsoft.Json;
using System.Security.Principal;

namespace IASServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class ADAuthenticationController : Controller
    {
        private KontaktyContext _context;

        private IasSecurityContext securitycontext;

        bool czy_w_domu = ConfigurationManager.AppSettings.Get("czyProdukcyjny") == "true" ? false : true;
        string home_user = "CFYL";

        public ADAuthenticationController(KontaktyContext context, IasSecurityContext securitcx)
        {       

                _context = context;
            securitycontext = securitcx;
          
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
        [AllowAnonymous]
        public IActionResult JwtAuthenticate1([FromBody] ADUser user)
        {
           

            if (!czy_w_domu)
            {
                try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + "mf.gov.pl", user.Name, user.Password);
                object nativeObject = entry.NativeObject;
                return Ok( new { Token = CreateToken(user) }); 
            }
            catch (DirectoryServicesCOMException) { return Unauthorized(); }

            }
            return Ok(new { Token = CreateToken(new ADUser() { Name = user.Name==null? home_user: user.Name }) });
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
            //var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);        
            //var expiry = Math.Round((DateTime.UtcNow.AddMinutes(10) - unixEpoch).TotalSeconds);
            //var issuedAt = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
            //var notBefore = Math.Round((DateTime.UtcNow.AddMonths(6) - unixEpoch).TotalSeconds);

            var userData = GetUserData(user);
            var role = GetUserRole(userData);
            var securityrole = GetSecurityUserRole(userData);
            string[] scrole = null;         
            if (securityrole != null)
            {
                scrole = securityrole.Split(',');
                string[] tmp = new string[scrole.Length];
                int i = 0;
                foreach (string sc in scrole)
                {
                    tmp[i] = sc.Trim();
                    i++;
                }
                scrole = tmp;
            }

            //var payload = new Dictionary<string, object>
            //{
            //    {"name", user.Name},
            //    {"user",  user.Name},
            //    //{"userId", user.Id},
            //    {"role", role},
            //    {"userData", userData},
            //    //{"sub", user.Id},
            //    {"nbf", notBefore},
            //    {"iat", issuedAt},
            //    {"exp", expiry},
            //    {"securityrole", securityrole },      
            //};

          
            const string apikey = "VeryCompl!c@teSecretKey";
             
            //var token = JsonWebToken.Encode(payload, apikey, JwtHashAlgorithm.HS256);
            //return token;



            /////**********************

            List<Claim> claims = new List<Claim>()
                {
                  new Claim(JwtRegisteredClaimNames.Sub,  user.Name, ClaimValueTypes.String, "http://127.0.0.1:5000"),
                  new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String,  "http://127.0.0.1:5000"),                     
                  new Claim(ClaimTypes.Name, user.Name),     
                  //new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userData)),
                  new Claim("name", user.Name),
                  new Claim("user",  user.Name),
                  new Claim("userData", JsonConvert.SerializeObject(userData))
                };

            if (securityrole != null)
                foreach (string rola in scrole)
                claims.Add(new Claim(ClaimTypes.Role, rola, "http://127.0.0.1:5000"));
            else
                claims.Add(new Claim(ClaimTypes.Role, "", "http://127.0.0.1:5000"));


            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(apikey));
            var jwt = new JwtSecurityToken(
               issuer: "http://127.0.0.1:5000",
               audience: "http://127.0.0.1:5000",       
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(120),
               signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            ////var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            ////{
            ////    Formatting = Formatting.Indented
            ////});
            return encodedJwt;
            /////***********************


          
        }

        private string GetUserRole(Kontakty userData)
        {
            //if (!czy_w_domu)
            {
                if (userData.Wydzial is null || userData.Stanowisko is null) return "User";
                if (userData.Wydzial.Equals("Wydzia³ Informatyki"))
                    return "Admin";
                else if (userData.Stanowisko.Contains("kierownik"))
                    return "Supervisor";
            }
            return "User";
        }

        private string GetSecurityUserRole(Kontakty userData)
        {
            if (czy_w_domu)
                userData.Id = 1;

            return securitycontext.Role.FromSql("SELECT cast(1 as bigint) as id, Stuff((SELECT N', ' + rola from role r left join roleuzytkownika ru on r.id = ru.id_roli where ru.datakonca is null and ru.id_uzytkownika=" + userData.Id+ " FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,2,N'') as rola, null as modul,null as opis ").First().Rola;

            //return securitycontext.Role.FromSql("select id, rola from role r left join roleuzytkownika ru on r.id = ru.id_roli where ru.id_uzytkownika=" + userData.Id).AsEnumerable();

        }

        //private string GetUserRole(ADUser user)
        //{
        //    var role = _context.Kontakty.Where(k => k.Login.Equals(user.Name)).Select(k => k.Stanowisko.Contains("kierownik"));
        //    return "";
        //}

        private Kontakty GetUserData(ADUser user)
            => _context.Kontakty.SingleOrDefault(k => k.Login.Equals(user.Name));

        [HttpGet("{id}")]
        public async Task<IEnumerable<UserHistory>> GetUsersHistory([FromRoute] long id)
        {
       
            using (var cxt = securitycontext)
            using (var cmd = cxt.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = " select rola, datapoczatku, nadal, datakonca, odebral from ias.dbo.roleuzytkownika ru left join ias.dbo.role ro on ru.id_roli = ro.id where ru.id_uzytkownika="+id.ToString()+" order by rola, datapoczatku asc";

                cxt.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {
                    if (!result.HasRows)
                        return null;

                    List<UserHistory> rez = new List<UserHistory>();

                    while (result.Read())
                    {
                        DateTime datand;
                        DateTime dataod;
                  
                        rez.Add(new UserHistory()
                        {
                            rola = result.GetValue(0).ToString(),
                            datanadania = DateTime.TryParse(result.GetValue(1).ToString(), out datand)?datand.ToShortDateString():"",
                            nadal = result.GetValue(2).ToString(),
                            dataodebrania = DateTime.TryParse(result.GetValue(3).ToString(), out dataod) ? datand.ToShortDateString() : "",
                            odebral = result.GetValue(4).ToString()                           
                        });
                    }

                    return  rez;

                }

            }
        }


        [HttpGet]
        public async Task<IEnumerable<Role>> GetRole()
        {
            //var queryString = HttpContext.Request.Query;
            return await securitycontext.Role.ToListAsync();
        }


        [HttpGet]
        public async Task<IEnumerable<UserRole>> GetUsersNotInRole()
        {
            string id = Request.Query["id"];
            using (var cxt = _context)
            using (var cmd = cxt.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = " select id, coalesce(imie,'')+' '+coalesce(nazwisko,'') as nazwa, login, wydzial from ias.dbo.kontakty "
                    + "where id not in(select coalesce(id_uzytkownika, 0) from  roleuzytkownika where id_roli ="+id+" and datakonca is null) ";
               
                cxt.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {
                    if (!result.HasRows)
                        return null;

                    List<UserRole> rez = new List<UserRole>();

                    while (result.Read())
                    {
                        rez.Add(new UserRole() {
                            id = result.GetValue(0).ToString(),
                            nazwa = result.GetValue(1).ToString(),
                            login = result.GetValue(2).ToString(),
                            wydzial = result.GetValue(3).ToString()
                        });
                    }

                    return rez;

                }

            }
        }


        [HttpGet]
        public async Task<IEnumerable<UserRole>> GetUsersInRole()
        {
      
            string id = Request.Query["id"];
            using (var cxt = _context)
            using (var cmd = cxt.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = " select id, coalesce(imie,'')+' '+coalesce(nazwisko,'') as nazwa,login, wydzial from ias.dbo.kontakty "
                    + "where id in(select id_uzytkownika from  roleuzytkownika where id_roli =" + id + " and datakonca is null) ";

                cxt.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {
                    if (!result.HasRows)
                        return null;

                    List<UserRole> rez = new List<UserRole>();

                    while (result.Read())
                    {
                        rez.Add(new UserRole()
                        {
                            id = result.GetValue(0).ToString(),
                            nazwa = result.GetValue(1).ToString(),
                            login = result.GetValue(2).ToString(),
                            wydzial = result.GetValue(3).ToString()
                        });
                    }

                    return rez;

                }

            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUserRole([FromBody] Roleuzytkownika roleuzytkownika)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            roleuzytkownika.DataPoczatku = DateTime.Now;
            securitycontext.Roleuzytkownika.Add(roleuzytkownika);
            await securitycontext.SaveChangesAsync();

            //return CreatedAtAction("GetUsersInRole", new { id = roleuzytkownika.Id }, roleuzytkownika);
            var user = _context.Kontakty.Where(u => u.Id == roleuzytkownika.IdUzytkownika).Select(u=>u).FirstOrDefault();

            return Json(user);

        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserRole([FromBody] Roleuzytkownika roletoremove)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            securitycontext.Database.ExecuteSqlCommand("update roleuzytkownika set datakonca = {0},odebral = {3}  where id_roli={1} and id_uzytkownika={2}",DateTime.Now, roletoremove.IdRoli, roletoremove.IdUzytkownika, roletoremove.Odebral);

            //securitycontext.Roleuzytkownika.Remove(securitycontext.Roleuzytkownika.Where(a => a.IdRoli.Equals(roletoremove.IdRoli) && a.IdUzytkownika.Equals(roletoremove.IdUzytkownika)).Select(a => a).FirstOrDefault());
            await securitycontext.SaveChangesAsync();

            var user = _context.Kontakty.Where(u => u.Id == roletoremove.IdUzytkownika).Select(u => u).FirstOrDefault();
            return Json(user);

        }


        [HttpGet]
        public async Task<IEnumerable<UserRole>> GetAllUsers()
        {
    
            using (var cxt = _context)
            using (var cmd = cxt.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = " select id, coalesce(imie,'')+' '+coalesce(nazwisko,'') as nazwa,login, wydzial from ias.dbo.kontakty ";          

                cxt.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {
                    if (!result.HasRows)
                        return null;

                    List<UserRole> rez = new List<UserRole>();

                    while (result.Read())
                    {
                        rez.Add(new UserRole()
                        {
                            id = result.GetValue(0).ToString(),
                            nazwa = result.GetValue(1).ToString(),
                            login = result.GetValue(2).ToString(),
                            wydzial = result.GetValue(3).ToString()
                        });
                    }

                    return rez;

                }

            }
        }

        [HttpGet]
        public async Task<IEnumerable<Role>> GetRoleOffUser()
        {
            string id = Request.Query["id"];
      
            using (var cxt = _context)
            using (var cmd = cxt.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = " select * from role where id not in(select id_roli from roleuzytkownika where id_uzytkownika = "+id+" and datakonca is null) ";

                cxt.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {
                    if (!result.HasRows)
                        return null;

                    List<Role> rez = new List<Role>();

                    while (result.Read())
                    {
                        rez.Add(new Role()
                        {
                            Id = int.Parse(result.GetValue(0).ToString()),
                            Rola = result.GetValue(1).ToString(),
                  
                        });
                    }

                    return rez;

                }

            }
        }

        [HttpGet]
        public async Task<IEnumerable<Role>> GetUsersRoles()
        {
            string id = Request.Query["id"];

            using (var cxt = _context)
            using (var cmd = cxt.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = " select * from role where id in(select id_roli from roleuzytkownika where id_uzytkownika = " + id + " and datakonca is null) ";

                cxt.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {
                    if (!result.HasRows)
                        return null;

                    List<Role> rez = new List<Role>();

                    while (result.Read())
                    {
                        rez.Add(new Role()
                        {
                            Id = int.Parse(result.GetValue(0).ToString()),
                            Rola = result.GetValue(1).ToString(),

                        });
                    }

                    return rez;

                }

            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRoleToUser([FromBody] Roleuzytkownika roleuzytkownika)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            roleuzytkownika.DataPoczatku = DateTime.Now;
            securitycontext.Roleuzytkownika.Add(roleuzytkownika);
            await securitycontext.SaveChangesAsync();

            var user = securitycontext.Role.Where(u => u.Id == roleuzytkownika.IdRoli).Select(u=>u).FirstOrDefault();

            return Json(user);

        }

        [HttpPost]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] Roleuzytkownika roletoremove)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            securitycontext.Database.ExecuteSqlCommand("update roleuzytkownika set datakonca = {0},odebral = {3}  where id_roli={1} and id_uzytkownika={2}", DateTime.Now, roletoremove.IdRoli, roletoremove.IdUzytkownika, roletoremove.Odebral);

            //securitycontext.Roleuzytkownika.Remove(securitycontext.Roleuzytkownika.Where(a => a.IdRoli.Equals(roletoremove.IdRoli) && a.IdUzytkownika.Equals(roletoremove.IdUzytkownika)).Select(a => a).FirstOrDefault());
            await securitycontext.SaveChangesAsync();

            var user = securitycontext.Role.Where(u => u.Id == roletoremove.IdRoli).Select(u => u).FirstOrDefault();
            return Json(user);

        }

        #region funkcje tworzenia rol na bazie

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRola([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rola = await securitycontext.Role.SingleOrDefaultAsync(m => m.Id == id);

            if (rola == null)
            {
                return NotFound();
            }

            return Ok(rola);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] Role rola)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            securitycontext.Role.Add(rola);
            int newid = await securitycontext.SaveChangesAsync();

            return CreatedAtAction("GetRola", new { id = rola.Id }, rola);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole([FromRoute] long id, [FromBody] Role rola)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rola.Id)
            {
                return BadRequest();
            }

    
            securitycontext.Entry(rola).State = EntityState.Modified;

            try
            {
                await securitycontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool RolaExists(long id)
        => securitycontext.Role.Any(e => e.Id == id);

        [HttpPost("{id}")]
        public async Task<IActionResult> DelRole([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // _context.Database.SqlQuery("df");
            var rola = await securitycontext.Role.SingleOrDefaultAsync(m => m.Id == id);
            if (rola == null)
            {
                return NotFound();
            }


            securitycontext.Role.Remove(rola);
            await securitycontext.SaveChangesAsync();

            return Ok(rola);
        }

        #endregion


    }

    public class UserHistory
    {
        public string rola;
        public string datanadania;
        public string nadal;
        public string dataodebrania;
        public string odebral;
    }

    public class UserRole
    {
        public string id;
        public string nazwa;
        public string login;
        public string wydzial;
    }
}
