using Kitchen;
using KitchenMods;
using System.Reflection;
using UnityEngine;

namespace TestMod.Team.TeamChecks
{
    public class DebugApplianceMaterials : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F7)) return;

            var views = Object.FindObjectsOfType<ApplianceView>();
            if (views.Length == 0) return;

            var view = views[0];
            var type = view.GetType();
            var renderersField = type.GetField("MeshRenderers", BindingFlags.Instance | BindingFlags.NonPublic);
           
            var renderers = renderersField?.GetValue(view) as MeshRenderer[];
            if (renderers == null) { 
                Mod.Logger.LogInfo("[DEBUGGING] No renderers found"); 
                return; 
            }

            Mod.Logger.LogInfo($"[DEBUGGING] Found {renderers.Length} renderers");

            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                var mat = r.sharedMaterial;
                var shader = mat.shader;
                int propCount = shader.GetPropertyCount();
                Mod.Logger.LogInfo($"[DEBUGGING] Renderer {i} shader '{shader.name}' has {propCount} properties:");
                for (int p = 0; p < propCount; p++)
                {
                    string propName = shader.GetPropertyName(p);
                    var propType = shader.GetPropertyType(p);
                    Mod.Logger.LogInfo($"[DEBUGGING] {propName} ({propType})");
                }
            }

        }
    }
}