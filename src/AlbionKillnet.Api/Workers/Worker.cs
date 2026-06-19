using System.Text.Json;
using AlbionKillnet.Core.Entities;
using AlbionKillnet.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace AlbionKillnet.Api.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HttpClient _httpClient;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, HttpClient httpClient)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _httpClient = httpClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 AlbionKillnet Turso (LibSQL) Worker servisi uyandı: {time}", DateTimeOffset.Now);

        var servers = new Dictionary<string, string>
        {
            { "EU", "https://gameinfo-ams.albiononline.com/api/gameinfo/events" },
            { "NA", "https://gameinfo.albiononline.com/api/gameinfo/events" },
            { "AS", "https://gameinfo-sgp.albiononline.com/api/gameinfo/events" }
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var tasks = servers.Select(server => FetchAndSaveKillsAsync(server.Key, server.Value, stoppingToken));

                await Task.WhenAll(tasks);

                await Task.Delay(2000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Worker Ana Döngü Hatası: {ex.Message}");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task FetchAndSaveKillsAsync(string region, string baseUrl, CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        int totalAddedCount = 0;
        int currentOffset = 0;
        bool keepFetching = true;

        var currentBatchEventIds = new HashSet<long>();

        try
        {
            while (keepFetching && currentOffset <= 1000 && !stoppingToken.IsCancellationRequested)
            {
                string pagedUrl = $"{baseUrl}?limit=50&offset={currentOffset}";

                var response = await _httpClient.GetAsync(pagedUrl, stoppingToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("[{Region}] API'sine ulaşılamadı. Sayfa: {Offset}", region, currentOffset);
                    break;
                }

                var jsonString = await response.Content.ReadAsStringAsync(stoppingToken);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var incomingEvents = JsonSerializer.Deserialize<List<AlbionIncomingEvent>>(jsonString, options);

                if (incomingEvents == null || incomingEvents.Count == 0)
                {
                    break;
                }

                int newEventsInThisPage = 0;

                foreach (var ev in incomingEvents)
                {
                    if (currentBatchEventIds.Contains(ev.EventId)) continue;

                    bool exists = await dbContext.KillEvents.AnyAsync(k => k.EventId == ev.EventId, stoppingToken);

                    if (exists)
                    {
                        continue;
                    }

                    newEventsInThisPage++;

                    if (ev.TotalVictimKillFame < 95000 || ev.Killer == null || ev.Victim == null)
                    {
                        continue;
                    }

                    var rawData = new KillEventRawData
                    {
                        Killer = ev.Killer,
                        Victim = ev.Victim,
                        Participants = ev.Participants ?? new List<ParticipantState>()
                    };

                    rawData.OptimizeAllData();

                    var dbEvent = new KillEvent
                    {
                        EventId = ev.EventId,
                        BattleId = ev.BattleId,
                        ServerRegion = region,
                        TimeStamp = ev.TimeStamp,
                        TotalVictimKillFame = ev.TotalVictimKillFame,
                        KillArea = ev.KillArea,
                        KillerName = ev.Killer.Name,
                        VictimName = ev.Victim.Name,
                        RawData = rawData
                    };

                    dbContext.KillEvents.Add(dbEvent);
                    currentBatchEventIds.Add(ev.EventId);
                    totalAddedCount++;
                }

                if (currentOffset >= 250 && newEventsInThisPage == 0)
                {
                    keepFetching = false;
                }
                else
                {
                    currentOffset += 50;
                    if (currentOffset > 250)
                    {
                        _logger.LogInformation("🔥 [{Region}] Geçmişe sızdırılmış (Mists/Gecikmeli) savaşlar taranıyor... (Offset: {Offset})", region, currentOffset);
                    }
                }
            }

            if (totalAddedCount > 0)
            {
                await dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("✅ [{Region}] sunucusunda aralardan {Count} adet GİZLİ/YENİ ölüm çıkarılıp Turso'ya yazıldı!", region, totalAddedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Veri çekme hatası: {ex.Message}");
        }
    }

    private class AlbionIncomingEvent
    {
        public long EventId { get; set; }
        public long BattleId { get; set; }
        public DateTime TimeStamp { get; set; }
        public int TotalVictimKillFame { get; set; }
        public string? KillArea { get; set; }
        public KillerState Killer { get; set; } = null!;
        public VictimState Victim { get; set; } = null!;
        public List<ParticipantState> Participants { get; set; } = new();
    }
}