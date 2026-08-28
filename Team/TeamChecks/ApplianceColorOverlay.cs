using Kitchen;
using KitchenMods;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Team.TeamChecks
{
    public class ApplianceColorOverlay : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            var views = Object.FindObjectsOfType<ApplianceView>();

            var query = GetEntityQuery(
                ComponentType.ReadOnly<CAppliance>(),
                ComponentType.ReadOnly<CPosition>(),
                ComponentType.ReadOnly<CTeamAssignment>(),
                ComponentType.Exclude<CApplianceBlueprint>()
            );
            var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

            foreach (var e in entities)
            {
                if (!Require(e, out CPosition pos)) continue;
                if (!Require(e, out CTeamAssignment team)) continue;

                ApplianceView matchedView = null;
                float closestDist = float.MaxValue;
                foreach (var view in views)
                {
                    float dist = Vector3.Distance(view.transform.position, pos.Position);
                    if (dist < closestDist) { closestDist = dist; matchedView = view; }
                }
                if (matchedView == null) continue;

                var type = matchedView.GetType();
                var renderersField = type.GetField("MeshRenderers", BindingFlags.Instance | BindingFlags.NonPublic);
                var renderers = renderersField?.GetValue(matchedView) as MeshRenderer[];
                if (renderers == null) continue;

                Color tint = team.Team == 0
                    ? new Color(1f, 0.5f, 0.5f, 1f)
                    : new Color(0.5f, 0.5f, 1f, 1f);

                foreach (var renderer in renderers)
                {
                    if (renderer == null) continue;
                    var mat = renderer.material;

                    if (mat.HasProperty("_OverlayColour"))
                        mat.SetColor("_OverlayColour", tint);

                    if (mat.HasProperty("_HasTextureOverlay")) 
                        mat.SetFloat("_HasTextureOverlay", 1f);

                    if (mat.HasProperty("_Color0")) 
                        mat.SetColor("_Color0", tint);

                    if (mat.HasProperty("_Colour2")) 
                        mat.SetColor("_Colour2", tint);

                    if (mat.HasProperty("_Color2")) 
                        mat.SetColor("_Color2", tint);

                    if (mat.HasProperty("_Highlight")) 
                        mat.SetFloat("_Highlight", 0.3f);

                    if (mat.shader.name == "Simple Transparent")
                        if (mat.HasProperty("_Color")) 
                            mat.SetColor("_Color", tint);
                }
            }
            entities.Dispose();
        }
    }
}