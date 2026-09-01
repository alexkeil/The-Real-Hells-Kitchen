using PlateVsPlate.Setttings;
using PlateVsPlate.Team.TeamChecks;
using UnityEngine;

namespace PlateVsPlate.Debugging
{
    public class BalanceDebugDisplayBehaviour : MonoBehaviour
    {
        private GUIStyle _style;

        private void OnGUI()
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label) { fontSize = 20, normal = { textColor = Color.white } };
            }

            string mode = PvPModSettings.GetBalanceByCustomerCount() ? "Customer Count" : "Group Count";
            GUI.Label(new Rect(20, 100, 500, 30), $"Balance Mode: {mode}", _style);
            GUI.Label(new Rect(20, 130, 500, 30), $"Team A — Groups: {CustomerTeamBalanceTracker.Team0Groups}, Customers: {CustomerTeamBalanceTracker.Team0Customers}", _style);
            GUI.Label(new Rect(20, 160, 500, 30), $"Team B — Groups: {CustomerTeamBalanceTracker.Team1Groups}, Customers: {CustomerTeamBalanceTracker.Team1Customers}", _style);
            GUI.Label(new Rect(20, 190, 500, 30), $"Preferred Team Right Now: {(CustomerTeamBalanceTracker.PreferredTeam() == 0 ? "A" : "B")}", _style);
        }
    }

    public class BalanceDebugDisplaySystem : Kitchen.GenericSystemBase, KitchenMods.IModSystem
    {
        private static bool _created = false;

        protected override void OnUpdate()
        {
            if (!_created)
            {
                var go = new GameObject("TestMod_BalanceDebugDisplay");
                go.AddComponent<BalanceDebugDisplayBehaviour>();
                Object.DontDestroyOnLoad(go);
                _created = true;
            }
        }
    }
}