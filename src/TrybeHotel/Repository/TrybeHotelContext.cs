using Microsoft.EntityFrameworkCore;
using TrybeHotel.Models;

namespace TrybeHotel.Repository;
public class TrybeHotelContext : DbContext, ITrybeHotelContext
{
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<Hotel> Hotels { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;

    public TrybeHotelContext(DbContextOptions<TrybeHotelContext> options) : base(options) { }
    public TrybeHotelContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = GetConnectionString();
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }

    private static string GetConnectionString()
    {
        var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var connectionStringFirstPart = $"Server={dbServer};Database={dbName}";
        var connectionStringSecondPart = $"User={dbUser};Password={dbPassword}";
        return $"{connectionStringFirstPart};{connectionStringSecondPart};TrustServerCertificate=True";
    }
}
