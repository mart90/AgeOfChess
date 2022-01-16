namespace AgeOfChess
{
    class Piece : PlaceableObject
    {
        public bool IsWhite { get; }

        public Piece(TextureLibrary textureLibrary, bool isWhite) : base(textureLibrary) 
        {
            IsWhite = isWhite;
        }
    }
}
