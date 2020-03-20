using Library.Entities;
using Library.Models.DTO;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class BookBorrowInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public BookBorrowInitTests()
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

                _db.Book.Add(new Book
                {
                    IdBook = 1,
                    Title = "Pierwsza ksiazka"
                });
                _db.Book.Add(new Book
                {
                    IdBook = 2,
                    Title = "Druga ksiazka"
                });

                _db.BookBorrow.Add(new BookBorrow
                {
                    IdBook = 1,
                    IdUser = 1
                });

                _db.SaveChanges();
            }
        }

        [Fact]
        public async Task PostBookBorrow_201Created()
        {
            var newBookBorrow = new BookBorrowDto
            {
                IdBook = 2,
                IdUser = 1,
                Comment = "Wypozyczenie ksiazeczki"
            };

            string payload = JsonConvert.SerializeObject(newBookBorrow);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows", content);

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var addedBookBorrow = JsonConvert.DeserializeObject<BookBorrow>(responseContent);

            Assert.True(newBookBorrow.IdBook == addedBookBorrow.IdBook);
            Assert.True(addedBookBorrow.IdUser == 1);
            Assert.True(addedBookBorrow.IdBookBorrow == 2);
        }

        [Fact]
        public async Task PutBookBorrow_200Ok()
        {
            var toChangeBookBorrow = new UpdateBookBorrowDto
            {
                IdBookBorrow = 1,
                IdBook = 1,
                IdUser = 1,
                DateFrom = DateTime.Parse("10.10.2019"),
                DateTo = DateTime.Now
            };

            string payload = JsonConvert.SerializeObject(toChangeBookBorrow);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PutAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/{toChangeBookBorrow.IdBookBorrow}", content);

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<bool>(responseContent);

            Assert.True(response == true);
        }
    }
}
