using Kitchen;
using KitchenMods;
using Unity.Collections;

namespace PlateVsPlate.RestaurantSetup.InitialSetup
{
    
    // i wanted to remove this destroy.. but for some reason it crashes when the map loads in?!
    public class RemoveBuiltInMapPedestals : GenericSystemBase, IModSystem
    {
        static readonly int[] BuiltInPedestalIDs = { 1823459359, -1114059052 }; //477050702 }; //  };

        protected override void OnUpdate()
        {
            
            var query = GetEntityQuery(typeof(CAppliance));
            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var e in entities)
            {
                if (!Require(e, out CAppliance app)) continue;
                if (System.Array.IndexOf(BuiltInPedestalIDs, app.ID) >= 0)
                {
                    EntityManager.DestroyEntity(e);
                }
            }

            entities.Dispose();
        }
    }
    
    
}