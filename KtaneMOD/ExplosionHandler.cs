using Assets.Scripts.Records;
using Events;
using HarmonyLib;
using KtaneMOD.Config;
using UnityEngine;

namespace KtaneMOD;

[HarmonyPatch]
public static class ExplosionHandler
{
    static ExplosionHandler() // ran by Harmony
    {
        BombEvents.OnBombDetonated += OnExplode;
        BombEvents.OnBombSolved += OnDefuse;
    }

    public static float DelayAmount { get; set; }

    private static void OnExplode()
    {
        GameRecord record = RecordManager.Instance.GetCurrentRecord();

        if (record.Result == GameResultEnum.ExplodedDueToTime || record.Result == GameResultEnum.ExplodedDueToStrikes && record.TimeRemaining < 15f)
        {
            DelayAmount = Random.Range(OnlineConfig.MinExplodeDelay + OnlineConfig.PresendOffset, OnlineConfig.MaxExplodeDelay + OnlineConfig.PresendOffset);
            Plugin.Logger.LogWarning($"Delayed for {DelayAmount}");
            RandomTargets.GetShockTargets().Explode(DelayAmount - OnlineConfig.PresendOffset);
        }
        else if (record.Result == GameResultEnum.ExplodedDueToStrikes)
        {
            RandomTargets.GetShockTargets().Explode();
        }
    }

    private static void OnDefuse()
    {
        PiShock.Self.Defuse();
        PiShock.Partner.Defuse();
    }

    [HarmonyPatch(typeof(Bomb), nameof(Bomb.OnStrike))]
    [HarmonyPrefix]
    private static void OnStrikePatch(Bomb __instance)
    {
        if (__instance.NumStrikes != __instance.NumStrikesToLose - 1)
        {
            RandomTargets.GetShockTargets().Strike();
        }
    }
}
