using AlbionKillnet.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace AlbionKillnet.Api.Endpoints;

public static class HomepageApi
{
    public static void MapHomepageApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/home");

        group.MapGet("/recent-kills", async (AppDbContext db) =>
        {
            var recentKills = await db.KillEvents
                .OrderByDescending(k => k.TimeStamp)
                .Take(15)
                .ToListAsync();

            return Results.Ok(recentKills);
        });
    }
}