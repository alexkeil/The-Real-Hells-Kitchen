using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace PlateVsPlate.Team.TeamChecks
{
    [HarmonyPatch(typeof(RetrieveBlueprint), "Perform")]
    public static class BlockWrongTeamBlueprintPickUp
    {
        static bool Prefix(ref InteractionData data)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (!em.HasComponent<CTeamAssignment>(data.Target)) return true; 
            if (!em.HasComponent<CTeamAssignment>(data.Interactor)) return false;

            var playerTeam = em.GetComponentData<CTeamAssignment>(data.Interactor);
            var targetTeam = em.GetComponentData<CTeamAssignment>(data.Target);

            if (playerTeam.Team != targetTeam.Team) return false;

            return true;
        }
    }
}