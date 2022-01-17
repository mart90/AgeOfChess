namespace AgeOfChess
{
    class MultiplayerGame : Game
    {
        public int Id { get; set; }
        public int FromLobbyId { get; set; }
        public int WhiteTimeMs { get; set; }
        public int BlackTimeMs { get; set; }

        public MultiplayerGame(TextureLibrary textureLibrary, FontLibrary fontLibrary) : base(textureLibrary, fontLibrary)
        {

        }
    }
}
