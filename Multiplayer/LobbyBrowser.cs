using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AgeOfChess
{
    class LobbyBrowser : IUiWindow
    {
        public int HeightPixels { get; }
        public int WidthPixels { get; }
        public AppUIState CorrespondingUiState { get; }
        public List<IUiPart> UiParts { get; }
        public AppUIState? NewUiState { get; set; }
        public TextNotification TextNotification { get; private set; }
        public List<Lobby> Lobbies { get; private set; }
        public Lobby CreatedLobby { get; set; }
        public DateTime LastRefresh { get; set; }

        public MultiplayerGame CreatedGame { get; private set; }

        private readonly TextureLibrary _textureLibrary;
        private readonly FontLibrary _fontLibrary;
        private readonly MultiplayerApiClient _apiClient;

        public LobbyBrowser(TextureLibrary textureLibrary, FontLibrary fontLibrary, MultiplayerApiClient apiClient)
        {
            CorrespondingUiState = AppUIState.InLobbyBrowser;

            HeightPixels = 600;
            WidthPixels = 600;

            UiParts = new List<IUiPart>
            {
                new Button(textureLibrary, fontLibrary, new Rectangle(20, HeightPixels - 70, 120, 35), ButtonType.CancelLobby, "Stop hosting", false),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 420, HeightPixels - 70, 120, 35), ButtonType.Back, "Back"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 290, HeightPixels - 70, 120, 35), ButtonType.JoinLobby, "Join"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 160, HeightPixels - 70, 120, 35), ButtonType.CreateLobby, "Create")
            };

            _apiClient = apiClient;
            _textureLibrary = textureLibrary;
            _fontLibrary = fontLibrary;

            Lobbies = new List<Lobby>();
        }

        public void ClickUiPartByLocation(Point location)
        {
            Lobby selectedLobby = Lobbies.SingleOrDefault(e => e.IsSelected);

            ClearSelection();

            var uiPart = this.GetUiPartByLocation(location);

            if (uiPart == null)
            {
                SelectLobbyByLocation(location);
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
                    LastRefresh = DateTime.Now.AddMinutes(-1);
                }
                else if (button.Type == ButtonType.CancelLobby)
                {
                    CancelLobby();
                    Refresh();
                }
                else if (button.Type == ButtonType.JoinLobby && selectedLobby != null)
                {
                    JoinLobby(selectedLobby);
                }
            }
        }

        private void SelectLobbyByLocation(Point location)
        {
            int lobbyIndex = (int)Math.Floor((double)(location.Y - 50) / 30);

            if (lobbyIndex < 0 || Lobbies.Count <= lobbyIndex)
            {
                return;
            }

            Lobbies[lobbyIndex].IsSelected = true;
        }

        private void Refresh()
        {
            Lobbies = _apiClient.GetActiveLobbies();
            LastRefresh = DateTime.Now;
        }

        private void CancelLobby()
        {
            _apiClient.CancelLobby(CreatedLobby.Id);
            CreatedLobby = null;
        }

        public void ClearSelection()
        {
            TextNotification = null;

            foreach (IUiPart uiPart in UiParts.Where(e => e is TextBox))
            {
                ((TextBox)uiPart).HasFocus = false;
            }

            Lobbies.ForEach(e => e.IsSelected = false);
        }

        public bool PlayerJoinedMyLobby()
        {
            return CreatedLobby != null && !Lobbies.Any(e => e.Id == CreatedLobby.Id);
        }

        public void HandlePlayerJoined()
        {
            if (CreatedLobby.Settings.MapSeed == null)
            {
                // Joining player generated the map. Get the seed from server
                CreatedLobby.Settings.MapSeed = _apiClient.GetGeneratedMapSeed(CreatedLobby.Id);
            }

            CreatedGame = new MultiplayerGame(_textureLibrary, _fontLibrary, _apiClient, CreatedLobby.Settings)
            {
                Id = _apiClient.GetGameId(CreatedLobby.Id)
            };

            Thread.Sleep(1000);

            CreatedGame.SetOpponent();

            NewUiState = AppUIState.InGame;

            CreatedLobby = null;
        }

        private void JoinLobby(Lobby lobby)
        {
            CreatedGame = new MultiplayerGame(_textureLibrary, _fontLibrary, _apiClient, lobby.Settings);

            lobby.Settings.MapSeed = CreatedGame.MapSeed;

            CreatedGame.Id = _apiClient.JoinLobby(lobby);
            CreatedGame.SetOpponent();

            NewUiState = AppUIState.InGame;
        }

        public void Update(SpriteBatch spriteBatch)
        {
            if ((DateTime.Now - LastRefresh).TotalSeconds > 10)
            {
                Refresh();
            }

            IUiPart stopHostingButton = UiParts.Single(e => e is Button btn && btn.Type == ButtonType.CancelLobby);
            stopHostingButton.IsEnabled = CreatedLobby != null;

            spriteBatch.DrawString(_fontLibrary.DefaultFontBold, "------------- Open lobbies -------------", new Vector2(20, 20), Color.Black);
            spriteBatch.DrawString(_fontLibrary.DefaultFont, $"Logged in as: {_apiClient.AuthenticatedUser.Username} ({_apiClient.AuthenticatedUser.LastElo})", new Vector2(280, 20), Color.Black);

            for (int rowNumber = 0; rowNumber < Lobbies.Count; rowNumber++)
            {
                Lobby lobby = Lobbies[rowNumber];

                var spriteFont = lobby.IsSelected ? _fontLibrary.DefaultFontBold : _fontLibrary.DefaultFont;

                spriteBatch.DrawString(spriteFont, lobby.Title(), new Vector2(20, 60 + rowNumber * 30), Color.Black);
            }

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
