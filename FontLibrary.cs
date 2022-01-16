using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AgeOfChess
{
    class FontLibrary
    {
        public SpriteFont DefaultFont { get; }
        public SpriteFont Size5Font { get; }

        public FontLibrary(ContentManager contentManager)
        {
            DefaultFont = contentManager.Load<SpriteFont>("fonts/defaultFont");
            Size5Font = contentManager.Load<SpriteFont>("fonts/size5");
        }
    }
}
