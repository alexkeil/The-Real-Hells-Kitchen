using HarmonyLib;
using Kitchen;
using KitchenData;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Team.TeamChecks
{
    [HarmonyPatch(typeof(PickUpAndDropAppliance), "PerformPickUp")]
    public static class BlockWrongTeamPickUp
    {
        static bool Prefix(EntityContext ctx, Entity player, ref CAttemptingInteraction interact, in CPosition pos, bool should_act, OccupancyLayer layer, ref bool __result)
        {
            if (!should_act) return true;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var target = interact.Target;

            if (!em.HasComponent<CTeamAssignment>(target))
            {
                Mod.Logger.LogInfo($"[DEBUGGING] Target entity {target.Index} has NO CTeamAssignment — letting through");
                return true;
            }

            if (!em.HasComponent<CTeamAssignment>(player))
            {
                Mod.Logger.LogInfo($"[DEBUGGING] Blocked pickup — player has no team");
                __result = false;
                return false;
            }

            var playerTeam = em.GetComponentData<CTeamAssignment>(player);
            var targetTeam = em.GetComponentData<CTeamAssignment>(target);

            if (playerTeam.Team != targetTeam.Team)
            {
                Mod.Logger.LogInfo($"[DEBUGGING] Blocked pickup — team mismatch (player: {playerTeam.Team}, item: {targetTeam.Team})");
                __result = false;
                return false;
            }

            return true; // same team — allow
        }
    }

    [HarmonyPatch(typeof(PickUpAndDropAppliance), "PerformDrop")]
    public static class BlockWrongTeamDrop
    {
        static bool Prefix(EntityContext ctx, Entity player, ref CAttemptingInteraction interact, CItemHolder player_holder, CPosition pos, bool should_act, ref bool __result)
        {
            if (!should_act) return true;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var heldItem = player_holder.HeldItem;

            if (!em.HasComponent<CTeamAssignment>(heldItem)) return true;

            if (!em.HasComponent<CTeamAssignment>(player))
            {
                __result = false;
                return false;
            }

            var playerTeam = em.GetComponentData<CTeamAssignment>(player);
            var itemTeam = em.GetComponentData<CTeamAssignment>(heldItem);

            if (playerTeam.Team != itemTeam.Team)
            {
                Mod.Logger.LogInfo($"[DEBUGGING] Blocked drop — team mismatch");
                __result = false;
                return false;
            }

            return true;
        }
    }
}