using Kitchen;
using KitchenData;
using KitchenMods;
using System.Collections.Generic;
using PlateVsPlate.Team;
using PlateVsPlate.Team.TeamChecks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using TestMod.RestaurantSetup.InitialSetup;

namespace PlateVsPlate.RestaurantSetup.InitialSetup
{
    public class DuplicateTeamAppliances : GenericSystemBase, IModSystem
    {
        private static readonly HashSet<string> IgnoredAppliances = new HashSet<string> { "Bench" };
        private static readonly HashSet<Entity> _processedTables = new HashSet<Entity>();
        private static readonly HashSet<Entity> _processedProviders = new HashSet<Entity>();
        private static readonly List<Entity> _pendingTeam0 = new List<Entity>();
        private static readonly List<Entity> _pendingTeam1 = new List<Entity>();

        private const float DuplicateOffsetX = 2f;
        private const float BoundsMargin = 3f;

        public static void ResetDuplicationState()
        {
            _processedTables.Clear();
            _processedProviders.Clear();
            _pendingTeam0.Clear();
            _pendingTeam1.Clear();
        }

        protected override void OnUpdate()
        {
            if (!Has<SIsDayTime>() && !Has<SIsNightTime>()) return;
            DuplicateTables();
            DuplicateProviders();
            ResolvePendingTags();
        }

        private float GetSafeOffsetDirection(float originalX)
        {
            if (!MapBoundsInfo.HasBounds) return 1f;
            float mapCenterX = (MapBoundsInfo.MinX + MapBoundsInfo.MaxX) / 2f;
            return originalX < mapCenterX ? -1f : 1f;
        }

        private bool IsWithinBounds(float x)
        {
            if (!MapBoundsInfo.HasBounds) return true;
            return x >= MapBoundsInfo.MinX - BoundsMargin && x <= MapBoundsInfo.MaxX + BoundsMargin;
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
                if (_processedTables.Contains(original)) continue;
                if (!Require(original, out CAppliance appliance)) continue;
                if (!Require(original, out CPosition position)) continue;
                if (!Data.TryGet<Appliance>(appliance.ID, out var applianceData, false)) continue;
                if (IgnoredAppliances.Contains(applianceData.Name)) continue;

                float direction = GetSafeOffsetDirection(position.Position.x);
                var duplicatePosition = position;
                duplicatePosition.Position += new Vector3(DuplicateOffsetX * direction, 0f, 0f);
                if (!IsWithinBounds(duplicatePosition.Position.x)) continue;

                _processedTables.Add(original);
                EntityManager.AddComponentData(original, new CTeamAssignment { Team = 0 });
                EntityManager.AddComponentData(original, new CTeamMarker { Team = 0 });

                Entity copy = EntityManager.CreateEntity();
                EntityManager.AddComponentData(copy, new CCreateAppliance { ID = appliance.ID });
                EntityManager.AddComponentData(copy, duplicatePosition);
                _pendingTeam1.Add(copy);
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
                if (!Require(original, out CItemProvider provider)) continue;
                if (!Require(original, out CAppliance appliance)) continue;
                if (!Require(original, out CPosition position)) continue;
                if (_processedProviders.Contains(original)) continue;
                if (!Data.TryGet<Appliance>(appliance.ID, out var applianceData, false)) continue;
                if (IgnoredAppliances.Contains(applianceData.Name)) continue;

                float direction = GetSafeOffsetDirection(position.Position.x);
                var duplicatePosition = position;
                duplicatePosition.Position += new Vector3(DuplicateOffsetX * direction, 0f, 0f);
                if (!IsWithinBounds(duplicatePosition.Position.x)) continue;

                _processedProviders.Add(original);
                EntityManager.AddComponentData(original, new CTeamAssignment { Team = 0 });
                EntityManager.AddComponentData(original, new CTeamMarker { Team = 0 });

                Entity copy = EntityManager.CreateEntity();
                EntityManager.AddComponentData(copy, new CCreateAppliance { ID = appliance.ID });
                EntityManager.AddComponentData(copy, duplicatePosition);
                EntityManager.AddComponentData(copy, provider);
                _pendingTeam1.Add(copy);
            }
            providers.Dispose();
        }

        private void ResolvePendingTags()
        {
            ResolveList(_pendingTeam0, 0);
            ResolveList(_pendingTeam1, 1);
        }

        public static void QueuePendingTag(Entity e, int team)
        {
            if (team == 0) _pendingTeam0.Add(e);
            else _pendingTeam1.Add(e);
        }

        private void ResolveList(List<Entity> pending, int team)
        {
            for (int i = pending.Count - 1; i >= 0; i--)
            {
                var e = pending[i];
                if (!EntityManager.Exists(e)) { pending.RemoveAt(i); continue; }

                if (EntityManager.HasComponent<CAppliance>(e))
                {
                    EntityManager.AddComponentData(e, new CTeamAssignment { Team = team });
                    EntityManager.AddComponentData(e, new CTeamMarker { Team = team });
                    pending.RemoveAt(i);
                }
            }
        }
    } 
}