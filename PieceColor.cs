namespace AgeOfChess
{
    class PieceColor
    {
        public bool IsWhite { get; }
        public bool IsActive { get; set; }
        public int Gold { get; set; }
        public int TimeMiliseconds { get; set; }

        public PieceColor(bool isWhite)
        {
            IsWhite = isWhite;
            IsActive = isWhite;

            Gold = IsWhite ? 0 : 10;
        }
    }
}
