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
        List<IUiPart> UiParts { get; }
        void Update(SpriteBatch spriteBatch);
        void ClickUiPartByLocation(Point location);
        AppUIState? NewUiState { get; set; }
    }
}
