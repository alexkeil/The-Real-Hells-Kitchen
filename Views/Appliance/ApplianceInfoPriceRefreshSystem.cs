using Kitchen;
using KitchenMods;
using System.Reflection;
using PlateVsPlate.Settings;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using PlateVsPlate.Team;

namespace PlateVsPlate.Views.Appliance
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class ApplianceInfoPriceRefreshSystem : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {

            var views = Object.FindObjectsOfType<ApplianceInfoView>();
            Mod.Logger.LogInfo($"[DEBUGGING] ApplianceInfoPriceRefreshSystem — views={views.Length}");
            if (views.Length == 0) return;

            var query = GetEntityQuery(
                ComponentType.ReadOnly<CShowApplianceInfo>(),
                ComponentType.ReadOnly<CTeamMarker>(),
                ComponentType.ReadOnly<CPosition>()
            );
            var entities = query.ToEntityArray(Allocator.Temp);
            Mod.Logger.LogInfo($"[DEBUGGING] ApplianceInfoPriceRefreshSystem — entities={entities.Length}");

            foreach (var e in entities)
            {
                if (!Require(e, out CShowApplianceInfo info)) continue;
                if (!Require(e, out CTeamMarker marker)) continue;
                if (!Require(e, out CPosition pos)) continue;

                bool canAfford = TeamData.Get(marker.Team).Balance >= info.Price;

                ApplianceInfoView matchedView = null;
                float closestDist = float.MaxValue;
                foreach (var view in views)
                {
                    float dist = Vector3.Distance(view.transform.position, pos.Position);
                    if (dist < closestDist) { closestDist = dist; matchedView = view; }
                }

                Mod.Logger.LogInfo($"[DEBUGGING] entity={e.Index}, team={marker.Team}, price={info.Price}, canAfford={canAfford}, closestDist={closestDist}");

                if (matchedView == null) continue;  // note.. the way this whole method is done.. two people pining two blurprints close together
                                                    // could reveal the wrong 'coin' color
                var type = matchedView.GetType();
                var priceField = type.GetField("Price", BindingFlags.Instance | BindingFlags.NonPublic);
                var titleField = type.GetField("Title", BindingFlags.Instance | BindingFlags.NonPublic);
                var affordableField = type.GetField("Affordable", BindingFlags.Instance | BindingFlags.NonPublic);
                var unaffordableField = type.GetField("Unaffordable", BindingFlags.Instance | BindingFlags.NonPublic);

                var priceText = priceField?.GetValue(matchedView) as TextMeshPro;
                var titleText = titleField?.GetValue(matchedView) as TextMeshPro;

                var affordableColor = (Color)(affordableField?.GetValue(matchedView) ?? Color.white);
                var unaffordableColor = (Color)(unaffordableField?.GetValue(matchedView) ?? Color.red);
                var resultColor = canAfford ? affordableColor : unaffordableColor;

                if (priceText != null) priceText.color = resultColor;
                if (titleText != null) titleText.color = PvPTeamColors.GetTeamColor(marker.Team);
            }
            entities.Dispose();
        }
    }
}