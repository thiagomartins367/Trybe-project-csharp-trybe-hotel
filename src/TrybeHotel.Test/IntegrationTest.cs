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

public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientTest;

    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory.WithWebHostBuilder(
            builder =>
            {
                builder.ConfigureServices(
                    services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>)
                        );
                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<ContextTest>(
                            options => options.UseInMemoryDatabase("InMemoryTestDatabase")
                        );
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
                            appContext.Cities.Add(new City { CityId = 1, Name = "Manaus" });
                            appContext.Cities.Add(new City { CityId = 2, Name = "Palmas" });
                            appContext.SaveChanges();
                            appContext.Hotels.Add(new Hotel { HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1 });
                            appContext.Hotels.Add(new Hotel { HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2 });
                            appContext.Hotels.Add(new Hotel { HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1 });
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
                        }
                    });
            }).CreateClient();
    }

    [Trait("Category", "Route tests `/city`")]
    [Theory(DisplayName = "Can get all cities")]
    [MemberData(nameof(DataTestGetCities))]
    public async Task TestGetCities(string url, IEnumerable<CityDto> expected)
    {
        var response = await _clientTest.GetAsync(url);
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<CityDto>>();

        System.Net.HttpStatusCode.OK.Should().Be(response?.StatusCode);
        content.Should().BeEquivalentTo(expected);
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
}
