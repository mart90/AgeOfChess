namespace AgeOfChess
{
    class PieceColor
    {
        public bool IsWhite { get; }
        public bool IsActive { get; set; }
        public int Gold { get; set; }
        public int TimeMiliseconds { get; set; }
        public string PlayedByStr { get; set; }

        public bool IsUs { get; set; }

        public PieceColor(bool isWhite, string playedByStr)
        {
            IsWhite = isWhite;
            PlayedByStr = playedByStr;
            IsActive = isWhite;

            Gold = IsWhite ? 0 : 10;
        }
    }
}
