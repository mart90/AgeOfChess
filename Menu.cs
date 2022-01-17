using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AgeOfChess
{
    class Menu : IUiWindow
    {
        public AppUIState CorrespondingUiState { get; }
        public List<IUiPart> UiParts { get; set; }
        public AppUIState? NewUiState { get; set; }
        public int HeightPixels { get; }
        public int WidthPixels { get; }

        public Menu(TextureLibrary textureLibrary, FontLibrary fontLibrary)
        {
            CorrespondingUiState = AppUIState.InMenu;
            HeightPixels = 600;
            WidthPixels = 600;

            UiParts = new List<IUiPart>
            {
                new Button(textureLibrary, fontLibrary, new Rectangle(225, 200, 150, 35), ButtonType.SinglePlayer, "Single player"),
                new Button(textureLibrary, fontLibrary, new Rectangle(225, 240, 150, 35), ButtonType.Multiplayer, "Multiplayer")
            };
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IUiPart button in UiParts)
            {
                button.Draw(spriteBatch);
            }
        }

        public void ClickUiPartByLocation(Point location)
        {
            var uiPart = this.GetUiPartByLocation(location);

            if (uiPart == null)
            {
                return;
            }

            Button button = uiPart is Button b ? b : null;

            if (button == null)
            {
                return;
            }

            if (button.Type == ButtonType.SinglePlayer)
            {
                NewUiState = AppUIState.CreatingSinglePlayerGame;
            }
            else if (button.Type == ButtonType.Multiplayer)
            {
                NewUiState = AppUIState.InLoginScreen;
            }
        }

        public void ReceiveKeyboardInput(TextInputEventArgs args)
        {
        }
    }
}
