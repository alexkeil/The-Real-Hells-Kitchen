using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Debugging
{
    public class SpawnTwoItemsTestSystem : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F8)) return;
            int onionID = -201067776; // temp ID
            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

            Debug.Log($"[DEBUGGING] Found {players.Length} player entities.");

            CPosition playerPos = default;
            if (players.Length > 0)
            {
                bool found = Require(players[0], out playerPos);
                Debug.Log($"[DEBUGGING] CPosition success: {found}, value: {playerPos.Position}");
            }
            players.Dispose();

            for (int i = 0; i < 2; i++)
            {
                Entity e = EntityManager.CreateEntity();
                EntityManager.AddComponentData(e, new CCreateItem
                {
                    ID = onionID,
                    Holder = default(Entity)
                });
                EntityManager.AddComponentData(e, playerPos);
                Debug.Log($"[DEBUGGING] Created entity {e.Index} with CCreateItem + CPosition.");
            }
            Debug.Log("[DEBUGGING] Requested 2.");
        }
    }
}
