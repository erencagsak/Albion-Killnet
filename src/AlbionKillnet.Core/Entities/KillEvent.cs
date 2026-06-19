namespace AlbionKillnet.Core.Entities;

// --- Main Table ---
public class KillEvent
{
    public long EventId { get; set; }
    public long BattleId { get; set; }
    public string ServerRegion { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
    public int TotalVictimKillFame { get; set; }
    public string? KillArea { get; set; }
    public string KillerName { get; set; } = string.Empty;
    public string VictimName { get; set; } = string.Empty;

    public KillEventRawData RawData { get; set; } = null!;
}

public class KillEventRawData
{
    public KillerState Killer { get; set; } = null!;
    public VictimState Victim { get; set; } = null!;
    public List<ParticipantState> Participants { get; set; } = new();

    public void OptimizeAllData()
    {
        Killer?.Equipment?.OptimizeStorage();
        Victim?.Equipment?.OptimizeStorage();

        if (Participants != null)
        {
            foreach (var p in Participants)
            {
                p.Equipment?.OptimizeStorage();
            }
        }
    }
}

public class BasePlayerInfo
{
    public string Name { get; set; } = string.Empty;
    public string? GuildName { get; set; }
    public string? AllianceName { get; set; }
    public double AverageItemPower { get; set; }
    public Equipment Equipment { get; set; } = null!;
}

public class KillerState : BasePlayerInfo
{
    public int KillFame { get; set; }
    public double FameRatio { get; set; }
}

public class VictimState : BasePlayerInfo
{
    public int DeathFame { get; set; }
    public List<Item?> Inventory { get; set; } = new();
}

public class ParticipantState : BasePlayerInfo
{
    public double DamageDone { get; set; }
    public double SupportHealingDone { get; set; }
}

public class Equipment
{
    public Item? MainHand { get; set; }
    public Item? OffHand { get; set; }
    public Item? Head { get; set; }
    public Item? Armor { get; set; }
    public Item? Shoes { get; set; }
    public Item? Bag { get; set; }
    public Item? Cape { get; set; }
    public Item? Mount { get; set; }
    public Item? Potion { get; set; }
    public Item? Food { get; set; }

    public void OptimizeStorage()
    {
        var otherItems = new[] { Head, Armor, Shoes, Bag, Cape, Mount, Potion, Food };

        foreach (var item in otherItems.Where(i => i != null))
        {
            item!.LegendarySoul = null;
        }
    }
}

public class Item
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Quality { get; set; }

    public LegendarySoul? LegendarySoul { get; set; }
}

public class LegendarySoul
{
    public string? Name { get; set; }
    public long PvPFameGained { get; set; }
    public List<LegendaryTrait> Traits { get; set; } = new();
}

public class LegendaryTrait
{
    public string? Trait { get; set; }
    public double Value { get; set; }
}