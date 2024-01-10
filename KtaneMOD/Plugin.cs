using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using Events;
using HarmonyLib;
using UnityEngine.Networking;

namespace KtaneMOD;

[BepInPlugin("KtaneMOD", "KtaneMOD", "1.0.0"), HarmonyPatch]
public sealed class Plugin : BaseUnityPlugin
{
    public static Plugin Instance;

    public ConfigEntry<string> StrikeEndpoint;
    public ConfigEntry<string> ExplodeEndpoint;
    public ConfigEntry<string> DefuseEndpoint;

    private void Awake()
    {
        Instance = this;

        StrikeEndpoint = Config.Bind("Endpoints", "StrikeEndpoint", "http://localhost:8080/ktane/strike");
        ExplodeEndpoint = Config.Bind("Endpoints", "ExplodeEndpoint", "http://localhost:8080/ktane/explode");
        DefuseEndpoint = Config.Bind("Endpoints", "DefuseEndpoint", "http://localhost:8080/ktane/defuse");

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        BombEvents.OnBombDetonated += OnDetonate;
        BombEvents.OnBombSolved += OnDefuse;
    }

    private void OnDetonate()
    {
        Logger.LogWarning("Bomb denotated! Sending request");
        UnityWebRequest request = UnityWebRequest.Get(ExplodeEndpoint.Value);
        request.SendWebRequest();
    }

    private void OnDefuse()
    {
        Logger.LogWarning("Bomb defused! Sending request");
        UnityWebRequest request = UnityWebRequest.Get(DefuseEndpoint.Value);
        request.SendWebRequest();
    }

    [HarmonyPatch(typeof(Bomb), nameof(Bomb.OnStrike))]
    private static void Prefix(Bomb __instance)
    {
        if (__instance.NumStrikes != __instance.NumStrikesToLose - 1)
        {
            Instance.Logger.LogWarning("Got a strike! Sending request");
            UnityWebRequest request = UnityWebRequest.Get(Instance.StrikeEndpoint.Value);
            request.SendWebRequest();
        }
    }
}
