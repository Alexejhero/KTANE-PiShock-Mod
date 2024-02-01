using System.Collections;
using Assets.Scripts.Records;
using UnityEngine;

namespace KtaneMOD;

public static class Events
{
    public static float DelayAmount { get; set; }

    public static void OnStrike()
    {
        PiShock.Strike();
    }

    public static void OnExplode()
    {
        GameRecord record = RecordManager.Instance.GetCurrentRecord();

        if (record.Result == GameResultEnum.ExplodedDueToTime || record.Result == GameResultEnum.ExplodedDueToStrikes && record.TimeRemaining < 15f)
        {
            int branch = Random.Range(0, 4);

            switch (branch)
            {
                case 0: // short delay, shock
                    DelayAmount = Random.Range(2f, 4f);
                    break;

                case 1: // long delay, shock
                    DelayAmount = Random.Range(6f, 8f);
                    break;

                case 2: // long delay, vibration + shock
                    DelayAmount = Random.Range(6f, 8f);
                    float fakeoutDelay = Random.Range(0.3f, 0.9f) * DelayAmount;
                    PiShock.FakeoutExplode(fakeoutDelay);
                    break;

                case 3: // instant explosion
                    DelayAmount = 0f;
                    break;
            }

            Plugin.Logger.LogWarning($"Delayed for {DelayAmount}, branch {branch}");
            PiShock.Explode(DelayAmount);
        }
        else if (record.Result == GameResultEnum.ExplodedDueToStrikes)
        {
            PiShock.Explode();
        }
    }

    public static void OnDefuse()
    {
        PiShock.Defuse();
    }

    public static void OnReturnToOffice()
    {
        PiShock.Explode();
    }

    public static void OnTimerTick(int elapsed, int remaining)
    {
        if (remaining <= 15)
        {
            PiShock.TimeRunningOut();
        }
    }

    private static Coroutine _alarmClockCoroutine;
    public static void OnAlarmClockChange(bool on)
    {
        _alarmClockCoroutine = on ? Plugin.Instance.StartCoroutine(CoVibrate()) : null;
        return;

        IEnumerator CoVibrate()
        {
            while (_alarmClockCoroutine != null)
            {
                PiShock.AlarmClockBeep();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public static void OnLightsOff()
    {
        PiShock.LightsOff();
    }
}
