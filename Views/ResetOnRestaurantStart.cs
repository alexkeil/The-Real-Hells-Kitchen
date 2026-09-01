using Kitchen;
using KitchenMods;
using PlateVsPlate.RestaurantSetup.InitialSetup;
using PlateVsPlate.Team.TeamChecks;
using PlateVsPlate.Views.TeamMoney;

namespace PlateVsPlate.Team
{
    public class ResetOnRestaurantStart : GenericSystemBase, IModSystem
    {
        private bool _handledThisTransition = false;

        protected override void OnUpdate()
        {
            if (!Has<SPerformSceneTransition>())
            {
                _handledThisTransition = false;
                return;
            }
            if (_handledThisTransition) return;

            if (!Require(out SPerformSceneTransition transition)) return;

            if (transition.NextScene == SceneType.Kitchen)
            {
                int day = -1;
                if (Has<SDay>())
                {
                    Require(out SDay dayComp);
                    day = dayComp.Day;
                }

                if (day <= 0)
                {
                    DuplicateTeamAppliances.ResetDuplicationState();
                    InterceptPlayerBonus.Reset();
                    MoneyDisplayUpdate_Patch.ResetPanels();
                    Mod.Logger.LogInfo($"[DEBUGGING] Transitioning into Kitchen — fresh restaurant (Day={day}), state reset.");
                }
                else
                {
                    Mod.Logger.LogInfo($"[DEBUGGING] Transitioning into Kitchen — returning restaurant (Day={day}), NOT resetting.");
                }
            }
            _handledThisTransition = true;
        }
    }
}