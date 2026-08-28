using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;


namespace TestMod.Debugging
{
    public struct CAlreadyDuplicatedForTeams : IComponentData, IModComponent { }

    public class SpawnItemCrateAndItem : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F10)) return;

            int parcelApplianceID = -1936421857; 
            int onionItemID = -201067776; // temp ID

            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            
            CPosition playerPos = default;
            if (players.Length > 0) Require(players[0], out playerPos);
                players.Dispose();

            Entity e = EntityManager.CreateEntity();
            EntityManager.AddComponentData(e, new CCreateAppliance { ID = parcelApplianceID });
            EntityManager.AddComponentData(e, playerPos);
            EntityManager.AddComponentData(e, new CLetterIngredient { IngredientID = onionItemID });

            Debug.Log("[DEBUGGING] Spawned test parcel.");
        }
    }
}
