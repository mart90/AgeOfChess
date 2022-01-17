using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AgeOfChess
{
    class Menu : IUiWindow
    {
        public AppUIState CorrespondingUiState { get; }
        public List<Button> Buttons { get; set; }
        public AppUIState? NewUiState { get; set; }
        public int HeightPixels { get; }
        public int WidthPixels { get; }

        public Menu(TextureLibrary textureLibrary, FontLibrary fontLibrary)
        {
            CorrespondingUiState = AppUIState.InMenu;
            HeightPixels = 600;
            WidthPixels = 600;

            Buttons = new List<Button>
            {
                new Button(textureLibrary, fontLibrary, new Rectangle(225, 200, 150, 35), ButtonType.SinglePlayer, "Single player"),
                new Button(textureLibrary, fontLibrary, new Rectangle(225, 240, 150, 35), ButtonType.Multiplayer, "Multiplayer")
            };
        }

        public void HandleLeftMouseClick(Point location)
        {
            ClickButtonByLocation(location);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Button button in Buttons)
            {
                button.Draw(spriteBatch);
            }
        }

        public void ClickButtonByLocation(Point location)
        {
            var button = this.GetButtonByLocation(location);

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
                NewUiState = AppUIState.InLobbyBrowser;
            }
        }
    }
}
