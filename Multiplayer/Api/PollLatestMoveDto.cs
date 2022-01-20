namespace AgeOfChess
{
    class PollLatestMoveDto
    {
        public int MoveNumber { get; set; }
        public string SourceSquare { get; set; }
        public string DestinationSquare { get; set; }
        public string PiecePlaced { get; set; }
        public string ObjectCaptured { get; set; }
        public int? WhiteTimeMs { get; set; }
        public int? BlackTimeMs { get; set; }

        public Move ToMove()
        {
            return new Move(SourceSquare, DestinationSquare, PiecePlaced, ObjectCaptured);
        }
    }
}
