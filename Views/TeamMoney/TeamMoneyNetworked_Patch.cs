using HarmonyLib;
using Kitchen;

namespace PlateVsPlate.Views.TeamMoney
{
    [HarmonyPatch(typeof(ApplianceView))]
    internal class TeamMoneyNetworked_Patch
    {
        [HarmonyPatch(nameof(ApplianceView.Initialise))]
        [HarmonyPostfix]
        internal static void Initialise_Postfix(ApplianceView __instance)
        {
            __instance.gameObject.AddComponent<TeamMoneyNetworkedSubview>();
        }
    }
}