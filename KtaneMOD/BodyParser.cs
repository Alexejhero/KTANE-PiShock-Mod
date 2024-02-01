using System.Collections.Generic;
using System.IO;
using System.Net;

namespace KtaneMOD;

public class BodyParser
{
    public static Dictionary<string, string> ParseUrlEncoded(HttpListenerRequest request)
    {
        Dictionary<string, string> result = new();

        string bodyString = new StreamReader(request.InputStream).ReadToEnd();
        string[] bodyParts = bodyString.Split('&');

        foreach (string bodyPart in bodyParts)
        {
            string[] keyValue = bodyPart.Split('=');
            result.Add(keyValue[0], keyValue[1]);
        }

        return result;
    }
}
