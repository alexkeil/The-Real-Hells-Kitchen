using HarmonyLib;
using Kitchen;
using System;
using Unity.Entities;
using UnityEngine;

namespace TestMod.RestaurantSetup.InitialSetup
{
    [HarmonyPatch(typeof(OpenLetter), "Perform")]
    public static class OpenLetter_MirrorForSecondTeam
    {
        static bool Prefix(ref InteractionData data)
        {
            try
            {
                var em = World.DefaultGameObjectInjectionWorld.EntityManager;

                if (em.HasComponent<CTeamAssignment>(data.Target)) return true;
                em.AddComponentData(data.Target, new CTeamAssignment { Team = 0 });

                if (!em.HasComponent<CLetterBlueprint>(data.Target)) return true;
                var letter = em.GetComponentData<CLetterBlueprint>(data.Target);

                if (!em.HasComponent<CPosition>(data.Target)) return true;
                var pos = em.GetComponentData<CPosition>(data.Target);

                Entity originalResult = PostHelpers.OpenBlueprintLetter(data.Context, data.Target);
                data.Context.Set(originalResult, new CTeamAssignment { Team = 0 });

                var mirroredPos = pos;
                mirroredPos.Position = new Vector3(-pos.Position.x, pos.Position.y, pos.Position.z);

                Entity tempLetter = em.CreateEntity();
                em.AddComponentData(tempLetter, mirroredPos);
                em.AddComponentData(tempLetter, letter);

                Entity mirroredResult = PostHelpers.OpenBlueprintLetter(data.Context, tempLetter);
                data.Context.Set(mirroredResult, new CTeamAssignment { Team = 1 });

                em.DestroyEntity(tempLetter);

                data.Context.Destroy(data.Target);

                Logger($"[DEBUGGING] Opened & mirrored blueprint letter (appliance {letter.ApplianceID}) to {mirroredPos.Position}");

                return false;
            }
            catch (System.Exception ex)
            {
                Mod.Logger.LogInfo($"[DEBUGGING] EXCEPTION in OpenLetter mirror prefix: {ex}");
                return true;
            }
        }

        private static void Logger(string v)
        {
            throw new NotImplementedException();
        }
    }
}