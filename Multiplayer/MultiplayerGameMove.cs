namespace AgeOfChess
{
    class MultiplayerGameMove
    {
        public int GameId { get; set; }
        public int MoveNumber { get; set; }
        public string SourceSquare { get; set; }
        public string DestinationSquare { get; set; }
        public string PiecePlaced { get; set; }
        public string ObjectCaptured { get; set; }
    }
}
