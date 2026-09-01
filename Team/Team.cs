using System.Collections.Generic;

namespace PlateVsPlate.Team
{
    public class Team
    {
        public int TeamID;

        public int Balance;
        public int TotalEarned;   
        public int Strikes;
        public int GroupsSeated;
        public int CustomersSeated;

        public Dictionary<int, int> Dishes = new Dictionary<int, int>();

        public void EarnMoney(int amount)
        {
            Balance += amount;
            if (amount > 0) TotalEarned += amount;
        }

        public void SpendMoney(int amount)
        {
            Balance -= amount; 
        }

        public int AddStrike()
        {
            Strikes++;
            return Strikes;
        }

        public void RecordSeated(int customerCount)
        {
            GroupsSeated++;
            CustomersSeated += customerCount;
        }

        public void RecordDish(int dishIdentifier)
        {
            if (Dishes.TryGetValue(dishIdentifier, out int count))
                Dishes[dishIdentifier] = count + 1;
            else
                Dishes[dishIdentifier] = 1;
        }

        public int GetDishCount(int dishIdentifier)
        {
            return Dishes.TryGetValue(dishIdentifier, out int count) ? count : 0;
        }

        public int GetTotalDishCount()
        {
            int total = 0;
            foreach (int count in Dishes.Values) total += count;
            return total;
        }

        public void Clear()
        {
            Mod.Logger.LogInfo($"[DEBUGGING] Team.Clear() called on team {TeamID} — was Balance={Balance}, now zeroing");
            Balance = 0;
            TotalEarned = 0; // assumption: resets alongside everything else at match reset
            Strikes = 0;
            GroupsSeated = 0;
            CustomersSeated = 0;
            Dishes.Clear();
        }
    }

    public static class TeamData
    {
        public static readonly Dictionary<int, Team> Teams = new Dictionary<int, Team>();

        public static Team Get(int team)
        {
            if (!Teams.TryGetValue(team, out var data))
            {
                data = new Team { TeamID = team };
                Teams[team] = data;
            }
            return data;
        }

        public static void ClearAll()
        {
            foreach (var team in Teams.Values)
                team.Clear();
        }
    }
}