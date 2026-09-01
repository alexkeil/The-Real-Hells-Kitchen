using HarmonyLib;
using Kitchen;
using PlateVsPlate.Views.Appliance;

namespace PlateVsPlate.Views
{
    [HarmonyPatch(typeof(ApplianceView))]
    internal class TeamViewSubviews_Patch
    {
        [HarmonyPatch(nameof(ApplianceView.Initialise))]
        [HarmonyPostfix]
        internal static void Initialise_Postfix(ApplianceView __instance)
        {
            __instance.gameObject.AddComponent<ApplianceTintSubview>();
        }
    }
}