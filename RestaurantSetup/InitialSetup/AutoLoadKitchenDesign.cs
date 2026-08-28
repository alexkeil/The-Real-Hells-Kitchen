using Kitchen;
using KitchenMods;
using ONe.KitchenDesigner.KitchenDesigns;
using ONe.KitchenDesigner.KitchenDesigns.Decoders;
using UnityEngine;

namespace TestMod.RestaurantSetup.InitialSetup
{
    public static class MapBoundsInfo
    {
        public static bool HasBounds = false;
        public static float MinX, MaxX, MinY, MaxY;
    }

    public class AutoLoadKitchenDesign : GenericSystemBase, IModSystem
    {
        private bool _hasLoaded = false;

        protected override void OnUpdate()
        {
            if (_hasLoaded) return;
            if (!Has<CSeededRunInfo>()) return;

            string savedLayout = "2NTU3OTQzMTU1OjIwMDI4NzYyOTU6MiwyOzEsMzoxOCwxMzoyLjUsMS44LDIuNSwxLjgsMi41LDEuOCwyLjUsMS44LDIuNSwxLjgsMi41LDEuOCwyLjUsMS44LDIuNSwxLjgsMi41LDEuOCwyLjUsMS44LDIuNSwxLjgsMi41LDEuOCwyLjUsMS44LDIuNSwxLjgsMi41LDEuOCwyLjUsMS44LDIuNSwxLjgsMi41LDEuODowLDQsMCw1LDE7MSw0LDEsNSwyOzIsNCwyLDUsMjszLDQsMyw1LDI7NCw0LDQsNSwyOzUsNCw1LDUsMjs2LDQsNiw1LDI7Nyw0LDcsNSwyOzgsLTEsOCwwLDM7OCw0LDgsNSwyOzksNCw5LDUsMjsxMCw0LDEwLDUsMjsxMSw0LDExLDUsMjsxMiw0LDEyLDUsMjo=";
            var design = V2Decoder.Load(savedLayout);

            if (design.Blueprint.Tiles.Count > 0)
            {
                var rawMinX = float.MaxValue;
                var rawMaxX = float.MinValue;
                var rawMinY = float.MaxValue;
                var rawMaxY = float.MinValue;

                foreach (var tile in design.Blueprint.Tiles)
                {
                    var pos = tile.Key;
                    if (pos.x < rawMinX) rawMinX = pos.x;
                    if (pos.x > rawMaxX) rawMaxX = pos.x;
                    if (pos.y < rawMinY) rawMinY = pos.y;
                    if (pos.y > rawMaxY) rawMaxY = pos.y;
                }

                float centerX = (rawMinX + rawMaxX) / 2f;
                float centerY = (rawMinY + rawMaxY) / 2f;

                MapBoundsInfo.MinX = rawMinX - centerX;
                MapBoundsInfo.MaxX = rawMaxX - centerX;
                MapBoundsInfo.MinY = rawMinY - centerY;
                MapBoundsInfo.MaxY = rawMaxY - centerY;
                MapBoundsInfo.HasBounds = true;
            }

            KitchenDesignLoader.LoadKitchenDesign(design, null);
            _hasLoaded = true;

            Mod.Logger.LogInfo($"[DEBUGGING] Auto-loaded design. Centered bounds: X[{MapBoundsInfo.MinX},{MapBoundsInfo.MaxX}] Y[{MapBoundsInfo.MinY},{MapBoundsInfo.MaxY}]");
        }
    }
}