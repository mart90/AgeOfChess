namespace AgeOfChess
{
    abstract class GameSettings
    {
        public int? BoardSize { get; set; }
        public bool TimeControlEnabled { get; set; }
        public int? StartTimeMinutes { get; set; }
        public int? TimeIncrementSeconds { get; set; }
        public string MapSeed { get; set; }

        public GameSettings()
        {
            BoardSize = 12;
            TimeControlEnabled = true;
            StartTimeMinutes = 5;
            TimeIncrementSeconds = 5;
        }
    }
}
