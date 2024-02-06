using UnityEngine;

namespace KtaneMOD;

public struct PiShockConfig
{
    public string username;
    public string apiKey;
    public string code;
    public string partnerCode;

    public int strikeIntensity;
    public int strikeDuration;

    public int explodeIntensity;
    public int explodeDuration;

    public bool shockBoth;

    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetString("shocker_pishock_username", username);
        PlayerPrefs.SetString("shocker_pishock_apikey", apiKey);
        PlayerPrefs.SetString("shocker_pishock_code", code);
        PlayerPrefs.SetString("shocker_pishock_partner_code", partnerCode);
        PlayerPrefs.SetInt("shocker_strike_intensity", strikeIntensity);
        PlayerPrefs.SetInt("shocker_strike_duration", strikeDuration);
        PlayerPrefs.SetInt("shocker_explode_intensity", explodeIntensity);
        PlayerPrefs.SetInt("shocker_explode_duration", explodeDuration);
        PlayerPrefs.SetInt("shocker_shock_both", shockBoth ? 1 : 0);

        PlayerPrefs.Save();
    }

    public static PiShockConfig LoadFromPlayerPrefs()
    {
        return new PiShockConfig
        {
            username = PlayerPrefs.GetString("shocker_pishock_username"),
            apiKey = PlayerPrefs.GetString("shocker_pishock_apikey"),
            code = PlayerPrefs.GetString("shocker_pishock_code"),
            partnerCode = PlayerPrefs.GetString("shocker_pishock_partner_code"),
            strikeIntensity = PlayerPrefs.GetInt("shocker_strike_intensity", 20),
            strikeDuration = PlayerPrefs.GetInt("shocker_strike_duration", 1),
            explodeIntensity = PlayerPrefs.GetInt("shocker_explode_intensity", 30),
            explodeDuration = PlayerPrefs.GetInt("shocker_explode_duration", 1),
            shockBoth = PlayerPrefs.GetInt("shocker_shock_both", 0) == 1
        };
    }
}
