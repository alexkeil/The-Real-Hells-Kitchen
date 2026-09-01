using Kitchen;
using KitchenMods;
using PlateVsPlate.Settings;
using PlateVsPlate.Team;
using PlateVsPlate.Views.TeamMoney;
using UnityEngine;

namespace PlateVsPlate.Views
{
    public class RefreshTeamPanels : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!MoneyDisplayUpdate_Patch.PanelsReady) return;

            bool inKitchen = GameInfo.CurrentScene == SceneType.Kitchen;
            MoneyDisplayUpdate_Patch.SetVisible(inKitchen);
            if (!inKitchen) return; // don't bother updating text while hidden

            var t0 = TeamData.Get(0);
            var t1 = TeamData.Get(1);
            int maxStrikes = PvPStrikeSettings.StrikesBeforeElimination;

            string strikes0 = new string('X', t0.Strikes) + new string('_', Mathf.Max(0, maxStrikes - t0.Strikes));
            string strikes1 = new string('X', t1.Strikes) + new string('_', Mathf.Max(0, maxStrikes - t1.Strikes));

            MoneyDisplayUpdate_Patch.SetTeam0Money($"{t0.Balance}");
            MoneyDisplayUpdate_Patch.SetTeam0Strikes(strikes0);
            MoneyDisplayUpdate_Patch.SetTeam1Money($"{t1.Balance}");
            MoneyDisplayUpdate_Patch.SetTeam1Strikes(strikes1);
        }
    }
}