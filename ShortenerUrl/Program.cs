using Microsoft.EntityFrameworkCore;
using ShortenerUrl.Endpoints;
using ShortenerUrl.Infrastructure;
using ShortenerUrl.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ShortenService>();
builder.Services.AddDbContext<ShortenUrlDbContext>(configure =>
{
    var hostName = builder.Configuration["MongoDbConnectionString:Host"];
    var databaseName = builder.Configuration["MongoDbConnectionString:DatabaseName"];
    if(hostName is null)
        throw new ArgumentNullException(nameof(hostName));
    if(databaseName is null)
        throw new ArgumentNullException(nameof(databaseName));
    configure.UseMongoDB(hostName, databaseName);
});


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.MapShortenEndPoint();
app.MapRedirectEndPoint();


app.Run();

