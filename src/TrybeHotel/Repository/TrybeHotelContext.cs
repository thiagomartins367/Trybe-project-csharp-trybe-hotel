using Microsoft.EntityFrameworkCore;
using TrybeHotel.Enums;
using TrybeHotel.Env;
using TrybeHotel.Models;

namespace TrybeHotel.Repository;
public class TrybeHotelContext : DbContext, ITrybeHotelContext
{
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<Hotel> Hotels { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    private DatabaseDialect DbDialect { get; set; } = DatabaseDialect.SQL_SERVER;
    private string DbServer { get; set; } = Environment
        .GetEnvironmentVariable(EnvironmentVariables.DB_SERVER)!;
    private string DbPort { get; set; } = Environment
        .GetEnvironmentVariable(EnvironmentVariables.DB_PORT)!;
    private string DbName { get; set; } = Environment
        .GetEnvironmentVariable(EnvironmentVariables.DB_NAME)!;
    private string DbUser { get; set; } = Environment
        .GetEnvironmentVariable(EnvironmentVariables.DB_USER)!;
    private string DbPassword { get; set; } = Environment
        .GetEnvironmentVariable(EnvironmentVariables.DB_PASSWORD)!;

    public TrybeHotelContext(DbContextOptions<TrybeHotelContext> options) : base(options)
    {
        SetDatabaseDialect();
    }
    public TrybeHotelContext()
    {
        SetDatabaseDialect();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = GetConnectionString();
        switch (DbDialect)
        {
            case DatabaseDialect.MYSQL:
                optionsBuilder.UseMySql(
                    connectionString, ServerVersion.AutoDetect(connectionString), null);
                break;
            default:
                optionsBuilder.UseSqlServer(connectionString);
                break;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }

    private string GetConnectionString()
    {
        switch (DbDialect)
        {
            case DatabaseDialect.MYSQL:
                var connectionStringMysql = $"Server={DbServer};Database={DbName}";
                connectionStringMysql += string.Concat(";", $"User Id={DbUser};Password={DbPassword}");
                connectionStringMysql += string.Concat(";", $"Port={DbPort}");
                return connectionStringMysql;
            default:
                var connectionStringSqlServer = $"Server={DbServer},{DbPort};Database={DbName}";
                connectionStringSqlServer += string.Concat(";", $"User={DbUser};Password={DbPassword}");
                connectionStringSqlServer += string.Concat(";", "TrustServerCertificate=True");
                return connectionStringSqlServer;
        }
    }

    private void SetDatabaseDialect()
    {
        var envDbDialect = Environment.GetEnvironmentVariable(EnvironmentVariables.DB_DIALECT);
        if (envDbDialect is null) return;
        var envDbDialectIsInt = int.TryParse(envDbDialect, out int dbDialectValue);
        if (!envDbDialectIsInt && !Enum.IsDefined(typeof(DatabaseDialect), dbDialectValue)) return;
        DbDialect = (DatabaseDialect)dbDialectValue;
    }
}
