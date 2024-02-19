using System.Text;
using TrybeHotel.Repository;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TrybeHotel.Env;
using TrybeHotel.Services;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TrybeHotel.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
var rootPath = Directory.GetCurrentDirectory();
string? dotenvPath = null;
const string developmentDotenvFileName = ".env.development.local";
const string productionDotenvFileName = ".env.production.local";
if (builder.Environment.IsDevelopment())
    dotenvPath = Path.Combine(rootPath, developmentDotenvFileName);
else
{
    dotenvPath = Path.Combine(rootPath, productionDotenvFileName);
}
EnvConfig.Load(dotenvPath);

// Configure port
var port = Environment.GetEnvironmentVariable(EnvironmentVariables.PORT);
builder.WebHost.UseUrls($"http://*:{port}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<TrybeHotelContext>();
builder.Services.AddScoped<ITrybeHotelContext, TrybeHotelContext>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddHttpClient<IGeoService, GeoService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string softwareLicenseUrl = Environment.GetEnvironmentVariable("SOFTWARE_LICENSE_URL")!;
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TrybeHotel",
        Description = "<div>Projeto de desenvolvimento de uma <b>API de booking</b> de várias redes de hotéis com recurso de <b>autenticação e autorização por token</b>, reserva de quartos e CRUD de hotéis, cidades, quartos e usuários.\nAlém disso conta com um recurso especial de <b>geolocalização</b> sendo possível obter os hotéis mais próximos de um determinado endereço através do consumo de dados da API <b><a href=\"https://nominatim.org\" target=\"_blank\">nominatim</a></b>.</div>",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Thiago Martins",
            Email = "thiago17thiago@gmail.com",
            Url = new Uri("https://thiagomartins367.github.io"),
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri(softwareLicenseUrl),
        }
    });
});
builder.Services.AddSwaggerGen(options =>
{
    string xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Insira o token de autenticação do usuário da seguinte forma: `Bearer <token>`",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
        });

    options.OperationFilter<AuthResponsesOperationFilter>();
});


builder.Services.AddControllersWithViews()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddHttpClient();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                "https://nominatim.openstreetmap.org",
                "https://openstreetmap.org"
            );
        });
});

TokenOptions tokenOptions = new();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenOptions.Secret)),
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Client", policy => policy.RequireClaim(ClaimTypes.Email));
    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Email).RequireClaim(ClaimTypes.Role, "admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// app.UseCors(MyAllowSpecificOrigins);
app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
