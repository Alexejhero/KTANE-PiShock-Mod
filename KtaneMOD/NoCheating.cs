using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.DossierMenu;
using HarmonyLib;
using JetBrains.Annotations;
using KtaneMOD.Config;
using UnityEngine;

namespace KtaneMOD;

[HarmonyPatch]
public sealed class NoCheating : MonoBehaviour
{
    private static bool ShouldPreventClose => OnlineConfig.NoCheating && Plugin.IsInGame;

    [HarmonyPatch(typeof(GameplayMenuPage), nameof(GameplayMenuPage.ReturnToSetupRoom))]
    [HarmonyPrefix]
    private static bool ReturnToSetupRoomPatch()
    {
        return !OnlineConfig.NoCheating;
    }

    [HarmonyPatch(typeof(MenuPage), nameof(MenuPage.RefreshLayout))]
    [HarmonyPrefix]
    private static void RefreshLayoutPatch(MenuPage __instance)
    {
        if (OnlineConfig.NoCheating && __instance is GameplayMenuPage gmp) gmp.returnToSetupEntry.IsHidden = true;
    }

    [HarmonyPatch]
    public static class DelayOnApplicationQuitPatch
    {
        [HarmonyTargetMethods, UsedImplicitly]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Component).IsAssignableFrom(t))
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .Where(m => m.Name == "OnApplicationQuit" && m.DeclaringType!.Name != nameof(NoCheating))
                .Select(m =>
                {
                    Plugin.Logger.LogMessage("Patching " + m.FullDescription());
                    return (MethodBase) m;
                });
        }

        [HarmonyPrefix, UsedImplicitly]
        public static bool OnApplicationQuitPatch()
        {
            return !ShouldPreventClose;
        }
    }

    private void OnApplicationQuit()
    {
        if (ShouldPreventClose)
        {
            Application.CancelQuit();
        }
    }
}
