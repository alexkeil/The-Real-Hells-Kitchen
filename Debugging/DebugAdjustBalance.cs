using PlateVsPlate;
using PlateVsPlate.Team.TeamChecks;
using UnityEngine;

namespace PlateVsPlate.Debugging
{
    public class DebugAdjustBalance : Kitchen.GenericSystemBase, KitchenMods.IModSystem
    {
        protected override void OnUpdate()
        {
            // Group count adjustments
            if (Input.GetKeyDown(KeyCode.F9))
            {
                CustomerTeamBalanceTracker.Team0Groups += 1;
                Mod.Logger.LogInfo($"[DEBUGGING] Team A Groups +3 = {CustomerTeamBalanceTracker.Team0Groups}");
            }
            if (Input.GetKeyDown(KeyCode.F10))
            {
                CustomerTeamBalanceTracker.Team1Groups += 1;
                Mod.Logger.LogInfo($"[DEBUGGING] Team B Groups +3 = {CustomerTeamBalanceTracker.Team1Groups}");
            }

            // Customer count adjustments
            if (Input.GetKeyDown(KeyCode.F11))
            {
                CustomerTeamBalanceTracker.Team0Customers += 1;
                Mod.Logger.LogInfo($"[DEBUGGING] Team A Customers +3 = {CustomerTeamBalanceTracker.Team0Customers}");
            }
            if (Input.GetKeyDown(KeyCode.F12))
            {
                CustomerTeamBalanceTracker.Team1Customers += 1;
                Mod.Logger.LogInfo($"[DEBUGGING] Team B Customers +3 = {CustomerTeamBalanceTracker.Team1Customers}");
            }
        }
    }
}