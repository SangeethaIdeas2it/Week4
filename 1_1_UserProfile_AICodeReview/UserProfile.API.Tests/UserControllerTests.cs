using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserProfile.API.Data;
using UserProfile.Models;
using Xunit;

namespace UserProfile.API.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<UserDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<UserDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });
        }
    }

    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public UserControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_CreateUser_And_Get_User_Returns_Created_User()
        {
            // Arrange
            var client = _factory.CreateClient();
            var userDto = new UserDto { Name = "Integration Test User", Email = "integration@test.com" };

            // Act: Create User
            var createResponse = await client.PostAsJsonAsync("/api/users", userDto);
            createResponse.EnsureSuccessStatusCode();
            var createdUser = await createResponse.Content.ReadFromJsonAsync<User>();

            // Assert: Create User
            Assert.NotNull(createdUser);
            Assert.Equal(userDto.Name, createdUser.Name);
            Assert.Equal(userDto.Email, createdUser.Email);

            // Act: Get User
            var getResponse = await client.GetAsync($"/api/users/{createdUser.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetchedUser = await getResponse.Content.ReadFromJsonAsync<User>();

            // Assert: Get User
            Assert.NotNull(fetchedUser);
            Assert.Equal(createdUser.Id, fetchedUser.Id);
            Assert.Equal(createdUser.Name, fetchedUser.Name);
        }

        [Fact]
        public async Task Put_UpdateUser_Returns_NoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var userDto = new UserDto { Name = "Original User", Email = "original@test.com" };
            var createResponse = await client.PostAsJsonAsync("/api/users", userDto);
            createResponse.EnsureSuccessStatusCode();
            var createdUser = await createResponse.Content.ReadFromJsonAsync<User>();

            var updatedUserDto = new UserDto { Id = createdUser.Id, Name = "Updated User", Email = "updated@test.com" };

            // Act
            var updateResponse = await client.PutAsJsonAsync($"/api/users/{createdUser.Id}", updatedUserDto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getResponse = await client.GetAsync($"/api/users/{createdUser.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetchedUser = await getResponse.Content.ReadFromJsonAsync<User>();
            Assert.NotNull(fetchedUser);
            Assert.Equal(updatedUserDto.Name, fetchedUser.Name);
            Assert.Equal(updatedUserDto.Email, fetchedUser.Email);
        }

        [Fact]
        public async Task Delete_DeleteUser_Returns_NoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var userDto = new UserDto { Name = "To Be Deleted", Email = "delete@test.com" };
            var createResponse = await client.PostAsJsonAsync("/api/users", userDto);
            createResponse.EnsureSuccessStatusCode();
            var createdUser = await createResponse.Content.ReadFromJsonAsync<User>();

            // Act
            var deleteResponse = await client.DeleteAsync($"/api/users/{createdUser.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await client.GetAsync($"/api/users/{createdUser.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
} 