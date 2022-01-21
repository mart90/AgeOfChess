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
        TextNotification TextNotification { get; set; }

        private MultiplayerApiClient _apiClient;
        private FontLibrary _fontLibrary;

        public Menu(TextureLibrary textureLibrary, FontLibrary fontLibrary, MultiplayerApiClient apiClient)
        {
            CorrespondingUiState = AppUIState.InMenu;
            HeightPixels = 600;
            WidthPixels = 600;

            _apiClient = apiClient;
            _fontLibrary = fontLibrary;

            UiParts = new List<IUiPart>
            {
                new Button(textureLibrary, fontLibrary, new Rectangle(225, 200, 150, 35), ButtonType.SinglePlayer, "Single player"),
                new Button(textureLibrary, fontLibrary, new Rectangle(225, 240, 150, 35), ButtonType.Multiplayer, "Multiplayer"),
                new Button(textureLibrary, fontLibrary, new Rectangle(225, 280, 150, 35), ButtonType.Leaderboard, "Leaderboard")
            };
        }

        public void Update(SpriteBatch spriteBatch)
        {
            foreach (IUiPart button in UiParts)
            {
                button.Draw(spriteBatch);
            }

            if (TextNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, TextNotification.Message, new Vector2(20, HeightPixels - 60), TextNotification.Color);
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
                if (!_apiClient.RunningLatestVersion())
                {
                    TextNotification = new TextNotification
                    {
                        Color = Color.Red,
                        Message = "Server is running a newer version. Get the latest from the discord"
                    };
                }
                else
                {
                    NewUiState = AppUIState.InLoginScreen;
                }
            }
            else if (button.Type == ButtonType.Leaderboard)
            {
                NewUiState = AppUIState.ViewingLeaderboard;
            }
        }

        public void ReceiveKeyboardInput(TextInputEventArgs args)
        {
        }
    }
}
