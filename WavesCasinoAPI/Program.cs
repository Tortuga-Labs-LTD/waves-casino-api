using Microsoft.EntityFrameworkCore;
using System.Net;
using WavesCasinoAPI.Clients;
using WavesCasinoAPI.Data;
using WavesCasinoAPI.Services;


var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    // options.EnableSensitiveDataLogging(true);
});

// Add services to the container.
builder.Services.AddSingleton<NodeClient>(new NodeClient(configuration.GetValue<string>("nodeApi")));
builder.Services.AddSingleton<BlockSyncService>();
builder.Services.Configure<GamesAPIOptions>(configuration.GetSection("games"));
builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var service = app.Services.GetRequiredService<BlockSyncService>();

app.UseSwagger();

app.UseCors(builder => builder
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader()
             );

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Waves Casino API V1");
});

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
