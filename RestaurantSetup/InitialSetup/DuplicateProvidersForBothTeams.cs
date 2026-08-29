using System.Collections.Generic;
using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestMod.RestaurantSetup.InitialSetup
{
    public class DuplicateTeamAppliances : GenericSystemBase, IModSystem
    {
        private static readonly HashSet<string> IgnoredAppliances =
            new HashSet<string>
            {
                "Bench",
            };


        private const float DuplicateOffsetX = 2f;

        protected override void OnUpdate()
        {

            if (!Has<SIsDayTime>() && !Has<SIsNightTime>())
                return;

            DuplicateTables();
            DuplicateProviders();
        }

        private void DuplicateTables()
        {
            var tableQuery = GetEntityQuery(
                ComponentType.ReadOnly<CApplianceTable>(),
                ComponentType.ReadOnly<CAppliance>(),
                ComponentType.ReadOnly<CPosition>(),
                ComponentType.Exclude<CTeamAssignment>()
            );

            var tables = tableQuery.ToEntityArray(Allocator.Temp);

            foreach (var original in tables)
            {
                if (!Require(original, out CAppliance appliance))
                    continue;

                if (!Require(original, out CPosition position))
                    continue;

                if (!Data.TryGet<Appliance>(
                        appliance.ID,
                        out var applianceData,
                        false))
                {
                    Mod.Logger.LogInfo($"[DEBUGGING] Could not find Appliance data for table {original.Index}, ID={appliance.ID}");
                    continue;
                }


                string applianceName = applianceData.Name;

                if (IgnoredAppliances.Contains(applianceName))
                {
                    Mod.Logger.LogInfo($"[DEBUGGING] Ignoring TABLE '{applianceName}'(Entity={original.Index})");
                    continue;
                }

                EntityManager.AddComponentData(
                    original,
                    new CTeamAssignment
                    {
                        Team = 0
                    }
                );

                var duplicatePosition = position;

                duplicatePosition.Position +=
                    new Vector3(
                        DuplicateOffsetX,
                        0f,
                        0f
                    );


                Entity copy = EntityManager.CreateEntity();
                EntityManager.AddComponentData(
                    copy,
                    new CCreateAppliance
                    {
                        ID = appliance.ID
                    }
                );

                EntityManager.AddComponentData(
                    copy,
                    duplicatePosition
                );

                EntityManager.AddComponentData(
                    copy,
                    new CTeamAssignment
                    {
                        Team = 1
                    }
                );

            }

            tables.Dispose();
        }

        private void DuplicateProviders()
        {
            var providerQuery = GetEntityQuery(
                ComponentType.ReadOnly<CItemProvider>(),
                ComponentType.ReadOnly<CAppliance>(),
                ComponentType.ReadOnly<CPosition>(),
                ComponentType.Exclude<CTeamAssignment>()
            );

            var providers = providerQuery.ToEntityArray(Allocator.Temp);

            foreach (var original in providers)
            {
                if (!Require(original, out CItemProvider provider))
                    continue;

                if (!Require(original, out CAppliance appliance))
                    continue;

                if (!Require(original, out CPosition position))
                    continue;

                if (!Data.TryGet<Appliance>(
                        appliance.ID,
                        out var applianceData,
                        false))
                {
                    Mod.Logger.LogInfo(
                        $"[DEBUGGING] Could not find Appliance data for " +
                        $"provider Entity={original.Index}, ID={appliance.ID}"
                    );

                    continue;
                }

                string applianceName = applianceData.Name;

                if (IgnoredAppliances.Contains(applianceName))
                {
                    Mod.Logger.LogInfo($"[DEBUGGING] Ignoring PROVIDER '{applianceName}'(Entity={original.Index})");
                    continue;
                }

                EntityManager.AddComponentData(
                    original,
                    new CTeamAssignment
                    {
                        Team = 0
                    }
                );

                var duplicatePosition = position;

                duplicatePosition.Position +=
                    new Vector3(
                        DuplicateOffsetX,
                        0f,
                        0f
                    );

                Entity copy = EntityManager.CreateEntity();
                EntityManager.AddComponentData(
                    copy,
                    new CCreateAppliance
                    {
                        ID = appliance.ID
                    }
                );

                EntityManager.AddComponentData(
                    copy,
                    duplicatePosition
                );

                EntityManager.AddComponentData(
                    copy,
                    new CTeamAssignment
                    {
                        Team = 1
                    }
                );

                EntityManager.AddComponentData(
                    copy,
                    provider
                );
            }
            providers.Dispose();
        }
    }
}

