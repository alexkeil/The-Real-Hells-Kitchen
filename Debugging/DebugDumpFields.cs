using System.Linq;
using Kitchen;
using KitchenMods;
using System.Reflection;
using UnityEngine;

namespace PlateVsPlate.Debugging
{
    public class DebugDumpFields : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F8)) DumpFields<ApplianceInfoView>();
            if (Input.GetKeyDown(KeyCode.F7)) DumpPriceTagChildren();
        }

        private void DumpFields<T>() where T : Object
        {
            var instances = Object.FindObjectsOfType<T>();
            Mod.Logger.LogInfo($"[DEBUGGING] Found {instances.Length} {typeof(T).Name} instances");

            if (instances.Length == 0) return;

            var instance = instances[0];
            var type = instance.GetType();

            while (type != null)
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    var value = field.GetValue(instance);
                    Mod.Logger.LogInfo($"[DEBUGGING] {type.Name}.{field.Name} ({field.FieldType.Name}) = {value}");
                }
                type = type.BaseType;
            }
        }

        private void DumpPriceTagChildren()
        {
            var instances = Object.FindObjectsOfType<ApplianceInfoView>();
            if (instances.Length == 0) return;

            var instance = instances[0];
            var type = instance.GetType();
            var priceTagField = type.GetField("PriceTag", BindingFlags.Instance | BindingFlags.NonPublic);
            var priceTagGO = priceTagField?.GetValue(instance) as GameObject;

            if (priceTagGO == null)
            {
                Mod.Logger.LogInfo("[DEBUGGING] PriceTag GameObject not found");
                return;
            }

            Mod.Logger.LogInfo($"[DEBUGGING] PriceTag has {priceTagGO.transform.childCount} children:");
            for (int i = 0; i < priceTagGO.transform.childCount; i++)
            {
                var child = priceTagGO.transform.GetChild(i);
                var componentNames = string.Join(", ", child.GetComponents<Component>().Select(c => c.GetType().Name));
                Mod.Logger.LogInfo($"[DEBUGGING]   Child {i}: {child.name}, components: {componentNames}");
            }
        }
    }
}