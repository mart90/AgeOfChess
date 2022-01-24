using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace AgeOfChess
{
    class CreateLobbyForm : GameSettingsForm, IUiWindow
    {
        public Lobby CreatedLobby { get; set; }

        private readonly FontLibrary _fontLibrary;
        private readonly MultiplayerApiClient _apiClient;

        public CreateLobbyForm(TextureLibrary textureLibrary, FontLibrary fontLibrary, MultiplayerApiClient apiClient) : base(textureLibrary, fontLibrary)
        {
            CorrespondingUiState = AppUIState.CreatingLobby;

            HeightPixels = 600;
            WidthPixels = 600;

            GameSettings = new MultiplayerGameSettings();

            UiParts.AddRange(new List<IUiPart>
            {
                new Button(textureLibrary, fontLibrary, new Rectangle(150, 370, 60, 35), ButtonType.BiddingToggle, "Toggle"),
                new TextBox(textureLibrary, fontLibrary, new Rectangle(110, 420, 100, 25), TextBoxType.MinRating, "Min rating", "1000"),
                new TextBox(textureLibrary, fontLibrary, new Rectangle(110, 450, 100, 25), TextBoxType.MaxRating, "Max rating", "2000"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 290, HeightPixels - 70, 120, 35), ButtonType.Back, "Back"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 160, HeightPixels - 70, 120, 35), ButtonType.StartLobby, "Create")
            });

            _apiClient = apiClient;
            _fontLibrary = fontLibrary;
        }

        public MultiplayerGameSettings Settings => (MultiplayerGameSettings)GameSettings;

        public override void ClickUiPartByLocation(Point location)
        {
            ClearSelection();

            base.ClickUiPartByLocation(location);

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
                if (button.Type == ButtonType.StartLobby)
                {
                    CreateLobby();

                    NewUiState = AppUIState.InLobbyBrowser;
                }
                else if (button.Type == ButtonType.Back)
                {
                    NewUiState = AppUIState.InLobbyBrowser;
                }
                else if (button.Type == ButtonType.BiddingToggle)
                {
                    Settings.BiddingEnabled = !Settings.BiddingEnabled;
                }
            }
        }

        public void CreateLobby()
        {
            string minRatingStr = this.TextBoxValueByType(TextBoxType.MinRating);

            if (!int.TryParse(minRatingStr, out int minRating))
            {
                TextNotification = new TextNotification
                {
                    Color = Color.Red,
                    Message = "Invalid minimum rating"
                };
                return;
            }

            string maxRatingStr = this.TextBoxValueByType(TextBoxType.MaxRating);

            if (!int.TryParse(maxRatingStr, out int maxRating))
            {
                TextNotification = new TextNotification
                {
                    Color = Color.Red,
                    Message = "Invalid maximum rating"
                };
                return;
            }

            Settings.MinRating = minRating;
            Settings.MaxRating = maxRating;

            CreatedLobby = new Lobby
            {
                Id = _apiClient.CreateLobby(Settings),
                Settings = Settings
            };
        }

        public void ClearSelection()
        {
            TextNotification = null;

            foreach (IUiPart uiPart in UiParts.Where(e => e is TextBox))
            {
                ((TextBox)uiPart).HasFocus = false;
            }
        }

        public override void Update(SpriteBatch spriteBatch)
        {
            base.Update(spriteBatch);

            var settings = (MultiplayerGameSettings)GameSettings;

            spriteBatch.DrawString(_fontLibrary.DefaultFontBold, "----------------- Multiplayer settings -----------------", new Vector2(20, 340), Color.Black);

            string biddingSetting = settings.BiddingEnabled ? "Enabled" : "Disabled";
            spriteBatch.DrawString(_fontLibrary.DefaultFont, $"Bidding: {biddingSetting}", new Vector2(20, 380), Color.Black);
        }
    }
}
