using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace AgeOfChess
{
    class LobbyBrowser : IUiWindow
    {
        public int HeightPixels { get; }
        public int WidthPixels { get; }
        public AppUIState CorrespondingUiState { get; }
        public List<IUiPart> UiParts { get; }
        public AppUIState? NewUiState { get; set; }
        public User AuthenticatedUser { get; set; }
        public TextNotification TextNotification { get; private set; }
        public List<Lobby> Lobbies { get; private set; }

        private readonly FontLibrary _fontLibrary;
        private readonly MultiplayerApiClient _apiClient;

        public LobbyBrowser(TextureLibrary textureLibrary, FontLibrary fontLibrary, MultiplayerApiClient apiClient)
        {
            CorrespondingUiState = AppUIState.InLobbyBrowser;

            HeightPixels = 600;
            WidthPixels = 600;

            UiParts = new List<IUiPart>
            {

                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 420, HeightPixels - 70, 120, 35), ButtonType.Back, "Back"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 290, HeightPixels - 70, 120, 35), ButtonType.JoinLobby, "Join"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 160, HeightPixels - 70, 120, 35), ButtonType.CreateLobby, "Create")
            };

            _apiClient = apiClient;
            _fontLibrary = fontLibrary;
        }

        public void ClickUiPartByLocation(Point location)
        {
            ClearSelection();

            var uiPart = this.GetUiPartByLocation(location);

            if (uiPart == null)
            {
                return;
            }

            if (uiPart is TextBox textBox)
            {
                textBox.Focus();
            }
            else if (uiPart is Button button)
            {
                if (button.Type == ButtonType.Back)
                {
                    NewUiState = AppUIState.InMenu;
                }
                else if (button.Type == ButtonType.CreateLobby)
                {
                    NewUiState = AppUIState.CreatingLobby;
                }
            }
        }

        public void ClearSelection()
        {
            TextNotification = null;

            foreach (IUiPart uiPart in UiParts.Where(e => e is TextBox))
            {
                ((TextBox)uiPart).HasFocus = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IUiPart uiPart in UiParts)
            {
                uiPart.Draw(spriteBatch);
            }

            if (TextNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, TextNotification.Message, new Vector2(20, HeightPixels - 60), TextNotification.Color);
            }
        }

        public void ReceiveKeyboardInput(TextInputEventArgs args)
        {
        }
    }
}
