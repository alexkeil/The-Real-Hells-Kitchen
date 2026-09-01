using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace PlateVsPlate.Team.TeamChecks
{
    // note.. ideally try to fix this somehow.. you get a visual glitch when you try to buy it.. can't buy but still
    [HarmonyPatch(typeof(PurchaseAfterDuration), "OnUpdate")]
    public static class InterceptDurationPurchaseMoney
    {
        static void Prefix()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return;
            var em = world.EntityManager;

            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<CForSale>(),
                ComponentType.ReadOnly<CPurchaseAfterDuration>(),
                ComponentType.ReadOnly<CBeingActedOnBy>()
            );

            var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            foreach (var e in entities)
            {
                if (!em.HasComponent<CBeingActedOnBy>(e)) continue;
                var actors = em.GetBuffer<CBeingActedOnBy>(e);
                if (actors.Length == 0) continue;

                Entity interactor = actors[0].Interactor;
                if (!em.HasComponent<CTeamAssignment>(interactor)) continue;
                if (!em.HasComponent<CForSale>(e)) continue;

                var team = em.GetComponentData<CTeamAssignment>(interactor);
                var sale = em.GetComponentData<CForSale>(e);
                PendingPurchases.Entries[e.Index] = (team.Team, sale.Price);
            }
            entities.Dispose();
        }
    }
}