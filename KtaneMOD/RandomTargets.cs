using System.Collections.Generic;
using UnityEngine;

namespace KtaneMOD;

public static class RandomTargets
{
    public static List<PiShock> GetShockTargets()
    {
        if (Plugin.PiShockConfig.shockBoth)
        {
            Plugin.Logger.LogWarning("Fair punishment");
            return [PiShock.Self, PiShock.Partner];
        }

        float rand = Random.Range(0f, 1f);

        Plugin.Logger.LogWarning("RNGesus says: " + rand);

        return rand switch
        {
            < 0.2f => [PiShock.Self],
            < 0.4f => [PiShock.Partner],
            _ => [PiShock.Self, PiShock.Partner]
        };
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
