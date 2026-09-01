using KitchenLib.Preferences;

namespace PlateVsPlate.Setttings
{
    public static class PvPModSettings
    {
        private static PreferenceManager manager;

        private const string ShowRestartPopupKey = "showRestartPopup";
        private const string StrikesBeforeEliminationKey = "strikesBeforeElimination";
        private const string Team0ColorKey = "team0Color";
        private const string Team1ColorKey = "team1Color";
        private const string BalanceByCustomerCountKey = "balanceByCustomerCount";

        private const bool DefaultShowRestartPopup = false;
        private const int DefaultStrikesBeforeElimination = 3;
        private const int DefaultTeam0ColorIndex = 0;
        private const int DefaultTeam1ColorIndex = 1;
        private const bool DefaultBalanceByCustomerCount = false;

        public static void RegisterPreferences()
        {
            manager = new PreferenceManager(Mod.MOD_GUID);

            manager.RegisterPreference(new PreferenceBool(ShowRestartPopupKey, DefaultShowRestartPopup));
            manager.RegisterPreference(new PreferenceInt(StrikesBeforeEliminationKey, DefaultStrikesBeforeElimination));
            manager.RegisterPreference(new PreferenceInt(Team0ColorKey, DefaultTeam0ColorIndex));
            manager.RegisterPreference(new PreferenceInt(Team1ColorKey, DefaultTeam1ColorIndex));
            manager.RegisterPreference(new PreferenceBool(BalanceByCustomerCountKey, DefaultBalanceByCustomerCount));

            manager.Load();
            manager.Save();
        }

        public static bool GetShowRestartPopup() =>
            manager.GetPreference<PreferenceBool>(ShowRestartPopupKey).Value;

        public static void SetShowRestartPopup(bool value)
        {
            manager.Set<PreferenceBool>(ShowRestartPopupKey, value);
            manager.Save();
        }

        public static int GetStrikesBeforeElimination() =>
            manager.GetPreference<PreferenceInt>(StrikesBeforeEliminationKey).Value;

        public static void SetStrikesBeforeElimination(int value)
        {
            manager.Set<PreferenceInt>(StrikesBeforeEliminationKey, value);
            manager.Save();
        }

        public static int GetTeam0ColorIndex() =>
            manager.GetPreference<PreferenceInt>(Team0ColorKey).Value;

        public static void SetTeam0ColorIndex(int value)
        {
            manager.Set<PreferenceInt>(Team0ColorKey, value);
            manager.Save();
        }

        public static int GetTeam1ColorIndex() =>
            manager.GetPreference<PreferenceInt>(Team1ColorKey).Value;

        public static void SetTeam1ColorIndex(int value)
        {
            manager.Set<PreferenceInt>(Team1ColorKey, value);
            manager.Save();
        }

        public static bool GetBalanceByCustomerCount() =>
            manager.GetPreference<PreferenceBool>(BalanceByCustomerCountKey).Value;

        public static void SetBalanceByCustomerCount(bool value)
        {
            manager.Set<PreferenceBool>(BalanceByCustomerCountKey, value);
            manager.Save();
        }
    }
}