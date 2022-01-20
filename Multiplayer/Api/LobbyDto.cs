using System;

namespace AgeOfChess
{
    class LobbyDto
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int HostId { get; set; }
        public DateTime HostLastPing { get; set; }
        public string Status { get; set; }
        public int? BoardSize { get; set; }
        public bool TimeControlEnabled { get; set; }
        public int? StartTimeMinutes { get; set; }
        public int? TimeIncrementSeconds { get; set; }
        public string MapSeed { get; set; }
        public bool BiddingEnabled { get; set; }
        public int MinRating { get; set; }
        public int MaxRating { get; set; }

        public Lobby ToLobby()
        {
            return new Lobby
            {
                Id = Id,
                CreatedAt = CreatedAt,
                HostId = HostId,
                HostLastPing = HostLastPing,
                Status = Status,
                Settings = new MultiplayerGameSettings
                {
                    BoardSize = BoardSize,
                    TimeControlEnabled = TimeControlEnabled,
                    StartTimeMinutes = StartTimeMinutes,
                    TimeIncrementSeconds = TimeIncrementSeconds,
                    MapSeed = MapSeed,
                    BiddingEnabled = BiddingEnabled,
                    MinRating = MinRating,
                    MaxRating = MaxRating
                }
            };
        }
    }
}
