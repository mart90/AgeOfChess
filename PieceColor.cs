using System;

namespace AgeOfChess
{
    class PieceColor
    {
        public bool IsWhite { get; }
        public bool IsActive { get; set; }
        public int Gold { get; set; }
        public int TimeMiliseconds { get; set; }
        public string PlayedBy { get; }

        public PieceColor(bool isWhite, string playedBy)
        {
            IsWhite = isWhite;
            PlayedBy = playedBy;
            IsActive = isWhite;

            Gold = IsWhite ? 0 : 10;
        }
    }
}
