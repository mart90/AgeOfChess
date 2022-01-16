namespace AgeOfChess
{
    class SinglePlayerGameSettingsForm : GameSettingsForm
    {
        public SinglePlayerGameSettingsForm(TextureLibrary textureLibrary, FontLibrary fontLibrary) : base(textureLibrary, fontLibrary)
        {
            CorrespondingUiState = AppUIState.CreatingSinglePlayerGame;

            GameSettings = new SinglePlayerGameSettings();
        }
    }
}
