using Kitchen;
using KitchenMods;
using System.Reflection;
using UnityEngine;

namespace TestMod.Debugging
{
    public class DebugDumpViewFields : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F8)) DumpFields<ApplianceView>();
           
            // if (Input.GetKeyDown(KeyCode.F9)) DumpFields<BlueprintView>();
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
    }
}