using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Events;
using HarmonyLib;
using KtaneMOD.Config;
using KtaneMOD.Server;

namespace KtaneMOD;

[BepInPlugin("KtaneMOD", "KtaneMOD", "1.0.0"), HarmonyPatch]
public sealed class Plugin : BaseUnityPlugin
{
    public static bool IsInGame => SceneManager.Instance && SceneManager.Instance.CurrentState == SceneManager.State.Gameplay;

    public static Plugin Instance { get; private set; }

    public new static ManualLogSource Logger { get; } = BepInEx.Logging.Logger.CreateLogSource("KtaneMOD");
    public static ModConfig ModConfig { get; set; }

    private void Awake()
    {
        Instance = this;

        gameObject.AddComponent<ConfigServer>();
        gameObject.AddComponent<NoCheating>();

        ModConfig = ModConfig.LoadFromPlayerPrefs();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }
}
