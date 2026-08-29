using Kitchen;
using KitchenMods;
using TestMod;

namespace TestMod.Debugging
{
    public class CleanupOnLobbyReturn : GenericSystemBase, IModSystem
    {
        private bool _wasInKitchen = false;
        private bool _sawGameOver = false;

        protected override void OnUpdate()
        {
            bool inKitchen = Has<SKitchenMarker>();

            if (Has<SGameOver>())
            {
                _sawGameOver = true; // remember this happened, even if lobby-return isn't immediate
            }

            if (_wasInKitchen && !inKitchen)
            {
                TestMod.Mod.Logger.LogInfo($"[DEBUGGING] Return to lobby — sawGameOver={_sawGameOver}");

                if (_sawGameOver)
                {
                    // TODO: real reset logic — this was an abandon/loss, not a normal completion
                }

                _sawGameOver = false; // reset for next session
            }

            _wasInKitchen = inKitchen;
        }
    }
}
