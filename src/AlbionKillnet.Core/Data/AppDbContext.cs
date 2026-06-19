using AlbionKillnet.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace AlbionKillnet.Core.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _config;

    public AppDbContext(IConfiguration config)
    {
        _config = config;
    }

    public DbSet<KillEvent> KillEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var rawUrl = _config["Turso:Url"] ?? string.Empty;
        var url = rawUrl.Replace("libsql://", "https://").TrimEnd('/');

        var token = _config["Turso:Token"];

        optionsBuilder.UseLibSql($"{url}/v2/pipeline;{token}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        modelBuilder.Entity<KillEvent>(entity =>
        {
            entity.HasKey(e => e.EventId);

            entity.Property(e => e.RawData)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, jsonOptions),
                      v => JsonSerializer.Deserialize<KillEventRawData>(v, jsonOptions)!
                  );
        });
    }
}