using Kitchen;
using KitchenMods;
using PlateVsPlate.Settings;
using PlateVsPlate.Team;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PlateVsPlate.Views.Blueprint
{

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class BlueprintPriceRefreshSystem : GenericSystemBase, IModSystem
    {
        private const string AffordableTag = "<sprite name=\"coin\" color=#FF9800>";
        private const string UnaffordableTag = "<sprite name=\"coin\" color=#660700>";

        protected override void OnUpdate()
        {
            var views = Object.FindObjectsOfType<BlueprintView>();
            if (views.Length == 0) return;

            var query = GetEntityQuery(typeof(CTeamMarker), typeof(CForSale), typeof(CPosition));
            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var e in entities)
            {
                var pos = EntityManager.GetComponentData<CPosition>(e);
                var marker = EntityManager.GetComponentData<CTeamMarker>(e);
                var sale = EntityManager.GetComponentData<CForSale>(e);

                BlueprintView matchedView = null;
                float closestDist = float.MaxValue;
                foreach (var view in views)
                {
                    float dist = Vector3.Distance(view.transform.position, pos.Position);
                    if (dist < closestDist) { closestDist = dist; matchedView = view; }
                }
                if (matchedView == null || closestDist > 1f) continue;

                bool canAfford = TeamData.Get(marker.Team).Balance >= sale.Price;

                var type = typeof(BlueprintView);
                var priceIconField = type.GetField("PriceIcon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var titleField = type.GetField("Title", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                var priceIcon = priceIconField?.GetValue(matchedView) as TMPro.TextMeshPro;
                var titleText = titleField?.GetValue(matchedView) as TMPro.TextMeshPro;

                if (titleText != null) titleText.color = PvPTeamColors.GetTeamColor(marker.Team);
                if (priceIcon == null) continue;

                string current = priceIcon.text;
                string wantedTag = canAfford ? AffordableTag : UnaffordableTag;
                string otherTag = canAfford ? UnaffordableTag : AffordableTag;
                if (current.EndsWith(otherTag))
                    priceIcon.text = current.Substring(0, current.Length - otherTag.Length) + wantedTag;
            }
            entities.Dispose();
        }
    }
    
}