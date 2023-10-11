namespace trybe_hotel.Test.Test;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class UserPostJson {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? UserType { get; set; }
}

public class ErrorJson {
    public string? message { get; set; }
}

public class TestReq02 : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientUserPost;

    public TestReq02(WebApplicationFactory<Program> factory)
    {

        _clientUserPost = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestPostUser");
                });
                services.AddScoped<ITrybeHotelContext, ContextTest>();
                services.AddScoped<ICityRepository, CityRepository>();
                services.AddScoped<IHotelRepository, HotelRepository>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IBookingRepository, BookingRepository>();

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();
                    appContext.Cities.Add(new City {Name = "Manaus"});
                    appContext.Cities.Add(new City {Name = "Palmas"});
                    appContext.SaveChanges();
                    appContext.Hotels.Add(new Hotel {HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1});
                    appContext.Hotels.Add(new Hotel {HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2});
                    appContext.Hotels.Add(new Hotel {HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1});
                    appContext.SaveChanges();
                    appContext.Rooms.Add(new Room { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 2, Name = "Room 2", Capacity = 3, Image = "Image 2", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 3, Name = "Room 3", Capacity = 4, Image = "Image 3", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 4, Name = "Room 4", Capacity = 2, Image = "Image 4", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 5, Name = "Room 5", Capacity = 3, Image = "Image 5", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 6, Name = "Room 6", Capacity = 4, Image = "Image 6", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 7, Name = "Room 7", Capacity = 2, Image = "Image 7", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 8, Name = "Room 8", Capacity = 3, Image = "Image 8", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 9, Name = "Room 9", Capacity = 4, Image = "Image 9", HotelId = 3 });
                    appContext.SaveChanges();
                    appContext.Users.Add(new User { UserId = 1, Name = "Ana", Email = "ana@trybehotel.com", Password = "Senha1", UserType = "admin" });
                    appContext.Users.Add(new User { UserId = 2, Name = "Beatriz", Email = "beatriz@trybehotel.com", Password = "Senha2", UserType = "client" });
                    appContext.Users.Add(new User { UserId = 3, Name = "Laura", Email = "laura@trybehotel.com", Password = "Senha3", UserType = "client" });
                    appContext.SaveChanges();
                    appContext.Bookings.Add(new Booking { BookingId = 1, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 2, RoomId = 1});
                    appContext.Bookings.Add(new Booking { BookingId = 2, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 3, RoomId = 4});
                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }
   
    [Trait("Category", "2. Desenvolva o endpoint POST /user")]
    [Theory(DisplayName = "Será validado que a resposta será um status http 201")]
    [InlineData("/user")]
    public async Task TestUserControllerPost(string url)
    {
        var inputObj = new {
            Name = "Maria",
            Email = "maria@trybehotel.com",
            Password = "123456"
        };
        var response = await _clientUserPost.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Trait("Category", "2. Desenvolva o endpoint POST /user")]
    [Theory(DisplayName = "Será validado que é possível retornar a pessoa cliente criada")]
    [InlineData("/user")]
    public async Task TestCityControllerPostResponse(string url)
    {
       var inputObj = new {
            Name = "Maria",
            Email = "mariab@trybehotel.com",
            Password = "123456"
        };
        var response = await _clientUserPost.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        UserPostJson jsonResponse = JsonConvert.DeserializeObject<UserPostJson>(responseString);
        Assert.Equal(4, jsonResponse.UserId);
        Assert.Equal("Maria", jsonResponse.Name);
        Assert.Equal("mariab@trybehotel.com", jsonResponse.Email);
        Assert.Equal("client", jsonResponse.UserType);
    }


    [Trait("Category", "2. Desenvolva o endpoint POST /user")]
    [Theory(DisplayName = "Será validado que não é possível cadastrar um e-mail já cadastrado")]
    [InlineData("/user")]
    public async Task TestCityControllerPostResponseConflict(string url)
    {
       var inputObj = new {
            Name = "Ana",
            Email = "ana@trybehotel.com",
            Password = "123456"
        };
        var response = await _clientUserPost.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(System.Net.HttpStatusCode.Conflict, response?.StatusCode);

        ErrorJson jsonResponse = JsonConvert.DeserializeObject<ErrorJson>(responseString);
        Assert.Equal("User email already exists", jsonResponse.message);
        
    }
    
}