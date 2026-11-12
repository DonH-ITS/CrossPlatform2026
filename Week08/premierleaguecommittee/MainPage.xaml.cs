namespace MauiApp1
{
    using System.Text.Json;

    public partial class MainPage : ContentPage
    {
        private List<Team> _teams = new List<Team>();
        private List<DisciplinaryRecord> _decisions = new List<DisciplinaryRecord>();

        private bool _alternateTheme = false;

        // These three properties are used by the data binding in Section 6.
        private string _committeeStatus = "Load the original league table to begin.";
        public string CommitteeStatus
        {
            get { return _committeeStatus; }
            set
            {
                _committeeStatus = value;
                OnPropertyChanged();
            }
        }

        private int _totalAdjustment = 0;
        public int TotalAdjustment
        {
            get { return _totalAdjustment; }
            set
            {
                _totalAdjustment = value;
                OnPropertyChanged();
            }
        }

        private string _decisionLog = "No decisions yet.";
        public string DecisionLog
        {
            get { return _decisionLog; }
            set
            {
                _decisionLog = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();

            // The colours in this lab assume a light theme. Without this line, a computer
            // set to dark mode would show white text on a light background.
            Application.Current!.UserAppTheme = AppTheme.Light;

            BindingContext = this;
        }

        private async void LoadOriginalTable_Clicked(object sender, EventArgs e)
        {
            // TODO: Open premierleague.json from Resources/Raw.
            using var stream = await FileSystem.OpenAppPackageFileAsync("premierleague.json");
            using var reader = new StreamReader(stream);

            // TODO: Read the JSON text from the file.
            string json = await reader.ReadToEndAsync();

            // TODO: Deserialise the JSON into a List<Team> and store it in _teams.
            _teams = JsonSerializer.Deserialize<List<Team>>(json);

            CalculateTable();
            DisplayTable();

            CommitteeStatus = "Original league table loaded.";
        }

        private void AddDecision_Clicked(object sender, EventArgs e)
        {
            // TODO: Read the three Entry controls.
            string teamName = TeamNameEntry.Text;
            string reason = ReasonEntry.Text;
            // TODO: Check that the points adjustment is a valid integer.
            if (!int.TryParse(PointsAdjustmentEntry.Text, out int points))
            {
                CommitteeStatus = "Enter a number for points adjustment";
                return;
            }
            // TODO: Check that a club name has been entered.
            // TODO: Check that a reason has been entered.
            if (string.IsNullOrEmpty(teamName) || string.IsNullOrEmpty(reason))
            {
                CommitteeStatus = "Enter a team Name and Reason";
                return;
            }
            // TODO: Check that the club exists in the league table (use FindTeam).
            Team? team = FindTeam(teamName);
            if (team == null)
            {
                CommitteeStatus = "Team not found in table";
                return;
            }
            // TODO: Create a DisciplinaryRecord and add it to _decisions.
            DisciplinaryRecord record = new DisciplinaryRecord
            {
                PointsAdjustment = points,
                Reason = reason,
                TeamName = team.Name
            };
            _decisions.Add(record);
            // TODO: Recalculate and redisplay the table, and call UpdateDecisionSummary().
            UpdateDecisionSummary();
            CalculateTable();
            DisplayTable();
            // TODO: Update CommitteeStatus and clear the three Entry controls.
            CommitteeStatus = $"{team.Name} has had their points changed";
            TeamNameEntry.Text = "";
            ReasonEntry.Text = "";
            PointsAdjustmentEntry.Text = "";
        }

        private async void SaveDecisions_Clicked(object sender, EventArgs e)
        {
            // TODO: Create a CommitteeData object containing _decisions.
            CommitteeData data = new CommitteeData();
            data.Decisions = _decisions;
            // TODO: Serialise it to JSON.
            try
            {
                string json = JsonSerializer.Serialize(data);
                string path = GetDecisionsPath();
                await DisplayAlertAsync("Message", path, "OK");
                // TODO: Write the JSON to decisions.json (use GetDecisionsPath()).
                await File.WriteAllTextAsync(path, json);
                // TODO: Update CommitteeStatus.
                CommitteeStatus = "File is written";
            }
            catch (Exception ex)
            {
                CommitteeStatus = "Failed to write file because " + ex.Message;
            }
        }

        private async void LoadDecisions_Clicked(object sender, EventArgs e)
        {
            // TODO: Check whether decisions.json exists. If it doesn't, say so in
            //       CommitteeStatus and stop.
            string path = GetDecisionsPath();
            if (!File.Exists(path))
            {
                CommitteeStatus = "File does not exist";
                return;
            }
            // TODO: Read the JSON text from the file.
            try
            {
                string json = await File.ReadAllTextAsync(path);
                // TODO: Deserialise it into a CommitteeData object.
                CommitteeData? data = JsonSerializer.Deserialize<CommitteeData>(json);
                // TODO: Replace _decisions with the loaded decisions.
                if (data != null)
                    _decisions = data.Decisions;
                else
                    return;
                // TODO: Recalculate and redisplay the table, and call UpdateDecisionSummary().
                UpdateDecisionSummary();
                CalculateTable();
                DisplayTable();
                // TODO: Update CommitteeStatus.
                CommitteeStatus = "File Loaded";
            }
            catch (Exception ex)
            {
                CommitteeStatus = "Error in reading file: " + ex.Message;
            }
        }

        private void ChangeTheme_Clicked(object sender, EventArgs e)
        {
            _alternateTheme = !_alternateTheme;

            if (_alternateTheme)
            {
                Resources["PageBackground"] = Color.FromArgb("#FFF0E0");
            }
            else
            {
                Resources["PageBackground"] = Color.FromArgb("#F4F4F4");
            }
        }

        // ------------------------------------------------------------
        // The methods below are supplied. You do not need to change them.
        // ------------------------------------------------------------

        // Returns the team with the given name (ignoring upper and lower case),
        // or null if there is no such team.
        private Team? FindTeam(string name)
        {
            foreach (Team team in _teams)
            {
                if (string.Equals(team.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return team;
                }
            }

            return null;
        }

        private void ClearDecisions_Clicked(object sender, EventArgs e)
        {
            _decisions.Clear();

            CalculateTable();
            DisplayTable();
            UpdateDecisionSummary();

            CommitteeStatus = "All decisions cleared.";
        }

        private void CalculateTable()
        {
            // Reset adjusted points to the original points.
            foreach (Team team in _teams)
            {
                team.AdjustedPoints = team.Points;
            }

            // Apply every disciplinary decision.
            foreach (DisciplinaryRecord decision in _decisions)
            {
                foreach (Team team in _teams)
                {
                    if (team.Name == decision.TeamName)
                    {
                        team.AdjustedPoints += decision.PointsAdjustment;
                    }
                }
            }

            // Reorder the table.
            _teams.Sort(CompareTeams);
        }

        // Comparison helper used by List.Sort().
        // Sort by adjusted points, then goal difference.
        private int CompareTeams(Team first, Team second)
        {
            int result = second.AdjustedPoints.CompareTo(first.AdjustedPoints);

            if (result == 0)
            {
                result = second.GoalDifference.CompareTo(first.GoalDifference);
            }

            return result;
        }

        private void DisplayTable()
        {
            LeagueTable.Children.Clear();
            LeagueTable.RowDefinitions.Clear();

            // Header row.
            LeagueTable.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            AddCell("Pos", 0, 0, true);
            AddCell("Club", 1, 0, true);
            AddCell("P", 2, 0, true);
            AddCell("W", 3, 0, true);
            AddCell("D", 4, 0, true);
            AddCell("L", 5, 0, true);
            AddCell("GD", 6, 0, true);
            AddCell("Pts", 7, 0, true);

            int position = 1;

            foreach (Team team in _teams)
            {
                int row = position;
                LeagueTable.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                AddCell(position.ToString(), 0, row);
                AddCell(team.Name, 1, row);
                AddCell(team.Played.ToString(), 2, row);
                AddCell(team.Won.ToString(), 3, row);
                AddCell(team.Drawn.ToString(), 4, row);
                AddCell(team.Lost.ToString(), 5, row);
                AddCell(team.GoalDifference.ToString(), 6, row);
                AddCell(team.AdjustedPoints.ToString(), 7, row);

                position++;
            }
        }

        private void AddCell(string text, int column, int row, bool isHeader = false)
        {
            Label label = new Label
            {
                Text = text,
                FontSize = 12,
                FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = column == 1 ? TextAlignment.Start : TextAlignment.Center
            };

            LeagueTable.Add(label, column, row);
        }

        // Works out the total adjustment and the list of decisions, and puts them
        // into the two bound properties. Call this whenever _decisions changes.
        private void UpdateDecisionSummary()
        {
            int total = 0;
            string log = "";

            foreach (DisciplinaryRecord decision in _decisions)
            {
                total += decision.PointsAdjustment;
                log += decision.TeamName + " (" + decision.PointsAdjustment + "): " + decision.Reason + "\n";
            }

            if (log == "")
            {
                log = "No decisions yet.";
            }

            TotalAdjustment = total;
            DecisionLog = log.TrimEnd();
        }

        // The full path of decisions.json in the application's data folder.
        private string GetDecisionsPath()
        {
            return Path.Combine(FileSystem.Current.AppDataDirectory, "decisions.json");
        }
    }
}
