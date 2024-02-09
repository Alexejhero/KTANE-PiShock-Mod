using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KtaneMOD;

public sealed class PiShock
{
    public static readonly PiShock Self = new(false);
    public static readonly PiShock Partner = new(true);

    private readonly bool _isPartner;
    private PiShock(bool partner) => _isPartner = partner;

    private const string API_URL = "https://do.pishock.com/api/apioperate/";

    public void Strike(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(0, Plugin.ModConfig.strikeIntensity, Plugin.ModConfig.strikeDuration);
        });
    }

    public void Explode(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(0, Plugin.ModConfig.explodeIntensity, Plugin.ModConfig.explodeDuration);
        });
    }

    public void Defuse(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(1, 100, 500);
        });
    }

    public void AlarmClockOn(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(1, 100, 100);
        });
    }

    public void Test(float delay = 0)
    {
        ExecuteWithDelay(delay, () =>
        {
            SendOperation(1, 20, 1);
            SendOperation(2, 50, 1);
        });
    }

    public void SendOperation(int op, int intensity, int duration)
    {
        Plugin.Instance.StartCoroutine(Coroutine());
        return;

        IEnumerator Coroutine()
        {
            Dictionary<string, object> @params = new()
            {
                {"Username", Plugin.ModConfig.username},
                {"Apikey", Plugin.ModConfig.apiKey},
                {"Code", !_isPartner ? Plugin.ModConfig.code : Plugin.ModConfig.partnerCode},
                {"Name", "Keep Talking and Nobody Explodes"},

                {"Op", op},
                {"Intensity", intensity},
                {"Duration", duration},
            };
            string jsonBody = JsonConvert.SerializeObject(@params);

            UnityWebRequest request = new(API_URL, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Plugin.Logger.LogFatal($"Sending PiShock operation: {(!_isPartner ? "self" : "partner")} {op} {intensity} {duration}");

            yield return request.SendWebRequest();

            Plugin.Logger.LogFatal(request.responseCode + " " + request.downloadHandler.text);
        }
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
}
