using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

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
            TargetElapsedTime = new System.TimeSpan(100000); // 100 updates p/s
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

            _windows.Add(new Menu(_textureLibrary, _fontLibrary));
            _windows.Add(new SinglePlayerGameSettingsForm(_textureLibrary, _fontLibrary));
            _windows.Add(new LoginScreen(_textureLibrary, _fontLibrary, _apiClient));
            _windows.Add(new LobbyBrowser(_textureLibrary, _fontLibrary, _apiClient));
            _windows.Add(new CreateLobbyForm(_textureLibrary, _fontLibrary, _apiClient));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            GetActiveUiWindow().Draw(_spriteBatch);

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

                if (_appUIState == AppUIState.InGame && window is SinglePlayerGameSettingsForm form)
                {
                    _windows.Add(new SinglePlayerGame((SinglePlayerGameSettings)form.GameSettings, _textureLibrary, _fontLibrary));
                }
                else if (_appUIState == AppUIState.InLobbyBrowser && window is LoginScreen loginScreen)
                {
                    var lobbyBrowser = (LobbyBrowser)_windows.Single(e => e is LobbyBrowser);
                    lobbyBrowser.AuthenticatedUser = loginScreen.AuthenticatedUser;
                }
                else if (_appUIState == AppUIState.InLoginScreen && window is Menu)
                {
                    var lobbyBrowser = (LobbyBrowser)_windows.Single(e => e is LobbyBrowser);
                    if (lobbyBrowser.AuthenticatedUser != null)
                    {
                        _appUIState = AppUIState.InLobbyBrowser;
                    }
                }

                IUiWindow newActiveWindow = GetActiveUiWindow();

                _graphics.PreferredBackBufferHeight = newActiveWindow.HeightPixels;
                _graphics.PreferredBackBufferWidth = newActiveWindow.WidthPixels;
                _graphics.ApplyChanges();
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
    }
}
