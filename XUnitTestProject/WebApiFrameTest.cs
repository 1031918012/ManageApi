using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TodoApi;
using Xunit;

namespace XUnitTestProject
{
    public class WebApiFrameTest
    {

        private readonly TestServer _server;
        private readonly HttpClient _client;

        public WebApiFrameTest()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task GetAllTest()
        {
            var response = await _client.GetAsync("/api/users");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            IList<User> users = JsonConvert.DeserializeObject<IList<User>>(responseString);

            Assert.Equal(3, users.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetTest(int id)
        {
            var response = await _client.GetAsync($"/api/users/{id}");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            User user = JsonConvert.DeserializeObject<User>(responseString);

            Assert.NotNull(user);
        }

        public class User
        {
            string Name { get; set; }
            string UserID { get; set; }
        }
    }
}
