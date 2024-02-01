using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace KtaneMOD;

public sealed class ConfigServer : MonoBehaviour
{
    private Thread _thread;

    private void Awake()
    {
        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KtaneMOD.Resources.config.html")!;
        StreamReader reader = new(stream);
        string html = reader.ReadToEnd();

        _thread = new Thread(() => RunWebServer(html));
        _thread.Start();
    }

    private void OnDestroy()
    {
        _thread.Abort();
    }

    private static void RunWebServer(string configHtml)
    {
        HttpListener listener = new();
        listener.Prefixes.Add("http://localhost:6969/");
        listener.Start();

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            try
            {
                Plugin.Logger.LogWarning(request.Url.AbsolutePath);
                Plugin.Logger.LogWarning(request.HttpMethod);

                if (request.Url.AbsolutePath.Contains("save") && request.HttpMethod == "POST")
                {
                    Dictionary<string, string> args = BodyParser.ParseUrlEncoded(request);

                    Plugin.Logger.LogWarning(string.Join(",", args.Select(a => $"{a.Key}={a.Value}").ToArray()));

                    PiShockConfig config = new()
                    {
                        username = args["username"],
                        apiKey = args["apiKey"],
                        code = args["code"],
                        strikeIntensity = int.Parse(args["strikeIntensity"]),
                        strikeDuration = int.Parse(args["strikeDuration"]),
                        explodeIntensity = int.Parse(args["explodeIntensity"]),
                        explodeDuration = int.Parse(args["explodeDuration"]),
                        // preventCheating = args.TryGetValue("preventCheating", out string value) && value == "on"
                    };
                    config.SaveToPlayerPrefs();

                    Plugin.PiShockConfig = config;

                    response.Redirect("/");
                }
                else if (request.Url.AbsolutePath.Contains("test") && request.HttpMethod == "GET")
                {
                    PiShock.Test();

                    response.Redirect("/");
                }
                else if (request.Url.AbsolutePath == "/" && request.HttpMethod == "GET")
                {
                    PiShockConfig config = Plugin.PiShockConfig;

                    string renderedHtml = configHtml
                        .Replace("{{username}}", config.username)
                        .Replace("{{apiKey}}", config.apiKey)
                        .Replace("{{code}}", config.code)
                        .Replace("{{strikeIntensity}}", config.strikeIntensity.ToString())
                        .Replace("{{strikeDuration}}", config.strikeDuration.ToString())
                        .Replace("{{explodeIntensity}}", config.explodeIntensity.ToString())
                        .Replace("{{explodeDuration}}", config.explodeDuration.ToString())
                        // .Replace("{{preventCheating}}", config.preventCheating ? "checked" : "")
                        ;

                    response.StatusCode = 200;
                    response.ContentType = "text/html";
                    response.ContentEncoding = System.Text.Encoding.UTF8;

                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(renderedHtml);

                    response.ContentLength64 = bytes.Length;
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.OutputStream.Close();
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
                response.StatusCode = 500;
            }

            response.Close();
        }
        // ReSharper disable once FunctionNeverReturns
    }
}
