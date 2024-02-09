using System.Collections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace KtaneMOD.Config;

[HarmonyPatch]
public static class OnlineConfig
{
    private const string GIST_URL = "https://gist.githubusercontent.com/Alexejhero/161af5ec58e50694b6a4c7b4b1e6055b/raw";

    public static bool NoCheating { get; private set; }
    public static float PresendOffset { get; private set; }

    private static IEnumerator CoRefresh()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{GIST_URL}?{(int) Time.time}");
        yield return www.SendWebRequest();

        if (www.responseCode != 200)
        {
            Plugin.Logger.LogError("Failed to fetch online config: " + www.error);
            NoCheating = false;
            yield break;
        }

        NoCheating = www.downloadHandler.text.Contains("no-cheating");

        Plugin.Logger.LogMessage($"Online config fetched: NoCheating is {NoCheating}");
    }

    [HarmonyPatch(typeof(GameplayState), nameof(GameplayState.EnterState))]
    [HarmonyPrefix]
    private static void EnterStatePatch()
    {
        Plugin.Instance.StartCoroutine(CoRefresh());
    }
}
