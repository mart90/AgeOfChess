using Microsoft.Xna.Framework;

namespace AgeOfChess
{
    class SinglePlayerGameSettingsForm : GameSettingsForm
    {
        public SinglePlayerGameSettingsForm(TextureLibrary textureLibrary, FontLibrary fontLibrary) : base(textureLibrary, fontLibrary)
        {
            CorrespondingUiState = AppUIState.CreatingSinglePlayerGame;

            GameSettings = new SinglePlayerGameSettings();

            UiParts.Add(new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 290, HeightPixels - 70, 120, 35), ButtonType.Back, "Back"));
            UiParts.Add(new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 160, HeightPixels - 70, 120, 35), ButtonType.StartGame, "Start game"));
        }

        public override void ClickUiPartByLocation(Point location)
        {
            base.ClickUiPartByLocation(location);

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

            if (button.Type == ButtonType.StartGame)
            {
                NewUiState = AppUIState.InGame;
            }
            else if (button.Type == ButtonType.Back)
            {
                NewUiState = AppUIState.InMenu;
            }
        }
    }
}
