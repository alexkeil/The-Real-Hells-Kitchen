using HarmonyLib;
using Kitchen;
using KitchenData;
using Unity.Entities;

namespace PlateVsPlate.Team.TeamChecks
{
    [HarmonyPatch(typeof(PickUpAndDropAppliance), "PerformPickUp")]
    public static class BlockWrongTeamPickUp
    {
        static bool Prefix(EntityContext ctx, Entity player, ref CAttemptingInteraction interact, in CPosition pos, bool should_act, OccupancyLayer layer, ref bool __result)
        {
            if (!should_act) return true;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var target = interact.Target;

            if (!em.HasComponent<CTeamAssignment>(target)) return true;

            if (!em.HasComponent<CTeamAssignment>(player))
            {
                __result = false;
                return false;
            }

            var playerTeam = em.GetComponentData<CTeamAssignment>(player);
            var targetTeam = em.GetComponentData<CTeamAssignment>(target);

            if (playerTeam.Team != targetTeam.Team)
            {
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
                __result = false;
                return false;
            }

            return true;
        }
    }
}