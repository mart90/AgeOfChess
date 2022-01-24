using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AgeOfChess
{
    public class UIController : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private List<IUiWindow> _windows;

        private TextureLibrary _textureLibrary;
        private FontLibrary _fontLibrary;

        private AppUIState _appUIState;

        private bool _leftMouseHeld;

        private MultiplayerApiClient _apiClient;

        public UIController()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TargetElapsedTime = new TimeSpan(100000); // 100 updates p/s
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.ApplyChanges();

            IsMouseVisible = true;
            Window.TextInput += HandleTextInput;

            _apiClient = new MultiplayerApiClient();

            _appUIState = AppUIState.InMenu;
            _windows = new List<IUiWindow>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _textureLibrary = new TextureLibrary(Content);
            _fontLibrary = new FontLibrary(Content);

            _windows.Add(new Menu(_textureLibrary, _fontLibrary, _apiClient));
            _windows.Add(new SinglePlayerGameSettingsForm(_textureLibrary, _fontLibrary));
            _windows.Add(new LoginScreen(_textureLibrary, _fontLibrary, _apiClient));
            _windows.Add(new LobbyBrowser(_textureLibrary, _fontLibrary, _apiClient));
            _windows.Add(new CreateLobbyForm(_textureLibrary, _fontLibrary, _apiClient));
            _windows.Add(new Leaderboard(_textureLibrary, _fontLibrary, _apiClient));
        }

        protected override void Update(GameTime gameTime)
        {
            if (_appUIState == AppUIState.InLobbyBrowser)
            {
                var lobbyBrowser = (LobbyBrowser)GetActiveUiWindow();
                if (lobbyBrowser.NewUiState == AppUIState.InGame)
                {
                    _appUIState = AppUIState.InGame;
                    _windows.Add(lobbyBrowser.CreatedGame);
                    lobbyBrowser.NewUiState = null;

                    RefreshDimensions();

                    BringToForeGround();
                }
            }

            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Released && _leftMouseHeld)
            {
                _leftMouseHeld = false;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && !_leftMouseHeld)
            {
                _leftMouseHeld = true;
                HandleLeftMouseClick(mouseState.Position);
            }

            base.Update(gameTime);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void BringToForeGround()
        {
            Process[] processes = Process.GetProcessesByName("AgeOfChess");
            SetForegroundWindow(processes[0].MainWindowHandle);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            GetActiveUiWindow().Update(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleLeftMouseClick(Point location)
        {
            IUiWindow window = GetActiveUiWindow();

            window.ClickUiPartByLocation(location);

            if (window.NewUiState != null)
            {
                _appUIState = window.NewUiState.Value;
                window.NewUiState = null;

                if (_appUIState == AppUIState.InGame && window is SinglePlayerGameSettingsForm gameSettingsForm)
                {
                    _windows.Add(new SinglePlayerGame((SinglePlayerGameSettings)gameSettingsForm.GameSettings, _textureLibrary, _fontLibrary));
                }
                else if (_appUIState == AppUIState.InLoginScreen)
                {
                    if (_apiClient.AuthenticatedUser != null)
                    {
                        _appUIState = AppUIState.InLobbyBrowser;
                    }
                }
                else if (_appUIState == AppUIState.InLobbyBrowser && window is CreateLobbyForm createLobbyForm)
                {
                    var lobbyBrowser = (LobbyBrowser)_windows.Single(e => e is LobbyBrowser);
                    lobbyBrowser.CreatedLobby = createLobbyForm.CreatedLobby;
                    createLobbyForm.CreatedLobby = null;
                }
                else if (_appUIState == AppUIState.InGame && window is LobbyBrowser lobbyBrowser)
                {
                    _windows.Add(lobbyBrowser.CreatedGame);
                }
                else if (_appUIState == AppUIState.InMenu && window is Game)
                {
                    if (window is MultiplayerGame)
                    {
                        // Get new ELO
                        _apiClient.RefreshUser();
                    }

                    _windows.RemoveAll(e => e is Game);
                }
                else if (_appUIState == AppUIState.ViewingLeaderboard)
                {
                    ((Leaderboard)_windows.Single(e => e is Leaderboard)).IsRefreshed = false;
                }

                RefreshDimensions();
            }
        }

        private IUiWindow GetActiveUiWindow()
        {
            return _windows.Single(e => e.CorrespondingUiState == _appUIState);
        }

        private void HandleTextInput(object sender, TextInputEventArgs args)
        {
            GetActiveUiWindow().ReceiveKeyboardInput(args);
        }

        private void RefreshDimensions()
        {
            IUiWindow activeWindow = GetActiveUiWindow();

            _graphics.PreferredBackBufferHeight = activeWindow.HeightPixels;
            _graphics.PreferredBackBufferWidth = activeWindow.WidthPixels;
            _graphics.ApplyChanges();
        }
    }
}
