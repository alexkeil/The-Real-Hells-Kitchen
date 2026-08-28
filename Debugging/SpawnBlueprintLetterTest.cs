using Kitchen;
using KitchenMods;
using Unity.Collections;
using UnityEngine;

namespace TestMod.Debugging
{
    public class SpawnBlueprintLetterTest : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F12)) return;

            int wokApplianceID = 1139247360;

            var playerQuery = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            var players = playerQuery.ToEntityArray(Allocator.Temp);
            CPosition playerPos = default;
            if (players.Length > 0) Require(players[0], out playerPos);
            players.Dispose();

            var testPos = playerPos;
            testPos.Position = new Vector3(playerPos.Position.x, playerPos.Position.y, playerPos.Position.z);

            PostHelpers.CreateBlueprintLetter(EntityManager, testPos.Position, wokApplianceID);

            Debug.Log($"[DEBUGGING] Spawned test blueprint letter at {testPos.Position}.");
        }
    }
}