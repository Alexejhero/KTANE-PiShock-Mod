using System.Collections.Generic;
using UnityEngine.Networking;

namespace KtaneMOD;

public static class PiShock
{
    private const string API_URL = "https://do.pishock.com/api/apioperate/";

    public static void Strike()
    {
        SendOperation(0, Plugin.PiShockConfig.strikeIntensity, Plugin.PiShockConfig.strikeDuration);
    }

    public static void Explode()
    {
        SendOperation(0, Plugin.PiShockConfig.explodeIntensity, Plugin.PiShockConfig.explodeDuration);
    }

    public static void Test()
    {
        SendOperation(1, 20, 1);
        SendOperation(2, 50, 1);
    }

    private static void SendOperation(int op, int intensity, int duration)
    {
        if (!Plugin.PiShockConfig.IsValid()) return;

        UnityWebRequest request = UnityWebRequest.Post(API_URL, new Dictionary<string, string>
        {
            {"Username", Plugin.PiShockConfig.username},
            {"Apikey", Plugin.PiShockConfig.apiKey},
            {"Code", Plugin.PiShockConfig.code},
            {"Name", "Keep Talking and Nobody Explodes"},

            {"Op", op.ToString()},
            {"Intensity", intensity.ToString()},
            {"Duration", duration.ToString()},
        });

        request.SendWebRequest();
    }
}
