using AlbionKillnet.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace AlbionKillnet.Api.Endpoints;

public static class KillEventApi
{
    public static void MapKillEventApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/player");

        group.MapGet("/{playerName}", async (string playerName, AppDbContext db) =>
        {
            var playerEvents = await db.KillEvents
                .Where(k => EF.Functions.Like(k.KillerName, playerName) || EF.Functions.Like(k.VictimName, playerName))
                .OrderByDescending(k => k.TimeStamp)
                .Take(20)
                .ToListAsync();

            if (playerEvents.Count == 0)
                return Results.NotFound(new { Message = "Oyuncu bulunamadı veya hiç savaşa girmemiş." });

            return Results.Ok(playerEvents);
        });
    }
}