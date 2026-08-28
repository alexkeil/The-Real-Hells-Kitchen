using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Debugging
{
    public class DebugShowLookTarget : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F6)) return;

            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CAttemptingInteraction), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Allocator.Temp);

            foreach (var player in players)
            {
                if (!Require(player, out CAttemptingInteraction attempt)) continue;
                if (!Require(player, out CPosition playerPos)) continue;

                Entity target = attempt.Target;
                string teamInfo = Has<CTeamAssignment>(target)
                    ? $"Team {EntityManager.GetComponentData<CTeamAssignment>(target).Team}"
                    : "no team tag";
                string applianceInfo = Require(target, out CAppliance app)
                    ? $"Appliance ID {app.ID}"
                    : "not an appliance";

                Debug.Log($"[DEBUGGING] Looking at entity {target.Index} — Type: {attempt.Type}, {applianceInfo}, {teamInfo}");

                // Also scan for anything nearby, in case the look-target check missed something
                var query = GetEntityQuery(typeof(CAppliance), typeof(CPosition));
                var entities = query.ToEntityArray(Allocator.Temp);

                foreach (var e in entities)
                {
                    if (!Require(e, out CPosition pos)) continue;
                    if (!Require(e, out CAppliance nearbyApp)) continue;

                    float dist = Vector3.Distance(playerPos.Position, pos.Position);
                    if (dist < 3f)
                    {
                        Debug.Log($"[DEBUGGING] Nearby: entity {e.Index}, Appliance ID {nearbyApp.ID}, distance {dist:F1}");
                    }
                }

                entities.Dispose();
            }

            players.Dispose();
        }
    }
}