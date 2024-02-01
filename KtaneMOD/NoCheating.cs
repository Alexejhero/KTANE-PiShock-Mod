using Assets.Scripts.DossierMenu;
using HarmonyLib;

namespace KtaneMOD;

[HarmonyPatch]
public static class NoCheating
{
    [HarmonyPatch(typeof(GameplayMenuPage), "ReturnToSetupRoom")]
    [HarmonyPrefix]
    private static void OnReturnToOfficePatch()
    {
        /*if (PiShockConfig.preventCheating)*/ Events.OnReturnToOffice();
    }
}
