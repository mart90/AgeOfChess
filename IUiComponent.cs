using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AgeOfChess
{
    interface IUiComponent
    {
        AppUIState CorrespondingUiState { get; }
        List<Button> Buttons { get; }
        void HandleLeftMouseClick(Point location);
        void Draw(SpriteBatch spriteBatch);
        void ClickButtonByLocation(Point location);
        AppUIState? NewUiState { get; set; }
        int WindowHeight { get; }
        int WindowWidth { get; }
    }
}
