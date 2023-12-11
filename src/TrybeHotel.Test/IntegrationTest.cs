namespace TrybeHotel.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Dto;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Diagnostics;
using System.Xml;
using System.IO;
using FluentAssertions;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Collections;

public class IntegrationTest: IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientTest;

    private Hashtable? TokenByUserType { get; set; } = null;

    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDatabase");
                });
                services.AddScoped<ITrybeHotelContext, ContextTest>();
                services.AddScoped<ICityRepository, CityRepository>();
                services.AddScoped<IHotelRepository, HotelRepository>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();
                    appContext.Cities.Add(new City {CityId = 1, Name = "Manaus"});
                    appContext.Cities.Add(new City {CityId = 2, Name = "Palmas"});
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

    private async Task<bool> SignInForDefaultUser()
    {
        if (TokenByUserType is not null) return true;
        TokenByUserType = new Hashtable();
        foreach (var item in Enum.GetValues(typeof(UserType)))
        {
            LoginDto requestBody = new();
            switch (item)
            {
                case UserType.Admin:
                    requestBody = new LoginDto() { Email = "ana@trybehotel.com", Password = "Senha1" };
                    break;
                case UserType.Client:
                    requestBody = new LoginDto() { Email = "laura@trybehotel.com", Password = "Senha3" };
                    break;
                default:
                    break;
            }
            var response = await _clientTest.PostAsJsonAsync("/login", requestBody);
            var resContent = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
            TokenByUserType[item] = resContent!.Token;
        }
        return true;
    }

    private void SetAuthorizationTokenOnClient(string? token)
    {
        _clientTest.DefaultRequestHeaders
                .Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Trait("Category", "Route tests `/login`")]
    [Theory(DisplayName = "Can sign in")]
    [MemberData(nameof(DataTestPostLogin))]
    public async Task TestPostLogin(string url, LoginDto loginEntry)
    {
        var response = await _clientTest.PostAsJsonAsync(url, loginEntry);
        var resContent = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();

        System.Net.HttpStatusCode.OK.Should().Be(response?.StatusCode);
        resContent.Should().BeOfType<AccessTokenResponse>();
        resContent?.Token.Split('.').Length.Should().Be(3);
    }

    public static TheoryData<string, LoginDto> DataTestPostLogin => new()
    {
        {
            "/login",
            new LoginDto()
            {
                Email = "ana@trybehotel.com",
                Password = "Senha1",
            }
        }
    };

    [Trait("Category", "Route tests `/user`")]
    [Theory(DisplayName = "Can get all users")]
    [MemberData(nameof(DataTestGetUsers))]
    public async Task TestGetUsers(string url, IEnumerable<UserDto> expected)
    {
        await SignInForDefaultUser();
        var AuthorizationToken = TokenByUserType?[UserType.Admin]?.ToString();
        SetAuthorizationTokenOnClient(AuthorizationToken);

        var response = await _clientTest.GetAsync(url);
        var resContent = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();

        System.Net.HttpStatusCode.OK.Should().Be(response?.StatusCode);
        resContent.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<string, IEnumerable<UserDto>> DataTestGetUsers => new()
    {
        {
            "/user",
            new List<UserDto>()
            {
                new() { UserId = 1, Name = "Ana", Email = "ana@trybehotel.com", UserType = "admin" },
                new() { UserId = 2, Name = "Beatriz", Email = "beatriz@trybehotel.com", UserType = "client" },
                new() { UserId = 3, Name = "Laura", Email = "laura@trybehotel.com", UserType = "client" },
            }
        }
    };

    [Trait("Category", "Route tests `/city`")]
    [Theory(DisplayName = "Can get all cities")]
    [MemberData(nameof(DataTestGetCities))]
    public async Task TestGetCities(string url, IEnumerable<CityDto> expected)
    {
        var response = await _clientTest.GetAsync(url);
        var resContent = await response.Content.ReadFromJsonAsync<IEnumerable<CityDto>>();

        System.Net.HttpStatusCode.OK.Should().Be(response?.StatusCode);
        resContent.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<string, IEnumerable<CityDto>> DataTestGetCities => new()
    {
        {
            "/city",
            new List<CityDto>()
            {
                new() { CityId = 1, Name = "Manaus" },
                new() { CityId = 2, Name = "Palmas" },
            }
        }
    };

    [Trait("Category", "Route tests `/city`")]
    [Theory(DisplayName = "Can add a city")]
    [MemberData(nameof(DataTestPostCity))]
    public async Task TestPostCity(string url, City cityEntry, CityDto expected)
    {
        var response = await _clientTest.PostAsJsonAsync(url, cityEntry);
        var resContent = await response.Content.ReadFromJsonAsync<CityDto>();

        System.Net.HttpStatusCode.Created.Should().Be(response?.StatusCode);
        resContent.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<string, City, CityDto> DataTestPostCity => new()
    {
        {
            "/city",
            new City() { Name = "Guarulhos" },
            new CityDto() { CityId = 3, Name = "Guarulhos" }
        }
    };

    [Trait("Category", "Route tests `/hotel`")]
    [Theory(DisplayName = "Can get all hotels")]
    [MemberData(nameof(DataTestGetHotels))]
    public async Task TestGetHotels(string url, IEnumerable<HotelDto> expected)
    {
        var response = await _clientTest.GetAsync(url);
        var resContent = await response.Content.ReadFromJsonAsync<IEnumerable<HotelDto>>();

        System.Net.HttpStatusCode.OK.Should().Be(response?.StatusCode);
        resContent.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<string, IEnumerable<HotelDto>> DataTestGetHotels => new()
    {
        {
            "/hotel",
            new List<HotelDto>()
            {
                new() { HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1, CityName = "Manaus" },
                new() { HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2, CityName = "Palmas" },
                new() { HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1, CityName = "Manaus" },
            }
        }
    };

    [Trait("Category", "Route tests `/hotel`")]
    [Theory(DisplayName = "Can add a hotel")]
    [MemberData(nameof(DataTestPostHotel))]
    public async Task TestPostHotel(string url, Hotel hotelEntry, HotelDto expected)
    {
        await SignInForDefaultUser();
        var AuthorizationToken = TokenByUserType?[UserType.Admin]?.ToString();
        SetAuthorizationTokenOnClient(AuthorizationToken);

        var response = await _clientTest.PostAsJsonAsync(url, hotelEntry);
        var resContent = await response.Content.ReadFromJsonAsync<HotelDto>();

        System.Net.HttpStatusCode.Created.Should().Be(response?.StatusCode);
        resContent.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<string, Hotel, HotelDto> DataTestPostHotel => new()
    {
        {
            "/hotel",
            new Hotel() { Name = "Trybe Hotel AM", Address = "Avenida Atlântica, 1400", CityId = 1 },
            new HotelDto() { HotelId = 4, Name = "Trybe Hotel AM", Address = "Avenida Atlântica, 1400", CityId = 1, CityName = "Manaus" }
        }
    };

    [Trait("Category", "Route tests `/room`")]
    [Theory(DisplayName = "Can get all rooms from a hotel")]
    [MemberData(nameof(DataTestGetRoomsByHotelId))]
    public async Task TestGetRoomsByHotelId(string url, IEnumerable<RoomDto> expected)
    {
        var response = await _clientTest.GetAsync(url);
        var resContent = await response.Content.ReadFromJsonAsync<IEnumerable<RoomDto>>();

        System.Net.HttpStatusCode.OK.Should().Be(response?.StatusCode);
        resContent.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<string, IEnumerable<RoomDto>> DataTestGetRoomsByHotelId => new()
    {
        {
            "/room/1",
            new List<RoomDto>()
            {
                new() { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", Hotel = new HotelDto() { HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1, CityName = "Manaus" } },
                new() { RoomId = 2, Name = "Room 2", Capacity = 3, Image = "Image 2", Hotel = new HotelDto() { HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1, CityName = "Manaus" } },
                new() { RoomId = 3, Name = "Room 3", Capacity = 4, Image = "Image 3", Hotel = new HotelDto() { HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1, CityName = "Manaus" } },
            }
        }
    };

    [Trait("Category", "Route tests `/room`")]
    [Theory(DisplayName = "Can add a room")]
    [MemberData(nameof(DataTestPostRoom))]
    public async Task TestPostRoom(string url, Room roomEntry, RoomDto expected)
    {
        await SignInForDefaultUser();
        var AuthorizationToken = TokenByUserType?[UserType.Admin]?.ToString();
        SetAuthorizationTokenOnClient(AuthorizationToken);

        var response = await _clientTest.PostAsJsonAsync(url, roomEntry);
        var resContent = await response.Content.ReadFromJsonAsync<RoomDto>();

        System.Net.HttpStatusCode.Created.Should().Be(response?.StatusCode);
        resContent.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<string, Room, RoomDto> DataTestPostRoom => new()
    {
        {
            "/room",
            new Room() { Name = "Room 10", Capacity = 4, Image = "Image 10", HotelId = 3 },
            new RoomDto() { RoomId = 10, Name = "Room 10", Capacity = 4, Image = "Image 10", Hotel = new HotelDto() { HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1, CityName = "Manaus" } }
        }
    };

    [Trait("Category", "Route tests `/room`")]
    [Theory(DisplayName = "Can delete a room")]
    [InlineData("/room/1")]
    public async Task TestDeleteRoomById(string url)
    {
        await SignInForDefaultUser();
        var AuthorizationToken = TokenByUserType?[UserType.Admin]?.ToString();
        SetAuthorizationTokenOnClient(AuthorizationToken);

        var response = await _clientTest.DeleteAsync(url);
        var resContent = await response.Content.ReadAsStringAsync();

        System.Net.HttpStatusCode.NoContent.Should().Be(response?.StatusCode);
        resContent.Length.Should().Be(0);
    }
}
