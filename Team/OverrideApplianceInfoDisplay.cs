using Kitchen;
using KitchenMods;
using System.Reflection;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Team
{
    public class OverrideApplianceInfoDisplay : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            var views = Object.FindObjectsOfType<ApplianceInfoView>();

            var query = GetEntityQuery(
                ComponentType.ReadOnly<CShowApplianceInfo>(),
                ComponentType.ReadOnly<CPosition>(),
                ComponentType.ReadOnly<CTeamAssignment>()
            );
            var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

            foreach (var e in entities)
            {
                if (!Require(e, out CShowApplianceInfo info)) continue;
                if (!Require(e, out CTeamAssignment team)) continue;
                if (!Require(e, out CPosition pos)) continue;

                var teamData = TeamMoney.Get(team.Team);
                bool canAfford = teamData.Balance >= info.Price;

                ApplianceInfoView matchedView = null;
                float closestDist = float.MaxValue;
                foreach (var view in views)
                {
                    float dist = Vector3.Distance(view.transform.position, pos.Position);
                    if (dist < closestDist) { closestDist = dist; matchedView = view; }
                }
                if (matchedView == null) continue;

                var type = matchedView.GetType();

                var priceField = type.GetField("Price", BindingFlags.Instance | BindingFlags.NonPublic);
                var titleField = type.GetField("Title", BindingFlags.Instance | BindingFlags.NonPublic);
                var affordableField = type.GetField("Affordable", BindingFlags.Instance | BindingFlags.NonPublic);
                var unaffordableField = type.GetField("Unaffordable", BindingFlags.Instance | BindingFlags.NonPublic);
                var priceTagField = type.GetField("PriceTag", BindingFlags.Instance | BindingFlags.NonPublic);

                var priceText = priceField?.GetValue(matchedView) as TextMeshPro;
                var titleText = titleField?.GetValue(matchedView) as TextMeshPro;
                var priceTagGO = priceTagField?.GetValue(matchedView) as GameObject;

                var affordableColor = (Color)(affordableField?.GetValue(matchedView) ?? Color.white);
                var unaffordableColor = (Color)(unaffordableField?.GetValue(matchedView) ?? Color.red);
                var resultColor = canAfford ? affordableColor : unaffordableColor;

                if (priceText != null) priceText.color = resultColor;

                if (titleText != null)
                    titleText.color = team.Team == 1 ? new Color(0.3f, 0.6f, 1f) : new Color(1f, 0.4f, 0.4f);

                if (priceTagGO != null)
                {
                    var unitTransform = priceTagGO.transform.Find("Unit");
                    if (unitTransform != null)
                    {
                        var unitText = unitTransform.GetComponent<TextMeshPro>();
                        if (unitText != null)
                        {
                            unitText.color = resultColor;
                        }
                    }
                }
            }
            entities.Dispose();
        }
    }
}