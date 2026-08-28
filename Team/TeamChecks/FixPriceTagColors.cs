using Kitchen;
using KitchenMods;
using TestMod.Team;
using TestMod;
using Unity.Entities;

public class FixPriceTagColors : GenericSystemBase, IModSystem
{
    protected override void OnUpdate()
    {
        var query = GetEntityQuery(
            ComponentType.ReadOnly<CApplianceInfo>(),
            ComponentType.ReadOnly<CTeamAssignment>()
        );
        var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

        foreach (var e in entities)
        {
            if (!Require(e, out CApplianceInfo info)) continue;
            if (!Require(e, out CTeamAssignment team)) continue;
            if (info.Mode != CApplianceInfo.ApplianceInfoMode.Shop) continue;

            var teamData = TeamMoney.Get(team.Team);
            bool canAfford = teamData.Balance >= info.Price;

            // TODO: find the actual rendered GameObject/TextMeshPro for this entity's
            // price tag and set its color directly — this needs the real view lookup,
            // which we haven't confirmed how to reach from ECS-side code yet.
        }
        entities.Dispose();
    }
}