using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AgeOfChess
{
    class PlacePieceButton : Button
    {
        public Type PieceType { get; }
        public Texture2D WhitePieceTexture { get; }
        public Texture2D BlackPieceTexture { get; }
        public int PieceCost { get; }

        public PlacePieceButton(TextureLibrary textureLibrary, FontLibrary fontLibrary, Rectangle location, Type pieceType) : base(textureLibrary, fontLibrary, location, ButtonType.Default)
        {
            PieceType = pieceType;

            if (pieceType == typeof(Queen))
            {
                WhitePieceTexture = textureLibrary.GetObjectTextureByType(typeof(WhiteQueen));
                BlackPieceTexture = textureLibrary.GetObjectTextureByType(typeof(BlackQueen));
                PieceCost = 100;
            }
            else if (pieceType == typeof(Rook))
            {
                WhitePieceTexture = textureLibrary.GetObjectTextureByType(typeof(WhiteRook));
                BlackPieceTexture = textureLibrary.GetObjectTextureByType(typeof(BlackRook));
                PieceCost = 45;
            }
            else if (pieceType == typeof(Bishop))
            {
                WhitePieceTexture = textureLibrary.GetObjectTextureByType(typeof(WhiteBishop));
                BlackPieceTexture = textureLibrary.GetObjectTextureByType(typeof(BlackBishop));
                PieceCost = 35;
            }
            else if (pieceType == typeof(Knight))
            {
                WhitePieceTexture = textureLibrary.GetObjectTextureByType(typeof(WhiteKnight));
                BlackPieceTexture = textureLibrary.GetObjectTextureByType(typeof(BlackKnight));
                PieceCost = 40;
            }
            else if (pieceType == typeof(Pawn))
            {
                WhitePieceTexture = textureLibrary.GetObjectTextureByType(typeof(WhitePawn));
                BlackPieceTexture = textureLibrary.GetObjectTextureByType(typeof(BlackPawn));
                PieceCost = 30;
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool activePlayerIsWhite)
        {
            base.Draw(spriteBatch);

            spriteBatch.DrawString(FontLibrary.DefaultFont, PieceCost.ToString(), new Vector2(TextLocation.X, TextLocation.Y), Microsoft.Xna.Framework.Color.White);
            spriteBatch.Draw(activePlayerIsWhite ? WhitePieceTexture : BlackPieceTexture, ObjectLocation, Microsoft.Xna.Framework.Color.White);
        }
    }
}
