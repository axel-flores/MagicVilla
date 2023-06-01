

using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/*  para añadir serilog:
 *  instalar serilog.aspnetcore y serilog.sinks.file
 */
//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//    .WriteTo.File("log/villalogs.txt", rollingInterval:RollingInterval.Day).CreateLogger() ;
//builder.Host.UseSerilog();

builder.Services.AddDbContext<MagicVillaDbContext>(option => {
    //option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSqlConnection"));
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
    });

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddAutoMapper(typeof(MappingConfig));

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(u =>
{
    u.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    u.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer( j =>
{
    j.RequireHttpsMetadata = false;
    j.SaveToken = true;
    j.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = true,
    };
});

//para obtener una peticion http aceptable
builder.Services.AddControllers(
    //option => { option.ReturnHttpNotAcceptable = true; }
    ).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();//para agregar soporte para devolver xml
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = $"JWT Authorization header using the bearer scheme \r\n\r\n" +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example: \"Bearer 132456a\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"

    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
}
    
);
builder.Services.AddSingleton<ILogging, Logging>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
