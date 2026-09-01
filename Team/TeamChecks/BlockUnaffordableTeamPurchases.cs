using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace PlateVsPlate.Team.TeamChecks
{
    public class BlockUnaffordableTeamPurchases : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            var query = GetEntityQuery(
                ComponentType.ReadOnly<CForSale>(),
                ComponentType.ReadOnly<CPurchaseAfterDuration>(),
                ComponentType.ReadOnly<CBeingActedOnBy>()
            );
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var e in entities)
            {
                if (!Require(e, out CForSale sale)) continue;
                if (!Require(e, out DynamicBuffer<CBeingActedOnBy> actors)) continue;
                if (actors.Length == 0) continue;

                Entity interactor = actors[0].Interactor;
                if (!Require(interactor, out CTeamAssignment interactorTeam)) continue;

                bool shouldBlock = false;

                if (Require(e, out CTeamAssignment itemTeam) && itemTeam.Team != interactorTeam.Team)
                    shouldBlock = true;

                var teamData = TeamData.Get(interactorTeam.Team);
                bool canAfford = teamData.Balance >= sale.Price;
                if (!canAfford) shouldBlock = true;

                bool isInactive = Has<CIsInactive>(e);

                if (shouldBlock && !isInactive)
                    EntityManager.AddComponent<CIsInactive>(e);
                else if (!shouldBlock && isInactive)
                    EntityManager.RemoveComponent<CIsInactive>(e);

            }
            entities.Dispose();
        }
    }
}