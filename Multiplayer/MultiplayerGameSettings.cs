namespace AgeOfChess
{
    class MultiplayerGameSettings : GameSettings
    {
        public bool BiddingEnabled { get; set; }
        public int MinRating { get; set; }
        public int MaxRating { get; set; }

        public MultiplayerGameSettings()
        {
            MinRating = 1000;
            MaxRating = 2000;
        }
    }
}
