using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace KtaneMOD;

public sealed class ConfigServer : MonoBehaviour
{
    private readonly string _configHtml;
    private readonly string _testHtml;

    private Thread _thread;

    private ConfigServer()
    {
        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KtaneMOD.Resources.config.html")!;
        StreamReader reader = new(stream);
        _configHtml = reader.ReadToEnd();

        stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KtaneMOD.Resources.test.html")!;
        reader = new StreamReader(stream);
        _testHtml = reader.ReadToEnd();
    }

    private void Awake()
    {
        _thread = new Thread(RunWebServer);
        _thread.Start();
    }

    private void OnDestroy()
    {
        _thread.Abort();
    }

    private void RunWebServer()
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
                if (request.Url.AbsolutePath.Contains("amiplaying") && request.HttpMethod == "GET")
                {
                    response.StatusCode = 200;
                    response.ContentType = "text/plain";
                    response.ContentEncoding = System.Text.Encoding.UTF8;

                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Events.IsInGame ? "yes" : "no");

                    response.ContentLength64 = bytes.Length;
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.OutputStream.Close();

                    continue;
                }

                Plugin.Logger.LogWarning($"{request.HttpMethod} {request.Url.AbsolutePath}");

                if (request.Url.AbsolutePath.Contains("simpletest") && request.HttpMethod == "GET")
                {
                    PiShock.Self.Test();
                    PiShock.Partner.Test();

                    response.Redirect("/");
                }
                else if (request.Url.AbsolutePath.Contains("testpls") && request.HttpMethod == "POST")
                {
                    Dictionary<string, string> args = BodyParser.ParseUrlEncoded(request);

                    PiShock.Self.SendOperation(int.Parse(args["operation"]), int.Parse(args["intensity"]), int.Parse(args["duration"]));

                    response.StatusCode = 200;
                }
                else if (request.Url.AbsolutePath.Contains("test") && request.HttpMethod == "GET")
                {
                    response.StatusCode = 200;
                    response.ContentType = "text/html";
                    response.ContentEncoding = System.Text.Encoding.UTF8;

                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(_testHtml);

                    response.ContentLength64 = bytes.Length;
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.OutputStream.Close();
                }
                else if (request.Url.AbsolutePath.Contains("save") && request.HttpMethod == "POST")
                {
                    Dictionary<string, string> args = BodyParser.ParseUrlEncoded(request);

                    PiShockConfig config = new()
                    {
                        username = args["username"],
                        apiKey = args["apiKey"],
                        code = args["code"],
                        partnerCode = args["partnerCode"],
                        strikeIntensity = int.Parse(args["strikeIntensity"]),
                        strikeDuration = int.Parse(args["strikeDuration"]),
                        explodeIntensity = int.Parse(args["explodeIntensity"]),
                        explodeDuration = int.Parse(args["explodeDuration"]),
                        shockBoth = args.TryGetValue("shockBoth", out string value) && value == "on"
                    };
                    config.SaveToPlayerPrefs();

                    Plugin.PiShockConfig = config;

                    response.Redirect("/");
                }
                else if (request.Url.AbsolutePath == "/" && request.HttpMethod == "GET")
                {
                    PiShockConfig config = Plugin.PiShockConfig;

                    string renderedHtml = _configHtml
                        .Replace("{{username}}", config.username)
                        .Replace("{{apiKey}}", config.apiKey)
                        .Replace("{{code}}", config.code)
                        .Replace("{{partnerCode}}", config.partnerCode)
                        .Replace("{{strikeIntensity}}", config.strikeIntensity.ToString())
                        .Replace("{{strikeDuration}}", config.strikeDuration.ToString())
                        .Replace("{{explodeIntensity}}", config.explodeIntensity.ToString())
                        .Replace("{{explodeDuration}}", config.explodeDuration.ToString())
                        .Replace("{{shockBoth}}", config.shockBoth ? "checked" : "");

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
