using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestMod.RestaurantSetup.InitialSetup
{
    public class DuplicateProvidersForBothTeams : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Has<SIsDayTime>() && !Has<SIsNightTime>()) return; // excludes lobby

            var query = GetEntityQuery(
                ComponentType.ReadOnly<CItemProvider>(),
                ComponentType.ReadOnly<CPosition>(),
                ComponentType.ReadOnly<CAppliance>(),
                ComponentType.Exclude<CTeamAssignment>()
            );

            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var original in entities)
            {
                if (!Require(original, out CItemProvider provider)) continue;
                if (!Require(original, out CPosition pos)) continue;
                if (!Require(original, out CAppliance appliance)) continue;

                if (!Data.TryGet<Item>(provider.ProvidedItem, out _, false)) continue;

                var mirroredPos = pos;
                mirroredPos.Position = new Vector3(-pos.Position.x, pos.Position.y, pos.Position.z);

                if (MapBoundsInfo.HasBounds)
                    if (mirroredPos.Position.x < MapBoundsInfo.MinX || mirroredPos.Position.x > MapBoundsInfo.MaxX)
                        continue;

                Entity copy = EntityManager.CreateEntity();
                EntityManager.AddComponentData(copy, new CCreateAppliance { ID = appliance.ID });
                EntityManager.AddComponentData(copy, mirroredPos);
                EntityManager.AddComponentData(copy, new CTeamAssignment { Team = 1 });

                EntityManager.AddComponentData(original, new CTeamAssignment { Team = 0 });

                Mod.Logger.LogInfo($"[DEBUGGING] Mirrored provider {appliance.ID} to {mirroredPos.Position}");
            }

            entities.Dispose();

            var tableQuery = GetEntityQuery(
                ComponentType.ReadOnly<CApplianceTable>(),
                ComponentType.ReadOnly<CPosition>(),
                ComponentType.Exclude<CTeamAssignment>()
            );

            var tables = tableQuery.ToEntityArray(Allocator.Temp);
            foreach (var table in tables)
            {
                if (!Require(table, out CPosition tablePos)) continue;

                if (Mathf.Abs(tablePos.Position.x) < 0.5f)
                    Mod.Logger.LogInfo($"[DEBUGGING] Table {table.Index} close to center (x={tablePos.Position.x})");

                int team = tablePos.Position.x < 0 ? 0 : 1;
                EntityManager.AddComponentData(table, new CTeamAssignment { Team = team });
                Mod.Logger.LogInfo($"[DEBUGGING] Tagged table {table.Index} as Team {team}");
            }

            tables.Dispose();

        }
    }
}
