using System.Collections.Generic;

namespace AgeOfChess
{
    class LeaderboardDto
    {
        public List<LeaderboardEntryDto> Entries { get; set; }
        public int? TotalUsers { get; set; }
        public int? OurRank { get; set; }
    }

    class LeaderboardEntryDto
    {
        public string Username { get; set; }
        public double Rating { get; set; }
    }
}
