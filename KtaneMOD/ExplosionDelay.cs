using System.Collections;
using DarkTonic.MasterAudio;
using HarmonyLib;
using UnityEngine;

namespace KtaneMOD;

[HarmonyPatch]
internal static class ExplosionDelay
{
    [HarmonyPatch(typeof(PostGameState), nameof(PostGameState.EnterState))]
    [HarmonyPrefix]
    private static bool DelayEndGameResults(PostGameState __instance, bool success)
    {
        if (ExplosionHandler.DelayAmount <= 0) return true;

        Plugin.Instance.StartCoroutine(CoEnterState());

        return false;

        IEnumerator CoEnterState()
        {
            yield return new WaitForSeconds(ExplosionHandler.DelayAmount);
            ExplosionHandler.DelayAmount = 0;

            __instance.EnterState(success);
        }
    }

    [HarmonyPatch(typeof(MasterAudio), nameof(MasterAudio.PlaySound3DAtTransformAndForget))]
    [HarmonyPrefix]
    private static void DelayBombExplosionSound(string sType, ref float delaySoundTime)
    {
        if (ExplosionHandler.DelayAmount <= 0) return;

        if (sType == "bomb_explode") delaySoundTime = ExplosionHandler.DelayAmount;
    }
}
