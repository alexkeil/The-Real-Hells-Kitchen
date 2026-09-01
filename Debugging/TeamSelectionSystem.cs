using Kitchen;
using KitchenMods;
using PlateVsPlate.Team;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PlateVsPlate.Debugging
{
    public class TeamSelectionSystem : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Allocator.Temp);

            var markerQuery = GetEntityQuery(typeof(CTeamSelector), typeof(CPosition));
            var markers = markerQuery.ToEntityArray(Allocator.Temp);

            foreach (var player in players)
            {
                if (!Require(player, out CPosition playerPos)) continue;

                Entity closestMarker = Entity.Null;
                float closestDistance = float.MaxValue;
                CTeamSelector closestMarkerData = default;

                foreach (var marker in markers)
                {
                    if (marker == player) continue;
                    if (!Require(marker, out CPosition markerPos)) continue;
                    if (!Require(marker, out CTeamSelector markerData)) continue;

                    float distance = Vector3.Distance(playerPos.Position, markerPos.Position);
                    if (distance < 1.5f && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestMarker = marker;
                        closestMarkerData = markerData;
                    }
                }

                if (closestMarker == Entity.Null) continue;

                if (Has<CTeamAssignment>(player))
                    EntityManager.SetComponentData(player, new CTeamAssignment { Team = closestMarkerData.Team });
                else
                    EntityManager.AddComponentData(player, new CTeamAssignment { Team = closestMarkerData.Team });
            }

            players.Dispose();
            markers.Dispose();
        }
    }
}