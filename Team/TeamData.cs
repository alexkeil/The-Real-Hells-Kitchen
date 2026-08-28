using System.Collections.Generic;

namespace TestMod.Team
{
    public class TeamData
    {
        public int Team;
        public int Balance;
        public Dictionary<int, int> Dishes = new Dictionary<int, int>();

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
            Balance = 0;
            Dishes.Clear();
        }
    }

    public static class TeamMoney
    {
        public static readonly Dictionary<int, TeamData> Teams = new Dictionary<int, TeamData>();

        public static TeamData Get(int team)
        {
            if (!Teams.TryGetValue(team, out var data))
            {
                data = new TeamData { Team = team };
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