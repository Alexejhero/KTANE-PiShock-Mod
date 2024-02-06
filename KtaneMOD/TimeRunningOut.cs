using HarmonyLib;

namespace KtaneMOD;

[HarmonyPatch]
internal static class TimeRunningOut
{
    [HarmonyPatch(typeof(TimerComponent), nameof(TimerComponent.Start))]
    private static void OnTimerStartPatch(TimerComponent __instance)
    {
        __instance.TimerTick += Events.OnTimerTick;
    }

    [HarmonyPatch(typeof(BombComponent), nameof(BombComponent.OnDestroy))]
    private static void OnComponentDestroy(BombComponent __instance)
    {
        if (__instance is TimerComponent timerComponent)
            timerComponent.TimerTick -= Events.OnTimerTick;
    }
}
