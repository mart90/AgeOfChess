namespace AgeOfChess
{
    class MultiplayerGameSettings : GameSettings
    {
        public bool BiddingEnabled { get; set; }
        public int MinRating { get; set; }
        public int MaxRating { get; set; }
    }
}
