using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AgeOfChess
{
    interface IUiWindow
    {
        int HeightPixels { get; }
        int WidthPixels { get; }
        AppUIState CorrespondingUiState { get; }
        List<Button> Buttons { get; }
        void HandleLeftMouseClick(Point location);
        void Draw(SpriteBatch spriteBatch);
        void ClickButtonByLocation(Point location);
        AppUIState? NewUiState { get; set; }
    }
}
