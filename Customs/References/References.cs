using ApplianceLib.Api.References;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using static TestMod.Customs.References.GDOHelpers;

namespace TestMod.Customs.References
{
    internal static class GDOHelpers
    {
        internal static T1 GetModdedGDO<T1, T2>() where T1 : GameDataObject
        {
            return (T1)GDOUtils.GetCustomGameDataObject<T2>().GameDataObject;
        }
        internal static T GetExistingGDO<T>(int id) where T : GameDataObject
        {
            return (T)GDOUtils.GetExistingGDO(id);
        }
        internal static T Find<T>(int id) where T : GameDataObject
        {
            return (T)GDOUtils.GetExistingGDO(id) ?? (T)GDOUtils.GetCustomGameDataObject(id)?.GameDataObject;
        }

        internal static T Find<T, C>() where T : GameDataObject where C : CustomGameDataObject
        {
            return GDOUtils.GetCastedGDO<T, C>();
        }
        internal static T Find<T>(string modName, string name) where T : GameDataObject
        {
            return GDOUtils.GetCastedGDO<T>(modName, name);
        }
    }

    internal static class VanillaLib
    {
        // Vanilla Dishes
        internal static Dish HotdogDish => GetExistingGDO<Dish>(DishReferences.HotdogBase);

        // Vanilla Processes
        internal static Process Cook => GetExistingGDO<Process>(ProcessReferences.Cook);
        internal static Process Chop => GetExistingGDO<Process>(ProcessReferences.Chop);
        internal static Process Knead => GetExistingGDO<Process>(ProcessReferences.Knead);
        internal static Process Oven => GetExistingGDO<Process>(ProcessReferences.RequireOven);

        // Vanilla Ingredients
        internal static Item Flour => GetExistingGDO<Item>(ItemReferences.Flour);
        internal static Item Cheese => GetExistingGDO<Item>(ItemReferences.Cheese);
        internal static Item GratedCheese => GetExistingGDO<Item>(ItemReferences.CheeseGrated);
        internal static Item BreadSlice => GetExistingGDO<Item>(ItemReferences.BreadSlice);
        internal static Item Tomato => GetExistingGDO<Item>(ItemReferences.Tomato);
        internal static Item TomatoSlice => GetExistingGDO<Item>(ItemReferences.TomatoChopped);
        internal static Item TomatoSauce => GetExistingGDO<Item>(ItemReferences.TomatoSauce);
        internal static Item Egg => GetExistingGDO<Item>(ItemReferences.Egg);
        internal static Item EggCracked => GetExistingGDO<Item>(ItemReferences.EggCracked);
        internal static Item Apple => GetExistingGDO<Item>(ItemReferences.Apple);
        internal static Item AppleSlices => GetExistingGDO<Item>(ItemReferences.AppleSlices);
        internal static Item Sugar => GetExistingGDO<Item>(ItemReferences.Sugar);
        internal static Item BreadCrumbs => GetExistingGDO<Item>(ItemReferences.Breadcrumbs);
        internal static Item VanillaIceCream => GetExistingGDO<Item>(ItemReferences.IceCreamVanilla);
        internal static Item ApplePie => GetExistingGDO<Item>(ItemReferences.PieAppleCooked);
        internal static Item Oil => GetExistingGDO<Item>(ItemReferences.Oil);
        internal static Item OilIngredient => GetExistingGDO<Item>(ItemReferences.OilIngredient);
        internal static Item Onion => GetExistingGDO<Item>(ItemReferences.Onion);
        internal static Item OnionChopped => GetExistingGDO<Item>(ItemReferences.OnionChopped);
        internal static Item PieCrust => GetExistingGDO<Item>(ItemReferences.PieCrustCooked);
        internal static Item BurntBread => GetExistingGDO<Item>(ItemReferences.BurnedBread);
        internal static Item Dough => GetExistingGDO<Item>(ItemReferences.Dough);
        internal static Item SteakWellDone => GetExistingGDO<Item>(ItemReferences.SteakWelldone);
        internal static Item Meat => GetExistingGDO<Item>(ItemReferences.Meat);
        internal static Item Corn => GetExistingGDO<Item>(ItemReferences.CornRaw);
        internal static Item HuskedCorn => GetExistingGDO<Item>(ItemReferences.CornHusked);
        internal static Item ChoppedMeat => GetExistingGDO<Item>(ItemReferences.MeatChopped);
        internal static Item Rice => GetExistingGDO<Item>(ItemReferences.Rice);
        internal static Item RiceCooked => GetExistingGDO<Item>(ItemReferences.RiceContainerCooked);
        internal static Item Beans => GetExistingGDO<Item>(ItemReferences.BeansIngredient);
        internal static Item DogBun => GetExistingGDO<Item>(ItemReferences.HotdogBun);
        internal static Item CookedHotDog => GetExistingGDO<Item>(ItemReferences.HotdogCooked);
        internal static Item HotDog => GetExistingGDO<Item>(ItemReferences.HotdogRaw);
        internal static Item DepletedSoup => GetExistingGDO<Item>(ItemReferences.SoupDepleted);
        internal static Item Broth => GetExistingGDO<Item>(ItemReferences.BrothCookedOnion);
        internal static Item Pumpkin => GetExistingGDO<Item>(ItemReferences.Pumpkin);
        internal static Item PumpkinHallow => GetExistingGDO<Item>(ItemReferences.PumpkinHollow);
        internal static Item Broccoli => GetExistingGDO<Item>(ItemReferences.BroccoliRaw);
        internal static Item Carrot => GetExistingGDO<Item>(ItemReferences.Carrot);
        internal static Item Lettuce => GetExistingGDO<Item>(ItemReferences.Lettuce);
        internal static Item Mushroom => GetExistingGDO<Item>(ItemReferences.Mushroom);
        internal static Item Potato => GetExistingGDO<Item>(ItemReferences.Potato);

        // Vanilla Items
        internal static Item Plate => GetExistingGDO<Item>(ItemReferences.Plate);
        internal static Item DirtyPlate => GetExistingGDO<Item>(ItemReferences.PlateDirty);
        internal static Item Ketchup => GetExistingGDO<Item>(ItemReferences.CondimentKetchup);
        internal static Item Mustard => GetExistingGDO<Item>(ItemReferences.CondimentMustard);
        internal static Item Pot => GetExistingGDO<Item>(ItemReferences.Pot);
        internal static Item Water => GetExistingGDO<Item>(ItemReferences.Water);
        internal static Item ServingBoard => GetExistingGDO<Item>(ItemReferences.ServingBoard);
    }
    internal static class IngredientLibLib
    {
        public static Item Butter => Find<Item>(IngredientLib.References.GetIngredient("butter"));
        public static Item ButterSlice => Find<Item>(IngredientLib.References.GetSplitIngredient("butter"));
        public static Item Bacon => Find<Item>(IngredientLib.References.GetIngredient("bacon"));
        public static Item UncookedBacon => Find<Item>(IngredientLib.References.GetIngredient("chopped pork"));
        public static Item Pork => Find<Item>(IngredientLib.References.GetIngredient("pork"));
        public static Item Ham => Find<Item>(IngredientLib.References.GetIngredient("porkchop"));
        public static Item Milk => Find<Item>(IngredientLib.References.GetIngredient("milk"));
        public static Item MilkIngredient => Find<Item>(IngredientLib.References.GetSplitIngredient("milk"));
        public static Item CookedMacaroni => Find<Item>(IngredientLib.References.GetIngredient("cooked potted macaroni"));
        public static Item Macaroni => Find<Item>(IngredientLib.References.GetIngredient("macaroni"));
        public static Item Garlic => Find<Item>(IngredientLib.References.GetIngredient("garlic"));
        public static Item MincedGarlic => Find<Item>(IngredientLib.References.GetIngredient("minced garlic"));
        public static Item Chocolate => Find<Item>(IngredientLib.References.GetIngredient("chocolate"));
        public static Item ChoppedChocolate => Find<Item>(IngredientLib.References.GetIngredient("chopped chocolate"));
        public static Item ChocolateFilling => Find<Item>(IngredientLib.References.GetIngredient("chocolate sauce"));
        public static Item Cinnamon => Find<Item>(IngredientLib.References.GetIngredient("cinnamon"));
        public static Item Banana => Find<Item>(IngredientLib.References.GetIngredient("banana"));
        public static Item PeeledBanana => Find<Item>(IngredientLib.References.GetIngredient("peeled banana"));
        public static Item Oats => Find<Item>(IngredientLib.References.GetIngredient("oats"));
        public static Item Peppers => Find<Item>(IngredientLib.References.GetIngredient("peppers"));
        public static Item ChoppedPeppers => Find<Item>(IngredientLib.References.GetIngredient("chopped peppers"));
        public static Item WhippingCream => Find<Item>(IngredientLib.References.GetIngredient("whipping cream"));
        public static Item WhippedCream => Find<Item>(IngredientLib.References.GetIngredient("whipped cream"));
        public static Item Tortilla => Find<Item>(IngredientLib.References.GetIngredient("tortilla"));
        public static Item Noodles => Find<Item>(IngredientLib.References.GetIngredient("box pasta"));
        public static Item CookedNoodles => Find<Item>(IngredientLib.References.GetIngredient("cooked potted pasta"));

    }
    internal static class Alib
    {
        public static Item Cup => ApplianceLibGDOs.Refs.Cup;
    }
}
    /*
    internal static class MilkLib
    {
        internal static ItemGroup MilkGlass => GetModdedGDO<ItemGroup, MilkGlass>();
        internal static Dish MilkDish => GetModdedGDO<Dish, MilkDish>();
    }
    */
    

