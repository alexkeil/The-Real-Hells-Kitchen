using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace TestMod.Team.TeamChecks
{
    [HarmonyPatch(typeof(RetrieveBlueprint), "Perform")]
    public static class BlockWrongTeamBlueprintPickUp
    {
        static bool Prefix(ref InteractionData data)
        {
            Mod.Logger.LogInfo("[DEBUGGING] RetrieveBlueprint.Perform fired");
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (!em.HasComponent<CTeamAssignment>(data.Target)) return true; // untagged
            if (!em.HasComponent<CTeamAssignment>(data.Interactor))
            {
                Mod.Logger.LogInfo($"[DEBUGGING] Blocked blueprint pickup — player has no team");
                return false;
            }

            var playerTeam = em.GetComponentData<CTeamAssignment>(data.Interactor);
            var cabinetTeam = em.GetComponentData<CTeamAssignment>(data.Target);

            if (playerTeam.Team != cabinetTeam.Team)
            {
                Mod.Logger.LogInfo($"[DEBUGGING] Blocked blueprint pickup — team mismatch (player: {playerTeam.Team}, cabinet: {cabinetTeam.Team})");
                return false;
            }

            return true;
        }
    }
}