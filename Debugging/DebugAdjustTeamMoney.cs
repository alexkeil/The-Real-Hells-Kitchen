using Kitchen;
using KitchenMods;
using PlateVsPlate.Team;
using UnityEngine;

namespace PlateVsPlate.Debugging
{
    public class DebugAdjustTeamMoney : GenericSystemBase, IModSystem
    {
        private const int Amount = 50; 

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                TeamData.Get(0).Balance += Amount;
                Mod.Logger.LogInfo($"[DEBUGGING] Team 0 balance +{Amount} -> {TeamData.Get(0).Balance}");
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                TeamData.Get(0).Balance -= Amount;
                Mod.Logger.LogInfo($"[DEBUGGING] Team 0 balance -{Amount} -> {TeamData.Get(0).Balance}");
            }
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                TeamData.Get(1).Balance += Amount;
                Mod.Logger.LogInfo($"[DEBUGGING] Team 1 balance +{Amount} -> {TeamData.Get(1).Balance}");
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                TeamData.Get(1).Balance -= Amount;
                Mod.Logger.LogInfo($"[DEBUGGING] Team 1 balance -{Amount} -> {TeamData.Get(1).Balance}");
            }
        }
    }
}