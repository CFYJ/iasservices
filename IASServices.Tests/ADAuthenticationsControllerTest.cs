using IASServices.Controllers;
using IASServices.Models;
using JWT;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace IASServices.Tests
{
    public class ADAuthenticationsControllerTest
    {
        [Fact]
        public void CreateTokenForUserWhoBelongsToWydzialInformatykiShouldReturnRoleAdmin()
        {
            //Arrange
            const string apikey = "VeryCompl!c@teSecretKey";
            var user = new ADUser() { Name = "CFYJ5", Password = null };

            var builder = new DbContextOptionsBuilder<KontaktyContext>().UseInMemoryDatabase();
            var context = new KontaktyContext(builder.Options);
            var kontakty = Enumerable.Range(1, 10).Select(i => new Kontakty
            {
                Id = i,
                Login = $"CFYJ{i}",
                Wydzial = "Wydział Informatyki",
                Stanowisko = "naczelnik (kierownik) wydziału"
            });
            context.Kontakty.AddRange(kontakty);
            context.SaveChanges();

            var controller = new ADAuthenticationController(context);

            //Act
            var token = controller.CreateToken(user);
            var decodedToken = JsonWebToken.DecodeToObject(token, apikey, false);
            var role = (decodedToken as IDictionary)["role"];

            //Assert
            role.Should().Be(10);
            Assert.Equal("Admin", role);

        }

    }
}
