using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AgeOfChess
{
    class PlaceableObject
    {
        protected Texture2D Texture;
        protected TextureLibrary TextureLibrary;

        public PlaceableObject(TextureLibrary textureLibrary)
        {
            TextureLibrary = textureLibrary;
            Texture = TextureLibrary.GetObjectTextureByType(GetType());
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle location)
        {
            spriteBatch.Draw(Texture, location, Color.White);
        }
    }
}
