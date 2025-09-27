using UnityEngine;

namespace KtaneMOD.Config;

public struct ModConfig
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

    public bool noCheating;
    public int presendOffset;
    public int minExplodeDelay;
    public int maxExplodeDelay;

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
        PlayerPrefs.SetInt("shocker_no_cheating", noCheating ? 1 : 0);
        PlayerPrefs.SetInt("shocker_presend_offset", presendOffset);
        PlayerPrefs.SetInt("shocker_min_explode_delay", minExplodeDelay);
        PlayerPrefs.SetInt("shocker_max_explode_delay", maxExplodeDelay);

        PlayerPrefs.Save();
    }

    public static ModConfig LoadFromPlayerPrefs()
    {
        return new ModConfig
        {
            username = PlayerPrefs.GetString("shocker_pishock_username"),
            apiKey = PlayerPrefs.GetString("shocker_pishock_apikey"),
            code = PlayerPrefs.GetString("shocker_pishock_code"),
            partnerCode = PlayerPrefs.GetString("shocker_pishock_partner_code"),
            strikeIntensity = PlayerPrefs.GetInt("shocker_strike_intensity", 20),
            strikeDuration = PlayerPrefs.GetInt("shocker_strike_duration", 1),
            explodeIntensity = PlayerPrefs.GetInt("shocker_explode_intensity", 30),
            explodeDuration = PlayerPrefs.GetInt("shocker_explode_duration", 1),
            shockBoth = PlayerPrefs.GetInt("shocker_shock_both", 1) == 1,
            noCheating = PlayerPrefs.GetInt("shocker_no_cheating", 1) == 1,
            presendOffset = PlayerPrefs.GetInt("shocker_presend_offset", 750),
            minExplodeDelay = PlayerPrefs.GetInt("shocker_min_explode_delay", 0),
            maxExplodeDelay = PlayerPrefs.GetInt("shocker_max_explode_delay", 5000)
        };
    }
}
