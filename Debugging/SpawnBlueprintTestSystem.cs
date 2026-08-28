using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Debugging
{
    public class SpawnBlueprintTestSystem : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F7)) return;

            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Allocator.Temp);

            if (players.Length == 0)
            {
                Debug.Log("[DEBUGGING] F7: no player entity found.");
                players.Dispose();
                return;
            }

            Entity playerEntity = players[0];
            players.Dispose();

            int testApplianceID = 1139247360; // temp item

            Entity blueprint = EntityManager.CreateEntity();
            EntityManager.AddComponentData(blueprint, new CCreateAppliance { ID = AssetReference.Blueprint });

            if (Require(playerEntity, out CPosition playerPos))
                EntityManager.AddComponentData(blueprint, playerPos);

            EntityManager.AddComponentData(blueprint, new CApplianceBlueprint
            {
                Appliance = testApplianceID,
                IsCopy = false
            });
            EntityManager.AddComponentData(blueprint, new CForSale { Price = 0 });
            EntityManager.AddComponentData(blueprint, default(CShopEntity));
            EntityManager.AddComponent<CHeldAppliance>(blueprint);
            EntityManager.AddComponentData(blueprint, new CHeldBy { Holder = playerEntity });
            EntityManager.AddComponentData(playerEntity, new CItemHolder { HeldItem = blueprint });

            Debug.Log($"[DEBUGGING] Spawned test (appliance {testApplianceID}) hand.");
        }
    }
}
