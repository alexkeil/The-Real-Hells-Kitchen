using HarmonyLib;
using Kitchen;
using KitchenData;
using Unity.Entities;
using UnityEngine;

namespace TestMod.RestaurantSetup
{

    [HarmonyPatch(typeof(OpenIngredientParcel), "Perform")]
    public static class OpenIngredientParcel_MirrorForSecondTeam
    {
        static void Postfix(ref InteractionData data, CLetterIngredient ___Letter, CPosition ___Position)
        {
            int id = GameData.Main.ReferableObjects.DefaultProvider.ID;
            if (GameData.Main.TryGet(___Letter.IngredientID, out Item item, true))
            {
                Appliance appliance = item.DedicatedProvider;
                id = appliance == null ? GameData.Main.ReferableObjects.DefaultProvider.ID : appliance.ID;
            }

            var mirroredPos = ___Position;
            mirroredPos.Position = new Vector3(-___Position.Position.x, ___Position.Position.y, ___Position.Position.z);

            Entity entity = data.Context.CreateEntity();
            data.Context.Set(entity, new CCreateAppliance { ID = id });
            data.Context.Set(entity, CItemProvider.InfiniteItemProvider(___Letter.IngredientID));
            data.Context.Set(entity, mirroredPos);
            data.Context.Set(entity, new CTeamAssignment { Team = 1 });

            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(data.Target, new CTeamAssignment { Team = 0 });

            Mod.Logger.LogInfo($"[DEBUGGING] Mirrored parcel (item {___Letter.IngredientID}) to {mirroredPos.Position}");
        }
    }
}