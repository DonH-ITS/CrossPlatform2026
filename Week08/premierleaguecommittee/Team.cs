using System.Text.Json.Serialization;

namespace MauiApp1
{
    public class Team
    {
        public string Name { get; set; } = "";
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }
        public int GoalDifference { get; set; }
        public int Points { get; set; }

        // This value is calculated and is not stored in premierleague.json.
        [JsonIgnore]
        public int AdjustedPoints { get; set; }
    }
}
