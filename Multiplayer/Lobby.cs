﻿using System;

namespace AgeOfChess
{
    class Lobby
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int HostId { get; set; }
        public DateTime HostLastPing { get; set; }
        public string Status { get; set; }

        public bool IsSelected { get; set; }

        public MultiplayerGameSettings Settings { get; set; }

        public string Title()
        {
            string mapStr = Settings.MapSeed != null ? "Seeded" : "Generated";

            string title = $"{Settings.BoardSize}x{Settings.BoardSize} {mapStr} - ";

            if (Settings.TimeControlEnabled)
            {
                title += $"{Settings.StartTimeMinutes}+{Settings.TimeIncrementSeconds} - ";
            }
            else
            {
                title += "Unlimited time - ";
            }

            title += Settings.BiddingEnabled ? "Bidding - " : "No bidding - ";

            title += $"Rating {Settings.MinRating}-{Settings.MaxRating}";

            return title;
        }
    }
}
