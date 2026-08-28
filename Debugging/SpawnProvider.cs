using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Debugging
{
    public class SpawnProvider : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F9)) return;

            int onionProviderID = -2042103798;

            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            CPosition playerPos = default;
            if (players.Length > 0) Require(players[0], out playerPos);
            players.Dispose();

            Entity e = EntityManager.CreateEntity();
            EntityManager.AddComponentData(e, new CCreateAppliance { ID = onionProviderID });
            EntityManager.AddComponentData(e, playerPos);

            Debug.Log($"[DEBUGGING] Created ONE entity {e.Index} for onion provider at {playerPos.Position}.");
        }
    }
}
