using System.Collections;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KtaneMOD.Config;

[HarmonyPatch]
public static class OnlineConfig
{
    static OnlineConfig() // ran by Harmony
    {
        ResetToDefault();
    }

    private const string GIST_URL = "https://gist.githubusercontent.com/Alexejhero/161af5ec58e50694b6a4c7b4b1e6055b/raw";

    public static bool NoCheating { get; private set; }
    public static float PresendOffset { get; private set; }
    public static float MinExplodeDelay { get; private set; }
    public static float MaxExplodeDelay { get; private set; }

    private static void ResetToDefault()
    {
        NoCheating = false;
        PresendOffset = 0.75f;
        MinExplodeDelay = 0f;
        MaxExplodeDelay = 5f;
    }

    private static IEnumerator CoRefresh()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{GIST_URL}?{(int) Time.time}");
        yield return www.SendWebRequest();

        if (www.responseCode != 200)
        {
            Plugin.Logger.LogError("Failed to fetch online config: " + www.error);
            ResetToDefault();
            yield break;
        }

        var x = JsonConvert.DeserializeAnonymousType(www.downloadHandler.text, new
        {
            no_cheating = false,
            presend_offset = 0.75f,
            min_explode_delay = 0f,
            max_explode_delay = 5f
        });

        NoCheating = x.no_cheating;
        PresendOffset = x.presend_offset;
        MinExplodeDelay = x.min_explode_delay;
        MaxExplodeDelay = x.max_explode_delay;

        Plugin.Logger.LogMessage($"Online config fetched: {NoCheating} {PresendOffset} {MinExplodeDelay} {MaxExplodeDelay}");
    }

    [HarmonyPatch(typeof(GameplayState), nameof(GameplayState.EnterState))]
    [HarmonyPrefix]
    private static void EnterStatePatch()
    {
        Plugin.Instance.StartCoroutine(CoRefresh());
    }
}
