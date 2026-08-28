using Kitchen;
using KitchenMods;
using System.Reflection;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Team.TeamChecks
{
    public class OverrideBlueprintDisplay : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            var views = Object.FindObjectsOfType<BlueprintView>();

            // Build a lookup of real blueprint entities: appliance ID + price -> team
            var query = GetEntityQuery(
                ComponentType.ReadOnly<CApplianceBlueprint>(),
                ComponentType.ReadOnly<CForSale>(),
                ComponentType.ReadOnly<CTeamAssignment>()
            );
            var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

            foreach (var e in entities)
            {
                if (!Require(e, out CForSale sale)) continue;
                if (!Require(e, out CTeamAssignment team)) continue;
                if (!Require(e, out CPosition pos)) continue;

                var teamData = TeamMoney.Get(team.Team);
                bool canAfford = teamData.Balance >= sale.Price;

                BlueprintView matchedView = null;
                float closestDist = float.MaxValue;
                foreach (var view in views)
                {
                    float dist = Vector3.Distance(view.transform.position, pos.Position);
                    if (dist < closestDist) { closestDist = dist; matchedView = view; }
                }
                if (matchedView == null || closestDist > 1f) continue;

                var type = matchedView.GetType();

                var priceIconField = type.GetField("PriceIcon", BindingFlags.Instance | BindingFlags.NonPublic);
                var priceIcon = priceIconField?.GetValue(matchedView) as TextMeshPro;
                if (priceIcon != null)
                {
                    priceIcon.text = canAfford
                        ? "<sprite name=\"coin\" color=#FF9800>"
                        : "<sprite name=\"coin\" color=#660700>";
                }

                var titleField = type.GetField("Title", BindingFlags.Instance | BindingFlags.NonPublic);
                var titleText = titleField?.GetValue(matchedView) as TextMeshPro;
                if (titleText != null)
                {
                    titleText.color = team.Team == 1 ? new Color(0.3f, 0.6f, 1f) : new Color(1f, 0.4f, 0.4f);
                }
            }
            entities.Dispose();
        }
    }
    
}