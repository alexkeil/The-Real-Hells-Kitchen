using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestMod.Team.TeamChecks
{
    public class EnforceHeldItemOwnership : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            var query = GetEntityQuery(typeof(CHeldBy), typeof(CTeamAssignment));
            var items = query.ToEntityArray(Allocator.Temp);

            foreach (var item in items)
            {
                if (!Require(item, out CHeldBy heldBy)) continue;
                if (!Require(item, out CTeamAssignment itemTeam)) continue;

                bool holderHasTeam = Require(heldBy.Holder, out CTeamAssignment holderTeam);

                if (!holderHasTeam || itemTeam.Team != holderTeam.Team)
                {
                    if (!Require(heldBy.Holder, out CPosition holderPos))
                    {
                        Mod.Logger.LogInfo($"[DEBUGGING] Could not get holder position for force-drop");
                        continue;
                    }

                    holderPos.Rotation = Quaternion.identity;

                    EntityManager.RemoveComponent<CHeldAppliance>(item);
                    EntityManager.RemoveComponent<CHeldBy>(item);
                    EntityManager.SetComponentData(heldBy.Holder, default(CItemHolder));
                    EntityManager.SetComponentData(item, holderPos);
                    EntityManager.AddComponent<CRemoveView>(item);
                    EntityManager.SetComponentData(item, new CRequiresView { Type = ViewType.Appliance });

                }
            }

            items.Dispose();
        }
    }
}