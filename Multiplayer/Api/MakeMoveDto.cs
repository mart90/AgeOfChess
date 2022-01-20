namespace AgeOfChess
{
    class MakeMoveDto
    {
        public int GameId { get; set; }
        public int MoveNumber { get; set; }
        public string SourceSquare { get; set; }
        public string DestinationSquare { get; set; }
        public string PiecePlaced { get; set; }
        public string ObjectCaptured { get; set; }
        public bool IsWhite { get; set; }
        public int? TimeMs { get; set; }
    }
}
