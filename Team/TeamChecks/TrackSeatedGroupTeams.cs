using System.Collections.Generic;
using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace TestMod.Team.TeamChecks
{
    public class TrackSeatedGroupTeams : GenericSystemBase, IModSystem
    {
        private static readonly Dictionary<int, int> _lastKnownTeam = new Dictionary<int, int>();

        protected override void OnUpdate()
        {
            var query = GetEntityQuery(
                ComponentType.ReadOnly<CAssignedTable>(),
                ComponentType.ReadOnly<CCustomerGroup>()
            );
            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var e in entities)
            {
                if (!Require(e, out CAssignedTable assignedTable)) continue;

                Entity table = assignedTable.Table;
                if (!RequireBuffer(table, out DynamicBuffer<CTableSetParts> parts)) continue;

                int teamNumber = -1;
                foreach (var part in parts)
                {
                    Entity partEntity = part.Entity;
                    if (!EntityManager.Exists(partEntity)) continue;
                    if (Require(partEntity, out CTeamAssignment team))
                    {
                        teamNumber = team.Team;
                        break;
                    }
                }

                if (teamNumber != 0 && teamNumber != 1) continue;

                if (!_lastKnownTeam.ContainsKey(e.Index))
                {
                    Mod.Logger.LogInfo($"[DEBUGGING] Caching group {e.Index} with Team {teamNumber}");
                }
                _lastKnownTeam[e.Index] = teamNumber;
            }
            entities.Dispose();

            var leavingQuery = GetEntityQuery(
                ComponentType.ReadOnly<CGroupLeaving>(),
                ComponentType.Exclude<CTrackedLeaveEvent>()
            );
            var leaving = leavingQuery.ToEntityArray(Allocator.Temp);

            foreach (var e in leaving)
            {
                EntityManager.AddComponent<CTrackedLeaveEvent>(e);

                if (_lastKnownTeam.TryGetValue(e.Index, out int team))
                {
                    PvPTableLeaveTracker.Track(e, team);
                    Mod.Logger.LogInfo($"[DEBUGGING] Group {e.Index} leaving — Team {team} (table strike)");
                    _lastKnownTeam.Remove(e.Index);
                }
                else
                {
                    Mod.Logger.LogInfo($"[DEBUGGING] Group {e.Index} leaving — no cached team (queue/door, ignored)");
                }
            }
            leaving.Dispose();
        }
    }
}