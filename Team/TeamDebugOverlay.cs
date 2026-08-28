using Kitchen;
using KitchenMods;
using System.Text;
using UnityEngine;

namespace TestMod.Team
{
    public class TeamDebugOverlay : GenericSystemBase, IModSystem
    {
        private static bool _created = false;

        protected override void OnUpdate()
        {

            if (!_created)
            {
                var go = new GameObject("TestMod_DebugOverlay");
                go.AddComponent<TeamDebugOverlayBehaviour>();
                Object.DontDestroyOnLoad(go);
                _created = true;
            }

            var sb = new StringBuilder();
            foreach (var teamData in TeamMoney.Teams.Values)
            {
                if (sb.Length > 0) sb.Append("\n");
                sb.Append($"\nTeam {teamData.Team}: {teamData.Balance}");
            }
            
            TeamDebugOverlayBehaviour.MoneyText = sb.ToString();

        }
    }
}