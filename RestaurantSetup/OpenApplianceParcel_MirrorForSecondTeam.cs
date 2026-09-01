using HarmonyLib;
using Kitchen;
using PlateVsPlate.Team;
using Unity.Entities;
using UnityEngine;

namespace PlateVsPlate.RestaurantSetup
{
    [HarmonyPatch(typeof(OpenApplianceParcel), "Perform")]
    public static class OpenApplianceParcel_MirrorForSecondTeam
    {
        static void Postfix(ref InteractionData data, CLetterAppliance ___Letter, CPosition ___Position)
        {
            if (___Letter.ApplianceID == 238041352) return; // Booking Desk temp

            var mirroredPos = ___Position;
            mirroredPos.Position = new Vector3(-___Position.Position.x, ___Position.Position.y, ___Position.Position.z);

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;

            Entity entity = data.Context.CreateEntity();
            data.Context.Set(entity, new CCreateAppliance { ID = ___Letter.ApplianceID });
            data.Context.Set(entity, mirroredPos);
            data.Context.Set(entity, new CTeamAssignment { Team = 1 });
            data.Context.Set(entity, new CTeamMarker { Team = 1 });

            em.AddComponentData(data.Target, new CTeamAssignment { Team = 0 });
            em.AddComponentData(data.Target, new CTeamMarker { Team = 0 });

        }
    }
}