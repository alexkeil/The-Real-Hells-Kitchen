using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Debugging
{
    public class SpawnTeamMarkers : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F4)) return;

            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            if (players.Length == 0)
            {
                Debug.Log("[DEBUGGING] no player found.");
                players.Dispose();
                return;
            }

            CPosition playerPos = default;
            Require(players[0], out playerPos);
            players.Dispose();

            int markerApplianceID = -1448690107; // temp item

            Entity left = EntityManager.CreateEntity();
            EntityManager.AddComponentData(left, new CCreateAppliance { ID = markerApplianceID });
            EntityManager.AddComponentData(left, new CPosition { Position = playerPos.Position + new Vector3(-3f, 0f, 0f) });
            EntityManager.AddComponentData(left, new CTeamAssignment { Team = 0 });

            Entity right = EntityManager.CreateEntity();
            EntityManager.AddComponentData(right, new CCreateAppliance { ID = markerApplianceID });
            EntityManager.AddComponentData(right, new CPosition { Position = playerPos.Position + new Vector3(3f, 0f, 0f) });
            EntityManager.AddComponentData(right, new CTeamAssignment { Team = 1 });

            Debug.Log($"[DEBUGGING] F4: Spawned {playerPos.Position + new Vector3(-1f, 0f, 0f)} and {playerPos.Position + new Vector3(1f, 0f, 0f)}");
        }
    }
}