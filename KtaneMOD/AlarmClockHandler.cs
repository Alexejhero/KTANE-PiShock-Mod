using System.Collections;
using Assets.Scripts.Props;
using HarmonyLib;
using KtaneMOD.Config;
using UnityEngine;

namespace KtaneMOD;

[HarmonyPatch]
internal static class AlarmClockHandler
{
    private static bool _disablePatch;

    [HarmonyPatch(typeof(AlarmClock), nameof(AlarmClock.TurnOn))]
    [HarmonyPrefix]
    private static bool DelayAlarmClock(AlarmClock __instance)
    {
        if (_disablePatch) return true;

        PiShock.Self.AlarmClockOn();
        Plugin.Instance.StartCoroutine(CoTurnOn());

        return false;

        IEnumerator CoTurnOn()
        {
            yield return new WaitForSeconds(Plugin.ModConfig.presendOffset);

            _disablePatch = true;
            __instance.TurnOn();
            _disablePatch = false;
        }
    }
}
