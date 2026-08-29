using Kitchen;
using KitchenMods;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace TestMod.Team
{
    public class TeamWorldLabelSystem : GenericSystemBase, IModSystem
    {
        private static bool _created = false;

        protected override void OnUpdate()
        {
            if (!_created)
            {
                var go = new GameObject("TestMod_TeamWorldLabels");
                go.AddComponent<TeamWorldLabelBehaviour>();
                Object.DontDestroyOnLoad(go);
                _created = true;
            }

            var newLabels = new List<(Vector3, string, int)>();

            // Players
            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition), typeof(CTeamAssignment));
            var players = playerQuery.ToEntityArray(Allocator.Temp);
            foreach (var player in players)
            {
                if (!Require(player, out CPosition pos)) continue;
                if (!Require(player, out CTeamAssignment team)) continue;
                newLabels.Add((pos.Position + new Vector3(0f, 1.8f, 0f), $"Team {team.Team}", team.Team));
            }
            players.Dispose();

            // Appliances
            var applianceQuery = GetEntityQuery(typeof(CAppliance), typeof(CPosition), typeof(CTeamAssignment));
            var appliances = applianceQuery.ToEntityArray(Allocator.Temp);
            foreach (var appliance in appliances)
            {
                if (Has<CApplianceBlueprint>(appliance)) continue;
              
                if (!Require(appliance, out CPosition pos)) continue;
                if (!Require(appliance, out CTeamAssignment team)) continue;
                newLabels.Add((pos.Position + new Vector3(0f, 1.2f, 0f), $"[Team {team.Team}]", team.Team));
            }
            appliances.Dispose();

            var tablesQuery = GetEntityQuery(typeof(CApplianceTable), typeof(CPosition), typeof(CTeamAssignment));
            var tables = tablesQuery.ToEntityArray(Allocator.Temp);
            foreach (var table in tables)
            {
                if (!Require(table, out CPosition pos)) continue;
                if (!Require(table, out CTeamAssignment team)) continue;
                newLabels.Add((pos.Position + new Vector3(0f, 1.2f, 0f), $"[Team {team.Team}]", team.Team));
            }
            tables.Dispose();

            TeamWorldLabelBehaviour.Labels = newLabels;
        }
    }
}