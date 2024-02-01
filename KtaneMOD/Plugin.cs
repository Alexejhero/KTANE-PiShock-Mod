using System.Reflection;
using Assets.Scripts.DossierMenu;
using BepInEx;
using BepInEx.Logging;
using Events;
using HarmonyLib;

namespace KtaneMOD;

[BepInPlugin("KtaneMOD", "KtaneMOD", "1.0.0"), HarmonyPatch]
public sealed class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; } = BepInEx.Logging.Logger.CreateLogSource("KtaneMOD");
    public static PiShockConfig PiShockConfig { get; set; }

    private void Awake()
    {
        gameObject.AddComponent<ConfigServer>();

        PiShockConfig = PiShockConfig.LoadFromPlayerPrefs();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        BombEvents.OnBombDetonated += Events.OnExplode;
        BombEvents.OnBombSolved += Events.OnDefuse;
    }

    [HarmonyPatch(typeof(Bomb), nameof(Bomb.OnStrike))]
    private static void OnStrikePatch(Bomb __instance)
    {
        if (__instance.NumStrikes != __instance.NumStrikesToLose - 1)
        {
            Events.OnStrike();
        }
    }

    [HarmonyPatch(typeof(GameplayMenuPage), "ReturnToSetupRoom")]
    private static void OnQuitPatch()
    {
        if (PiShockConfig.preventCheating) Events.OnExplode();
    }
}
