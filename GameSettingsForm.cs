using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TextCopy;

namespace AgeOfChess
{
    abstract class GameSettingsForm : IUiComponent
    {
        public AppUIState CorrespondingUiState { get; protected set; }
        public List<Button> Buttons { get; }
        public AppUIState? NewUiState { get; set; }
        public GameSettings GameSettings { get; set; }
        public TextNotification TextNotification { get; set; }
        public int WindowHeight { get; }
        public int WindowWidth { get; }

        private readonly TextureLibrary _textureLibrary;
        private readonly FontLibrary _fontLibrary;

        public GameSettingsForm(TextureLibrary textureLibrary, FontLibrary fontLibrary)
        {
            _fontLibrary = fontLibrary;
            WindowHeight = 600;
            WindowWidth = 600;

            Buttons = new List<Button>()
            {
                new Button(textureLibrary, fontLibrary, new Rectangle(90, 90, 25, 22), ButtonType.MapSizeIncrease, "+1"),
                new Button(textureLibrary, fontLibrary, new Rectangle(120, 90, 25, 22), ButtonType.MapSizeDecrease, "-1"),
                new Button(textureLibrary, fontLibrary, new Rectangle(20, 130, 280, 35), ButtonType.PasteMapSeed, "Load map seed from clipboard"),
                new Button(textureLibrary, fontLibrary, new Rectangle(180, 190, 60, 35), ButtonType.TimeControlToggle, "Toggle"),
                new Button(textureLibrary, fontLibrary, new Rectangle(200, 235, 25, 22), ButtonType.StartTimeMinutesPlus1, "+1"),
                new Button(textureLibrary, fontLibrary, new Rectangle(230, 235, 40, 22), ButtonType.StartTimeMinutesPlus10, "+10"),
                new Button(textureLibrary, fontLibrary, new Rectangle(275, 235, 25, 22), ButtonType.StartTimeMinutesMinus1, "-1"),
                new Button(textureLibrary, fontLibrary, new Rectangle(305, 235, 40, 22), ButtonType.StartTimeMinutesMinus10, "-10"),
                new Button(textureLibrary, fontLibrary, new Rectangle(200, 265, 25, 22), ButtonType.TimeIncrementSecondsPlus1, "+1"),
                new Button(textureLibrary, fontLibrary, new Rectangle(230, 265, 40, 22), ButtonType.TimeIncrementSecondsPlus10, "+10"),
                new Button(textureLibrary, fontLibrary, new Rectangle(275, 265, 25, 22), ButtonType.TimeIncrementSecondsMinus1, "-1"),
                new Button(textureLibrary, fontLibrary, new Rectangle(305, 265, 40, 22), ButtonType.TimeIncrementSecondsMinus10, "-10"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WindowWidth - 290, WindowHeight - 70, 120, 35), ButtonType.Back, "Back"),
                new Button(textureLibrary, fontLibrary, new Rectangle(WindowWidth - 160, WindowHeight - 70, 120, 35), ButtonType.StartGame, "Start game")
            };
        }

        public void ClickButtonByLocation(Point location)
        {
            TextNotification = null;

            var button = this.GetButtonByLocation(location);

            if (button == null)
            {
                return;
            }

            if (button.Type == ButtonType.MapSizeIncrease)
            {
                IncreaseMapSize();
            }
            else if (button.Type == ButtonType.MapSizeDecrease)
            {
                DecreaseMapSize();
            }
            else if (button.Type == ButtonType.PasteMapSeed)
            {
                TryLoadSeedFromClipBoard();
            }
            else if (button.Type == ButtonType.TimeControlToggle)
            {
                GameSettings.TimeControlEnabled = !GameSettings.TimeControlEnabled;

                var buttons = Buttons
                    .Where(e => new List<ButtonType>
                    {
                        ButtonType.StartTimeMinutesPlus1,
                        ButtonType.StartTimeMinutesPlus10,
                        ButtonType.StartTimeMinutesMinus1,
                        ButtonType.StartTimeMinutesMinus10,
                        ButtonType.TimeIncrementSecondsPlus1,
                        ButtonType.TimeIncrementSecondsPlus10,
                        ButtonType.TimeIncrementSecondsMinus1,
                        ButtonType.TimeIncrementSecondsMinus10
                    }
                    .Contains(e.Type));

                if (!GameSettings.TimeControlEnabled)
                {
                    foreach (Button btn in buttons)
                    {
                        btn.IsEnabled = false;
                    }
                }
                else
                {
                    foreach (Button btn in buttons)
                    {
                        btn.IsEnabled = true;
                    }
                }
            }
            else if (button.Type == ButtonType.StartTimeMinutesPlus1)
            {
                GameSettings.StartTimeMinutes++;
            }
            else if (button.Type == ButtonType.StartTimeMinutesPlus10)
            {
                GameSettings.StartTimeMinutes += 10;
            }
            else if (button.Type == ButtonType.StartTimeMinutesMinus1)
            {
                if (GameSettings.StartTimeMinutes > 1)
                {
                    GameSettings.StartTimeMinutes--;
                }
            }
            else if (button.Type == ButtonType.StartTimeMinutesMinus10)
            {
                if (GameSettings.StartTimeMinutes > 11)
                {
                    GameSettings.StartTimeMinutes -= 10;
                }
            }
            else if (button.Type == ButtonType.TimeIncrementSecondsPlus1)
            {
                GameSettings.TimeIncrementSeconds++;
            }
            else if (button.Type == ButtonType.TimeIncrementSecondsPlus10)
            {
                GameSettings.TimeIncrementSeconds += 10;
            }
            else if (button.Type == ButtonType.TimeIncrementSecondsMinus1)
            {
                if (GameSettings.TimeIncrementSeconds > 0)
                {
                    GameSettings.TimeIncrementSeconds--;
                }
            }
            else if (button.Type == ButtonType.TimeIncrementSecondsMinus10)
            {
                if (GameSettings.TimeIncrementSeconds > 9)
                {
                    GameSettings.TimeIncrementSeconds -= 10;
                }
            }
            else if (button.Type == ButtonType.StartGame)
            {
                NewUiState = AppUIState.InGame;
            }
            else if (button.Type == ButtonType.Back)
            {
                NewUiState = AppUIState.InMenu;
            }
        }

        public void TryLoadSeedFromClipBoard()
        {
            string seed = ClipboardService.GetText();

            if (seed == null)
            {
                TextNotification = new TextNotification
                {
                    Message = "Clipboard is empty",
                    Color = Color.Red
                };
            }
            else if (!new MapGenerator(_textureLibrary, 12).ValidateSeed(seed))
            {
                TextNotification = new TextNotification
                {
                    Message = "Invalid seed",
                    Color = Color.Red
                };
            }
            else
            {
                GameSettings.MapSeed = seed;
                GameSettings.MapSize = int.Parse(seed.Split('x')[0]);

                TextNotification = new TextNotification
                {
                    Message = "Seed loaded",
                    Color = Color.Green
                };
            }
        }

        public void IncreaseMapSize()
        {
            if (GameSettings.MapSize == 20)
            {
                TextNotification = new TextNotification
                {
                    Message = "Maximum map size is 20",
                    Color = Color.Red
                };
            }
            else
            {
                if (GameSettings.MapSeed != null)
                {
                    GameSettings.MapSeed = null;
                }

                GameSettings.MapSize++;
            }
        }

        public void DecreaseMapSize()
        {
            if (GameSettings.MapSize == 8)
            {
                TextNotification = new TextNotification
                {
                    Message = "Minimum map size is 8",
                    Color = Color.Red
                };
            }
            else
            {
                if (GameSettings.MapSeed != null)
                {
                    GameSettings.MapSeed = null;
                }

                GameSettings.MapSize--;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_fontLibrary.DefaultFont, $"----- Game settings -----", new Vector2(20, 20), Color.White);

            string mapSetting = GameSettings.MapSeed != null ? "Seeded" : "Generated";
            spriteBatch.DrawString(_fontLibrary.DefaultFont, $"Map: {mapSetting}", new Vector2(20, 60), Color.White);
            spriteBatch.DrawString(_fontLibrary.DefaultFont, $"Size: {GameSettings.MapSize}", new Vector2(20, 95), Color.White);

            string timeControlSetting = GameSettings.TimeControlEnabled ? "Enabled" : "Disabled";
            spriteBatch.DrawString(_fontLibrary.DefaultFont, $"Time control: {timeControlSetting}", new Vector2(20, 200), Color.White);

            if (GameSettings.TimeControlEnabled)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFont, $"Initial time (minutes): {GameSettings.StartTimeMinutes}", new Vector2(20, 240), Color.White);
                spriteBatch.DrawString(_fontLibrary.DefaultFont, $"Increment (seconds): {GameSettings.TimeIncrementSeconds}", new Vector2(20, 270), Color.White);
            }

            foreach (Button button in Buttons)
            {
                button.Draw(spriteBatch);
            }

            if (TextNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, TextNotification.Message, new Vector2(20, WindowHeight - 60), TextNotification.Color);
            }
        }

        public void HandleLeftMouseClick(Point location)
        {
            ClickButtonByLocation(location);
        }
    }
}
