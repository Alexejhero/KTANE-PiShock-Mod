using System.Collections.Generic;

namespace KtaneMOD.Config;

public static class RandomTargets
{
    public static List<PiShock> GetShockTargets()
    {
        if (Plugin.ModConfig.shockBoth)
        {
            Plugin.Logger.LogWarning("Fair punishment");
            return [PiShock.Self, PiShock.Partner];
        }

        return [PiShock.Self];
    }

    public static void Strike(this IEnumerable<PiShock> pishocks)
    {
        foreach (PiShock pishock in pishocks)
            pishock.Strike();
    }

    public static void Explode(this IEnumerable<PiShock> pishocks, float delay = 0f)
    {
        foreach (PiShock pishock in pishocks)
            pishock.Explode(delay);
    }
}
