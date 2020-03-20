using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Helpers;
using Library.Models.DTO;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class UsersInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;


        public UsersInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.User.Add(new User
                {
                    IdUser = 1,
                    Email = "jd@pja.edu.pl",
                    Name = "Daniel",
                    Surname = "Jabłoński",
                    Login = "jd",
                    Password = "ASNDKWQOJRJOP!JO@JOP"
                });

                _db.SaveChanges();
            }
        }


        [Fact]
        public async Task GetUsers_200Ok()
        {
            //Arrange i Act
            var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(content);

            Assert.True(users.Count() == 1);
            Assert.True(users.ElementAt(0).Login == "jd");
        }

        [Fact]
        public async Task GetUser_200Ok()
        {
            var userId = 1;
            var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users/{userId}");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);

            Assert.True(user.IdUser == userId);
            Assert.True(user.Login == "jd");
        }

        [Fact]
        public async Task PostUser_201Created()
        {
            var newUser = new UserDto
            {
                Name = "Marcin",
                Surname = "S",
                Login = "msekrecki",
                Email = "s17960@pjwstk.edu.pl",
                Password = "hardpassword"
            };

            string payload = JsonConvert.SerializeObject(newUser);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/users", content);

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var addedUser = JsonConvert.DeserializeObject<User>(responseContent);

            Assert.True(newUser.Name == addedUser.Name);
            Assert.True(addedUser.IdUserRoleDict == (int)UserRoleHelper.UserRolesEnum.Reader);
        }
    }
}
