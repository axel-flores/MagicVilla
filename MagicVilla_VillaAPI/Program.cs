

using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddAutoMapper(typeof(MappingConfig));

//para obtener una peticion http aceptable
builder.Services.AddControllers(
    //option => { option.ReturnHttpNotAcceptable = true; }
    ).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();//para agregar soporte para devolver xml
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILogging, Logging>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
