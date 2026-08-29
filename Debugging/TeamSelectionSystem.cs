using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Debugging
{
    public class TeamSelectionSystem : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {


            if (!Input.GetKeyDown(KeyCode.E)) return;

            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Allocator.Temp);

            var markerQuery = GetEntityQuery(typeof(CTeamAssignment), typeof(CPosition));
            var markers = markerQuery.ToEntityArray(Allocator.Temp);

            foreach (var player in players)
            {
                if (!Require(player, out CPosition playerPos)) continue;

                Entity closestMarker = Entity.Null;
                float closestDistance = float.MaxValue;
                CTeamAssignment closestTeam = default;

                foreach (var marker in markers)
                {
                    if (marker == player) continue;
                    if (!Require(marker, out CPosition markerPos)) continue;
                    if (!Require(marker, out CTeamAssignment markerTeam)) continue;

                    float distance = Vector3.Distance(playerPos.Position, markerPos.Position);
                    if (distance < 1.5f && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestMarker = marker;
                        closestTeam = markerTeam;
                    }
                }

                if (closestMarker == Entity.Null)
                {
                    Mod.Logger.LogInfo("[DEBUGGING] E pressed, but not near anything");
                    continue;
                }

                if (Require(player, out CTeamAssignment currentTeam))
                {
                    EntityManager.SetComponentData(player, new CTeamAssignment { Team = closestTeam.Team });
                    Mod.Logger.LogInfo($"[DEBUGGING] Player REASSIGNED from Team {currentTeam.Team} to Team {closestTeam.Team}.");
                }
                else
                {
                    EntityManager.AddComponentData(player, new CTeamAssignment { Team = closestTeam.Team });
                    Mod.Logger.LogInfo($"[DEBUGGING] Player ASSIGNED to Team {closestTeam.Team}.");
                }
            }

            players.Dispose();
            markers.Dispose();

        }
    }
}