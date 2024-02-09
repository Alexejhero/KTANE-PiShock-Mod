using Assets.Scripts.Records;
using UnityEngine;

namespace KtaneMOD;

public static class Events
{
    public static bool IsInGame => SceneManager.Instance && SceneManager.Instance.CurrentState == SceneManager.State.Gameplay;

    public static float DelayAmount { get; set; }

    public static void OnStrike()
    {
        RandomTargets.GetShockTargets().Strike();
    }

    public static void OnExplode()
    {
        GameRecord record = RecordManager.Instance.GetCurrentRecord();

        if (record.Result == GameResultEnum.ExplodedDueToTime || record.Result == GameResultEnum.ExplodedDueToStrikes && record.TimeRemaining < 15f)
        {
            DelayAmount = Random.Range(0.75f, 6f);
            Plugin.Logger.LogWarning($"Delayed for {DelayAmount}");
            RandomTargets.GetShockTargets().Explode(DelayAmount - 0.75f);
        }
        else if (record.Result == GameResultEnum.ExplodedDueToStrikes)
        {
            RandomTargets.GetShockTargets().Explode();
        }
    }

    public static void OnDefuse()
    {
        PiShock.Self.Defuse();
        PiShock.Partner.Defuse();
    }

    public static void OnAlarmClockChange(bool on)
    {
        if (on) PiShock.Self.AlarmClockBeep();
    }
}
