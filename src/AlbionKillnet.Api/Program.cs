using AlbionKillnet.Api.Worker;
using AlbionKillnet.Core.Data;
using AlbionKillnet.Api.Endpoints;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Albion Killnet API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Albion Killnet API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("ReactAppPolicy");

app.MapGet("/health", () => Results.Ok("Motor aktif ve saglikli!"));

app.MapHomepageApi();
app.MapKillEventApi();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");