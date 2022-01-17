using System;

namespace AgeOfChess
{
    class Lobby
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int HostId { get; set; }
        public DateTime HostLastPing { get; set; }
        public string Status { get; set; }
        public bool IsSelected { get; set; }

        public MultiplayerGameSettings Settings { get; set; }
    }
}
