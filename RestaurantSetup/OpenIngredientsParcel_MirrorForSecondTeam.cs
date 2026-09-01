using HarmonyLib;
using Kitchen;
using KitchenData;
using PlateVsPlate.RestaurantSetup.InitialSetup;
using PlateVsPlate.Team;
using Unity.Entities;
using UnityEngine;

namespace PlateVsPlate.RestaurantSetup
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

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;

            bool hasProvider = em.HasComponent<CItemProvider>(data.Target);
            bool hasTable = em.HasComponent<CApplianceTable>(data.Target);

            var mirroredPos = ___Position;
            mirroredPos.Position = new Vector3(-___Position.Position.x, ___Position.Position.y, ___Position.Position.z);

            Entity entity = data.Context.CreateEntity();
            data.Context.Set(entity, new CCreateAppliance { ID = id });

            if (hasProvider)
                data.Context.Set(entity, em.GetComponentData<CItemProvider>(data.Target));
            else
                data.Context.Set(entity, CItemProvider.InfiniteItemProvider(___Letter.IngredientID));

            data.Context.Set(entity, mirroredPos);

            DuplicateTeamAppliances.QueuePendingTag(entity, 1);
            em.AddComponentData(data.Target, new CTeamAssignment { Team = 0 });
            em.AddComponentData(data.Target, new CTeamMarker { Team = 0 });

        }
    }
}