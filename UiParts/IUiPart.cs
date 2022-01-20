using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AgeOfChess
{
    interface IUiPart
    {
        void Draw(SpriteBatch spriteBatch);
        Point Center { get; }
        bool LocationIncludesPoint(Point point);
        bool IsEnabled { get; set; }
    }
}
