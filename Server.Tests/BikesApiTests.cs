using bike_matrix_tech_int.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Server.Tests
{
    public class ProductApiTests : IClassFixture<ProductApiTests.CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        // Create custom factory for http client to handle SQLite DB in memory
        public class CustomWebApplicationFactory : WebApplicationFactory<Program>
        {
            private SqliteConnection _connection;

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureServices(services =>
                {
                    // Remove old context registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Create and keep the same open SQLite in-memory connection
                    _connection = new SqliteConnection("DataSource=:memory:");
                    _connection.Open();

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseSqlite(_connection);
                    });

                    // Build service provider and ensure schema exists
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated(); 
                });
            }

            // Properly dispose of the connection to in memory SQLite after tests run
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    _connection?.Close();
                    _connection?.Dispose();
                }
            }
        }

        // Use custom http client factory for SQLite DB
        public ProductApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        // Mainline POST, PUT, GET ALL, GET, DELETE tests
        [Fact]
        public async Task BikesEndpointMainline()
        {
            // === 1. POST ===
            var newBike = new Bike
            {
                Email = "test@example.com",
                Brand = "Trek",
                Model = "Boone",
                Year = "2024"
            };

            var postResponse = await _client.PostAsJsonAsync("/api/bikes", newBike);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<Bike>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0);

            var postBody = await postResponse.Content.ReadAsStringAsync();
            Assert.Equal(created.Email, newBike.Email);
            Assert.Equal(created.Brand, newBike.Brand);
            Assert.Equal(created.Model, newBike.Model);
            Assert.Equal(created.Year, newBike.Year);

            // === 2. GET ALL ===
            var getAllResponse = await _client.GetAsync("/api/bikes");
            var getAllBody = await getAllResponse.Content.ReadAsStringAsync();
            getAllResponse.EnsureSuccessStatusCode();
            var allBikes = await getAllResponse.Content.ReadFromJsonAsync<List<Bike>>();
            Assert.NotNull(allBikes);
            var found = allBikes.FirstOrDefault(b => b.Id == created.Id);
            Assert.NotNull(found);

            Assert.Contains(allBikes, b =>
                b.Id == created.Id &&
                b.Brand == newBike.Brand &&
                b.Model == newBike.Model &&
                b.Year == newBike.Year &&
                b.Email == newBike.Email
             );

            // === 3. PUT ===
            var updatedBike = new Bike
            {
                Id = created.Id,
                Email = created.Email,
                Brand = created.Brand,
                Model = created.Model,
                Year = "2020" // Updating year
            };

            var putResponse = await _client.PutAsJsonAsync($"/api/bikes/{created.Id}", updatedBike);
            var putBody = await putResponse.Content.ReadAsStringAsync();
            putResponse.EnsureSuccessStatusCode();

            // === 4. GET BY ID ===
            var getByIdResponse = await _client.GetAsync($"/api/bikes/{created.Id}");
            var getByIdBody = await getByIdResponse.Content.ReadAsStringAsync();
            getByIdResponse.EnsureSuccessStatusCode();

            var fetched = await getByIdResponse.Content.ReadFromJsonAsync<Bike>();
            Assert.NotNull(fetched);
            Assert.Equal(updatedBike.Id, fetched.Id);
            Assert.Equal(updatedBike.Email, fetched.Email);
            Assert.Equal(updatedBike.Brand, fetched.Brand);
            Assert.Equal(updatedBike.Model, fetched.Model);
            Assert.Equal(updatedBike.Year, fetched.Year);

            // === 5. DELETE ===
            var deleteResponse = await _client.DeleteAsync($"/api/bikes/{created.Id}");
            var deleteBody = await deleteResponse.Content.ReadAsStringAsync();
            deleteResponse.EnsureSuccessStatusCode();

            // === 6. GET ID AFTER DELETE (expect 404) ===
            var getAfterDelete = await _client.GetAsync($"/api/bikes/{created.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getAfterDelete.StatusCode);
        }

        // Check invalid email format
        [Fact]
        public async Task PostBikes_InvalidEmail()
        {
            var bike = new Bike
            {
                Email = "not-an-email",
                Brand = "Trek",
                Model = "Boone",
                Year = "2024"
            };

            var response = await _client.PostAsJsonAsync("/api/bikes", bike);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(errorResponse);
            Assert.True(errorResponse.Errors.ContainsKey("Email"));
        }

        // Check invalid brand name
        [Fact]
        public async Task PostBikes_InvalidBrand()
        {
            var bike = new Bike
            {
                Email = "test@example.com",
                Brand = "InvalidBrand",
                Model = "Boone",
                Year = "2024"
            };

            var response = await _client.PostAsJsonAsync("/api/bikes", bike);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(errorResponse);
            Assert.True(errorResponse.Errors.ContainsKey("Brand"));
        }

        // Check invalid model name
        [Fact]
        public async Task PostBikes_InvalidModel()
        {
            var bike = new Bike
            {
                Email = "test@example.com",
                Brand = "Trek",
                Model = "NotARealModel",
                Year = "2024"
            };

            var response = await _client.PostAsJsonAsync("/api/bikes", bike);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(errorResponse);
            Assert.True(errorResponse.Errors.ContainsKey("Model"));
        }

        // Check invalid year
        [Fact]
        public async Task PostBikes_InvalidYear()
        {
            var bike = new Bike
            {
                Email = "test@example.com",
                Brand = "Trek",
                Model = "Boone",
                Year = "2021" // invalid, only 2020 or 2024 allowed
            };

            var response = await _client.PostAsJsonAsync("/api/bikes", bike);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(errorResponse);
            Assert.True(errorResponse.Errors.ContainsKey("Year"));
        }

        // Check invalid endpoint used
        [Fact]
        public async Task GetInvalidEndpoint()
        {
            var response = await _client.GetAsync("/api/nonexistent-endpoint");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
