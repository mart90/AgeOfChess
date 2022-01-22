namespace AgeOfChess
{
    public enum SquareType
    {
        Dirt,
        DirtRocks,
        DirtMine,
        DirtTrees,
        Grass,
        GrassRocks,
        GrassMine,
        GrassTrees
    }

    enum SquareColor
    {
        None,
        Blue,
        Red,
        Purple,
        Orange,
        Green
    }

    enum Direction
    {
        North = 0,
        NorthEast = 1,
        East = 2,
        SouthEast = 3,
        South = 4,
        SouthWest = 5,
        West = 6,
        NorthWest = 7
    }

    enum AppUIState
    {
        InMenu,
        CreatingSinglePlayerGame,
        InLoginScreen,
        InLobbyBrowser,
        InLobby,
        CreatingLobby,
        InGame,
        ViewingLeaderboard
    }

    enum GameState
    {
        Default,
        PieceSelected,
        PlacingPiece,
        Bidding,
        WaitingForOpponentBid
    }

    enum ButtonType
    {
        Default,
        Back,

        CopyMapSeed,
        BlackGoldIncrease,
        BlackGoldDecrease,

        SinglePlayer,
        Multiplayer,
        Leaderboard,
        
        StartGame,
        Login,
        Register,
        CreateLobby,
        JoinLobby,
        StartLobby,
        CancelLobby,

        PasteMapSeed,
        MapSizeIncrease,
        MapSizeDecrease,
        TimeControlToggle,
        StartTimeMinutesPlus1,
        StartTimeMinutesPlus10,
        StartTimeMinutesMinus1,
        StartTimeMinutesMinus10,
        TimeIncrementSecondsPlus1,
        TimeIncrementSecondsPlus10,
        TimeIncrementSecondsMinus1,
        TimeIncrementSecondsMinus10,

        BiddingToggle,
        SubmitBid,
        Resign
    }

    enum TextBoxType
    {
        Username,
        Password,
        MinRating,
        MaxRating,
        Bid
    }
}
