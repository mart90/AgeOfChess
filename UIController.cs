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

        private List<IUiComponent> _components;

        private TextureLibrary _textureLibrary;
        private FontLibrary _fontLibrary;

        private AppUIState _appUIState;

        private bool _leftMouseHeld;

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

            _appUIState = AppUIState.InMenu;
            _components = new List<IUiComponent>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _textureLibrary = new TextureLibrary(Content);
            _fontLibrary = new FontLibrary(Content);

            //var gs = new SinglePlayerGameSettings()
            //{
            //    MapSize = 12,
            //    TimeControlEnabled = false
            //};

            //NewSinglePlayerGame(gs);

            //_appUIState = AppUIState.InGame;

            _components.Add(new Menu(_textureLibrary, _fontLibrary));
            _components.Add(new SinglePlayerGameSettingsForm(_textureLibrary, _fontLibrary));
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

            GetActiveUiComponent().Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleLeftMouseClick(Point location)
        {
            IUiComponent component = GetActiveUiComponent();

            component.HandleLeftMouseClick(location);

            if (component.NewUiState != null)
            {
                _appUIState = component.NewUiState.Value;
                component.NewUiState = null;

                if (_appUIState == AppUIState.InGame && component is SinglePlayerGameSettingsForm form)
                {
                    _components.Add(new SinglePlayerGame((SinglePlayerGameSettings)form.GameSettings, _textureLibrary, _fontLibrary));
                }

                IUiComponent newComponent = GetActiveUiComponent();

                _graphics.PreferredBackBufferHeight = newComponent.WindowHeight;
                _graphics.PreferredBackBufferWidth = newComponent.WindowWidth;
                _graphics.ApplyChanges();
            }
        }

        private IUiComponent GetActiveUiComponent()
        {
            return _components.Single(e => e.CorrespondingUiState == _appUIState);
        }

        private void HandleTextInput(object sender, TextInputEventArgs args)
        {

        }
    }
}
