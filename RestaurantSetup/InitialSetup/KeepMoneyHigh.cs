using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace PlateVsPlate.RestaurantSetup.InitialSetup
{
    public class KeepMoneyHigh : GenericSystemBase, IModSystem
    {
        private const int TargetAmount = 999999999;

        protected override void OnUpdate()
        {
            var moneyQuery = GetEntityQuery(ComponentType.ReadWrite<SMoney>());
            if (moneyQuery.IsEmptyIgnoreFilter) return;

            var money = moneyQuery.GetSingleton<SMoney>();
            if (money.Amount < TargetAmount / 2) 
            {
                money.Amount = TargetAmount;
                SetSingleton(money);
            }
        }
    }
}