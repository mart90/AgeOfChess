using System;

namespace AgeOfChess
{
    class Lobby
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int HostId { get; set; }
        public DateTime HostLastPing { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int BoardSize { get; set; }
        public bool BiddingEnabled { get; set; }
        public bool TimeControlEnabled { get; set; }
        public int StartTimeMinutes { get; set; }
        public int TimeIncrementSeconds { get; set; }
        public int MinRating { get; set; }
        public int MaxRating { get; set; }
        public string MapSeed { get; set; }
    }
}
