using HarmonyLib;
using Kitchen;
using KitchenMods;
using System.Collections.Generic;
using PlateVsPlate.Settings;
using PlateVsPlate.Setttings;
using Unity.Collections;
using Unity.Entities;

namespace PlateVsPlate.Team.TeamChecks
{
    public class TrackSeatedGroupTeams : GenericSystemBase, IModSystem
    {
        private static readonly Dictionary<int, int> _lastKnownTeam = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> _lastPreferredTeam = new Dictionary<int, int>();

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
                Entity tableSet = assignedTable.Table;
                if (!RequireBuffer(tableSet, out DynamicBuffer<CTableSetParts> parts)) continue;

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
                    int groupSize = 1;
                    if (RequireBuffer(e, out DynamicBuffer<CGroupMember> members))
                        groupSize = members.Length;

                    CustomerTeamBalanceTracker.RecordSeated(teamNumber, groupSize);
                }
                _lastKnownTeam[e.Index] = teamNumber;
                _lastPreferredTeam.Remove(e.Index); 
            }
            entities.Dispose();

            var waitingQuery = GetEntityQuery(
                ComponentType.ReadOnly<CCustomerGroup>(),
                ComponentType.Exclude<CAssignedTable>()
            );
            var waiting = waitingQuery.ToEntityArray(Allocator.Temp);
            foreach (var e in waiting)
            {
                if (_lastKnownTeam.ContainsKey(e.Index)) continue; 
                _lastPreferredTeam[e.Index] = CustomerTeamBalanceTracker.PreferredTeam();
            }
            waiting.Dispose();

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
                    _lastKnownTeam.Remove(e.Index);
                }
                else if (_lastPreferredTeam.TryGetValue(e.Index, out int preferredTeam))
                {
                    PvPTableLeaveTracker.Track(e, preferredTeam);
                    _lastPreferredTeam.Remove(e.Index);
                }
            }
            leaving.Dispose();
        }
    }

    public static class CustomerTeamBalanceTracker
    {
        public static int Team0Groups
        {
            get => TeamData.Get(0).GroupsSeated;
            set => TeamData.Get(0).GroupsSeated = value;
        }
        public static int Team1Groups
        {
            get => TeamData.Get(1).GroupsSeated;
            set => TeamData.Get(1).GroupsSeated = value;
        }
        public static int Team0Customers
        {
            get => TeamData.Get(0).CustomersSeated;
            set => TeamData.Get(0).CustomersSeated = value;
        }
        public static int Team1Customers
        {
            get => TeamData.Get(1).CustomersSeated;
            set => TeamData.Get(1).CustomersSeated = value;
        }

        public static void RecordSeated(int team, int customerCount) =>
            TeamData.Get(team).RecordSeated(customerCount);

        public static int PreferredTeam()
        {
            bool byCustomerCount = PvPModSettings.GetBalanceByCustomerCount();
            int team0Value = byCustomerCount ? Team0Customers : Team0Groups;
            int team1Value = byCustomerCount ? Team1Customers : Team1Groups;
            return team0Value <= team1Value ? 0 : 1;
        }
    }

    [HarmonyPatch]
    internal static class LocationComparer_TeamBalance_Patch
    {
        [HarmonyPatch(typeof(LocationComparer), nameof(LocationComparer.Compare))]
        [HarmonyPrefix]
        public static bool Compare_Prefix(ref int __result, CAvailableAssignment x, CAvailableAssignment y)
        {
            int preferredTeam = CustomerTeamBalanceTracker.PreferredTeam();

            int? xTeam = GetTableTeam(x.Entity);
            int? yTeam = GetTableTeam(y.Entity);

            if (xTeam == null || yTeam == null || xTeam == yTeam)
                return true; // can't tell, or same team — let vanilla decide

            bool xIsPreferred = xTeam.Value == preferredTeam;
            bool yIsPreferred = yTeam.Value == preferredTeam;

            if (xIsPreferred && !yIsPreferred)
            {
                __result = -1;
                return false;
            }
            if (yIsPreferred && !xIsPreferred)
            {
                __result = 1;
                return false;
            }
            return true;
        }

        private static int? GetTableTeam(Entity groupEntity)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return null;
            var em = world.EntityManager;

            if (!em.HasComponent<CTableSetParts>(groupEntity)) return null;
            var parts = em.GetBuffer<CTableSetParts>(groupEntity);
            if (parts.Length == 0) return null;

            Entity tableEntity = parts[0].Entity;
            if (!em.Exists(tableEntity)) return null;
            if (!em.HasComponent<CTeamAssignment>(tableEntity)) return null;

            return em.GetComponentData<CTeamAssignment>(tableEntity).Team;
        }
    }
}
