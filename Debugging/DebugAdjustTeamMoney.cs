using Kitchen;
using KitchenMods;
using TestMod.Team;
using UnityEngine;

namespace TestMod.RestaurantSetup
{
    public class DebugAdjustTeamMoney : GenericSystemBase, IModSystem
    {
        private const int Amount = 50; 

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                TeamMoney.Get(0).Balance += Amount;
                Mod.Logger.LogInfo($"[DEBUGGING] Team 0 balance +{Amount} -> {TeamMoney.Get(0).Balance}");
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                TeamMoney.Get(0).Balance -= Amount;
                Mod.Logger.LogInfo($"[DEBUGGING] Team 0 balance -{Amount} -> {TeamMoney.Get(0).Balance}");
            }
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                TeamMoney.Get(1).Balance += Amount;
                Mod.Logger.LogInfo($"[DEBUGGING] Team 1 balance +{Amount} -> {TeamMoney.Get(1).Balance}");
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                TeamMoney.Get(1).Balance -= Amount;
                Mod.Logger.LogInfo($"[DEBUGGING] Team 1 balance -{Amount} -> {TeamMoney.Get(1).Balance}");
            }
        }
    }
}