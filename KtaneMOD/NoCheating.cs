using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.DossierMenu;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace KtaneMOD;

[HarmonyPatch]
public sealed class NoCheating : MonoBehaviour
{
    public static bool CanQuit => SceneManager.Instance.CurrentState != SceneManager.State.Gameplay;

    [HarmonyPatch(typeof(GameplayMenuPage), nameof(GameplayMenuPage.ReturnToSetupRoom))]
    [HarmonyPrefix]
    private static bool ReturnToSetupRoomPatch()
    {
        return false;
    }

    [HarmonyPatch(typeof(MenuPage), nameof(MenuPage.RefreshLayout))]
    [HarmonyPrefix]
    private static void RefreshLayoutPatch(MenuPage __instance)
    {
        if (__instance is GameplayMenuPage gmp) gmp.returnToSetupEntry.IsHidden = true;
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
            return CanQuit;
        }
    }

    private void OnApplicationQuit()
    {
        if (!CanQuit)
        {
            Application.CancelQuit();
        }
    }
}
