using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace AgeOfChess
{
    class LoginScreen : IUiWindow
    {
        public int HeightPixels { get; }
        public int WidthPixels { get; }
        public AppUIState CorrespondingUiState { get; }
        public List<IUiPart> UiParts { get; }
        public AppUIState? NewUiState { get; set; }
        public TextNotification TextNotification { get; private set; }

        private readonly FontLibrary _fontLibrary;
        private readonly MultiplayerApiClient _apiClient;

        public LoginScreen(TextureLibrary textureLibrary, FontLibrary fontLibrary, MultiplayerApiClient apiClient)
        {
            CorrespondingUiState = AppUIState.InLoginScreen;

            HeightPixels = 600;
            WidthPixels = 600;

            UiParts = new List<IUiPart>
            {
                new TextBox(textureLibrary, fontLibrary, new Rectangle(275, 200, 100, 25), TextBoxType.Username, "Username "),
                new TextBox(textureLibrary, fontLibrary, new Rectangle(275, 240, 100, 25), TextBoxType.Password, "Password "),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 420, HeightPixels - 70, 120, 35), ButtonType.Back, "Back"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 290, HeightPixels - 70, 120, 35), ButtonType.Login, "Login"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 160, HeightPixels - 70, 120, 35), ButtonType.Register, "Register")
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
                else if (button.Type == ButtonType.Login)
                {
                    TryLogin();
                }
                else if (button.Type == ButtonType.Register)
                {
                    TryRegister();
                }
            }
        }

        public void TryRegister()
        {
            string username = this.TextBoxValueByType(TextBoxType.Username);
            string plainTextPassword = this.TextBoxValueByType(TextBoxType.Password);

            if (username == "" || plainTextPassword == "")
            {
                return;
            }

            if (username.Length > 12)
            {
                TextNotification = new TextNotification
                {
                    Color = Color.Red,
                    Message = "Username too long. Max 12 characters"
                };
                return;
            }

            var existingUser = _apiClient.GetUserIdByName(username);

            if (existingUser != null)
            {
                TextNotification = new TextNotification
                {
                    Color = Color.Red,
                    Message = "Username is taken"
                };
                return;
            }

            _apiClient.RegisterUser(username, plainTextPassword);

            NewUiState = AppUIState.InLobbyBrowser;
        }

        public void TryLogin()
        {
            string username = this.TextBoxValueByType(TextBoxType.Username);
            string plainTextPassword = this.TextBoxValueByType(TextBoxType.Password);

            bool loginSuccess = _apiClient.Login(username, plainTextPassword);

            if (loginSuccess)
            {
                NewUiState = AppUIState.InLobbyBrowser;
            }
            else
            {
                TextNotification = new TextNotification
                {
                    Color = Color.Red,
                    Message = "Login failed"
                };
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

        public void Update(SpriteBatch spriteBatch)
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
    }
}
