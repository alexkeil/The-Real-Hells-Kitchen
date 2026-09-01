using Kitchen;
using KitchenMods;
using PlateVsPlate.Team;
using PlateVsPlate.Team.TeamChecks;
using Unity.Entities;
using UnityEngine;

namespace PlateVsPlate.Debugging
{
    public class SpawnTeamMarkers : GenericSystemBase, IModSystem
    {
        private static bool _spawned = false;

        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F4)) return;
            if (_spawned) return; 

            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            if (players.Length == 0) 
            { 
                players.Dispose();
                return; 
            }

            CPosition playerPos = default;
            Require(players[0], out playerPos);
            players.Dispose();

            int markerApplianceID = -1448690107;    // Danger hob - this is temp

            Entity left = EntityManager.CreateEntity();
            EntityManager.AddComponentData(left, new CCreateAppliance { ID = markerApplianceID });
            EntityManager.AddComponentData(left, new CPosition { Position = playerPos.Position + new Vector3(-3f, 0f, 0f) });
            EntityManager.AddComponentData(left, new CTeamAssignment { Team = 0 });
            EntityManager.AddComponentData(left, new CTeamSelector { Team = 0 });

            Entity right = EntityManager.CreateEntity();
            EntityManager.AddComponentData(right, new CCreateAppliance { ID = markerApplianceID });
            EntityManager.AddComponentData(right, new CPosition { Position = playerPos.Position + new Vector3(3f, 0f, 0f) });
            EntityManager.AddComponentData(right, new CTeamAssignment { Team = 1 });
            EntityManager.AddComponentData(right, new CTeamSelector { Team = 1 });

            _spawned = true;
        }
    }
}