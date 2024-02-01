using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace KtaneMOD;

public static class PiShock
{
    private const string API_URL = "https://do.pishock.com/api/apioperate/";

    public static void Strike(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(0, Plugin.PiShockConfig.strikeIntensity, Plugin.PiShockConfig.strikeDuration);
        });
    }

    public static void Explode(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(0, Plugin.PiShockConfig.explodeIntensity, Plugin.PiShockConfig.explodeDuration);
        });
    }

    public static void FakeoutExplode(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(1, 80, 300);
        });
    }

    public static void Defuse(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(1, 20, 500);
        });
    }

    public static void TimeRunningOut(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(2, 50, 300);
        });
    }

    public static void AlarmClockBeep(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(1, 20, 150);
        });
    }

    public static void Test(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(1, 20, 1);
            SendOperation(2, 50, 1);
        });
    }

    private static void ExecuteWithDelay(float delay, Action action)
    {
        Plugin.Instance.StartCoroutine(Coroutine());
        return;

        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(delay);
            action();
        }
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
