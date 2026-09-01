using Kitchen;
using KitchenMods;
using System.Collections.Generic;
using PlateVsPlate.Team;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PlateVsPlate.Settings
{
    public static class PvPStrikeData
    {
        public static int Team0Strikes
        {
            get => TeamData.Get(0).Strikes;
            set => TeamData.Get(0).Strikes = value;
        }
        public static int Team1Strikes
        {
            get => TeamData.Get(1).Strikes;
            set => TeamData.Get(1).Strikes = value;
        }

        public static void Reset() => TeamData.ClearAll();
    }

    public static class PvPStrikeSettings
    {
        public static bool ShowRestartPopup = false;
        public static int StrikesBeforeElimination = 3;
    }

    public struct CTrackedLeaveEvent : IComponentData { }

    public struct CPvPTeamStrikeMarker : IComponentData
    {
        public int Team;
    }

    public static class PvPTableLeaveTracker
    {
        private static readonly Dictionary<Entity, int> PendingLeaves =
            new Dictionary<Entity, int>();

        public static void Track(Entity group, int team)
        {
            if (team != 0 && team != 1)
                return;

            if (PendingLeaves.ContainsKey(group))
                return;

            PendingLeaves[group] = team;

            Mod.Logger.LogInfo($"[DEBUGGING] TRACKED TABLE LEAVE: Group={group.Index} Team={team}");
        }

        public static bool TryConsumeAny(out int team)
        {
            team = -1;

            if (PendingLeaves.Count == 0)
                return false;

            foreach (var pair in PendingLeaves)
            {
                team = pair.Value;

                Mod.Logger.LogInfo($"[DEBUGGING] CONSUMED TABLE LEAVE: Group={pair.Key.Index} Team={team}");

                PendingLeaves.Remove(pair.Key);
                return true;
            }

            return false;
        }

        public static int Count => PendingLeaves.Count;

        public static void Clear()
        {
            PendingLeaves.Clear();
        }
    }

    public class TagLeavingGroupWithTeam : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            var leavingQuery = GetEntityQuery(
                ComponentType.ReadOnly<CGroupStartLeaving>(),
                ComponentType.Exclude<CTrackedLeaveEvent>(),
                ComponentType.Exclude<CPvPTeamStrikeMarker>()
            );
            var leaving = leavingQuery.ToEntityArray(Allocator.Temp);

            if (leaving.Length > 0)
            {
                Mod.Logger.LogInfo($"[DEBUGGING] Found {leaving.Length} leaving group(s) to process this frame");
            }

            foreach (var e in leaving)
            {
                EntityManager.AddComponent<CTrackedLeaveEvent>(e);

                bool hasAssignedTable = Has<CAssignedTable>(e);
                Mod.Logger.LogInfo($"[DEBUGGING] Leaving group {e.Index}, hasAssignedTable={hasAssignedTable}");

                if (!Require(e, out CAssignedTable assignedTable)) continue;

                Entity table = assignedTable.Table;
                Mod.Logger.LogInfo($"[DEBUGGING][STRIKE] Group leaving — CAssignedTable entity Index={table.Index}, Version={table.Version}");

                bool tableHasTeam = Has<CTeamAssignment>(table);
                Mod.Logger.LogInfo($"[DEBUGGING] Table {table.Index} for group {e.Index}, hasTeam={tableHasTeam}");

                if (!Require(table, out CTeamAssignment team)) continue;

                EntityManager.AddComponentData(e, new CPvPTeamStrikeMarker { Team = team.Team });
                PvPTableLeaveTracker.Track(e, team.Team);

                Mod.Logger.LogInfo($"[DEBUGGING] Tagged leaving group {e.Index} with Team {team.Team}");
            }
            leaving.Dispose();
        }
    }

    [UpdateAfter(typeof(TagLeavingGroupWithTeam))]
    [UpdateBefore(typeof(HandleLifeLoseEvent))]
    public class PvPStrikeSystem : RestaurantSystem, IModSystem
    {
        private EntityQuery _loseLifeEvents;

        protected override void Initialise()
        {
            base.Initialise();
            _loseLifeEvents = GetEntityQuery(ComponentType.ReadOnly<CLoseLifeEvent>());
        }

        protected override void OnUpdate()
        {
            int loseLifeCount = _loseLifeEvents.CalculateEntityCount();
            if (loseLifeCount <= 0) return;

            Mod.Logger.LogInfo($"[DEBUGGING] CLoseLifeEvent count={loseLifeCount}");

            int pendingTableLeaves = PvPTableLeaveTracker.Count;

            if (pendingTableLeaves == 0)
            {
                Mod.Logger.LogInfo($"[DEBUGGING] {loseLifeCount} CLoseLifeEvent(s) had no tracked table group. Absorbing (queue failure — no consequence for now).");
                EntityManager.DestroyEntity(_loseLifeEvents);
                return;
            }

            int strikesToProcess = Mathf.Min(loseLifeCount, pendingTableLeaves);
            int actuallyProcessed = 0;

            for (int i = 0; i < strikesToProcess; i++)
            {
                if (!PvPTableLeaveTracker.TryConsumeAny(out int team)) break;

                AddStrikeAndHandle(team);
                actuallyProcessed++;
            }

            if (actuallyProcessed > 0)
            {
                Mod.Logger.LogInfo($"[DEBUGGING] Converted {actuallyProcessed} CLoseLifeEvent(s) into table strike(s).");
                EntityManager.DestroyEntity(_loseLifeEvents);
            }
        }

        public void AddStrikeAndHandle(int team)
        {
            if (team == 0) PvPStrikeData.Team0Strikes++;
            else PvPStrikeData.Team1Strikes++;

            int currentStrikes = team == 0 ? PvPStrikeData.Team0Strikes : PvPStrikeData.Team1Strikes;

            Mod.Logger.LogInfo($"[DEBUGGING] Team {team} table strike #{currentStrikes}");

            if (currentStrikes < PvPStrikeSettings.StrikesBeforeElimination)
            {
                if (PvPStrikeSettings.ShowRestartPopup)
                {
                    World.Add(new COfferRestartDay { Reason = LossReason.Patience });
                    Mod.Logger.LogInfo($"[DEBUGGING] Team {team} strike {currentStrikes}/{PvPStrikeSettings.StrikesBeforeElimination} — offering restart (popup)");
                }
                else
                {
                    World.Add<CRestartDayEvent>();
                    Mod.Logger.LogInfo($"[DEBUGGING] Team {team} strike {currentStrikes}/{PvPStrikeSettings.StrikesBeforeElimination} — forcing restart");
                }
            }
            else
            {
                Mod.Logger.LogInfo($"[DEBUGGING] Team {team} reached {PvPStrikeSettings.StrikesBeforeElimination} strikes — triggering real game over.");

                var em = World.DefaultGameObjectInjectionWorld.EntityManager;
                var entity = em.CreateEntity(typeof(SGameOver), typeof(CGamePauseBlock));
                em.SetComponentData(entity, new SGameOver { Reason = LossReason.Patience });

                PvPStrikeData.Reset();
                TeamData.ClearAll();
            }
        }
    }

    public class DebugAddStrike : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            var strikeSystem = World.GetExistingSystem<PvPStrikeSystem>();
            if (strikeSystem == null) return;

            if (Input.GetKeyDown(KeyCode.F5)) strikeSystem.AddStrikeAndHandle(0);
            if (Input.GetKeyDown(KeyCode.F6)) strikeSystem.AddStrikeAndHandle(1);
        }
    }

    public class DebugAdjustPatience : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.Home) && !Input.GetKeyDown(KeyCode.End)) return;

            var playerQuery = GetEntityQuery(ComponentType.ReadOnly<CPlayer>(), ComponentType.ReadOnly<CPosition>());
            var players = playerQuery.ToEntityArray(Allocator.Temp);
            if (players.Length == 0) { players.Dispose(); return; }
            if (!Require(players[0], out CPosition playerPos)) { players.Dispose(); return; }
            players.Dispose();

            var query = GetEntityQuery(
                ComponentType.ReadWrite<CPatience>(),
                ComponentType.ReadOnly<CPosition>(),
                ComponentType.Exclude<CGroupStartLeaving>()
            );
            var entities = query.ToEntityArray(Allocator.Temp);

            Entity closest = Entity.Null;
            float closestDist = float.MaxValue;

            foreach (var e in entities)
            {
                if (!Require(e, out CPosition pos)) continue;
                float dist = Vector3.Distance(playerPos.Position, pos.Position);
                if (dist < closestDist) { closestDist = dist; closest = e; }
            }

            if (closest != Entity.Null && Require(closest, out CPatience patience))
            {
                float delta = Input.GetKeyDown(KeyCode.Home) ? 1f : -1f;
                patience.RemainingTime = Mathf.Max(0.1f, patience.RemainingTime + delta);
                Set(closest, patience);

                Mod.Logger.LogInfo($"[DEBUGGING] Patience adjusted by {delta:+0.0;-0.0} seconds. Remaining={patience.RemainingTime:0.00}");
            }

            entities.Dispose();
        }
    }

    /*
    public class StrikeDisplayBehaviour : MonoBehaviour
    {
        public static string Team0Text = "";
        public static string Team1Text = "";

        private GUIStyle _style;

        private void OnGUI()
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label) { fontSize = 24, normal = { textColor = Color.white } };
            }

            GUI.Label(new Rect(20, 20, 300, 40), $"Team 0 Strikes: {Team0Text}", _style);
            GUI.Label(new Rect(20, 60, 300, 40), $"Team 1 Strikes: {Team1Text}", _style);
        }
    }
    */

    /*
    public class StrikeDisplaySystem : GenericSystemBase, IModSystem
    {
        private static bool _created = false;

        protected override void OnUpdate()
        {
            if (!_created)
            {
                var go = new GameObject("TestMod_StrikeDisplay");
                go.AddComponent<StrikeDisplayBehaviour>();
                UnityEngine.Object.DontDestroyOnLoad(go);
                _created = true;
            }

            StrikeDisplayBehaviour.Team0Text = new string('X', PvPStrikeData.Team0Strikes)
                + new string('_', Mathf.Max(0, PvPStrikeSettings.StrikesBeforeElimination - PvPStrikeData.Team0Strikes));

            StrikeDisplayBehaviour.Team1Text = new string('X', PvPStrikeData.Team1Strikes)
                + new string('_', Mathf.Max(0, PvPStrikeSettings.StrikesBeforeElimination - PvPStrikeData.Team1Strikes));
        }
    }
    */
}