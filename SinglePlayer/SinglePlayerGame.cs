using Microsoft.Xna.Framework;

namespace AgeOfChess
{
    class SinglePlayerGame : Game
    {
        public SinglePlayerGame(SinglePlayerGameSettings settings, TextureLibrary textureLibrary, FontLibrary fontLibrary) : base(textureLibrary, fontLibrary)
        {
            MapGenerator mapGenerator = new MapGenerator(textureLibrary, 12);

            if (settings.MapSeed != null)
            {
                Map = mapGenerator.GenerateFromSeed(settings.MapSeed);
            }
            else
            {
                Map = mapGenerator.GenerateMap(settings.MapSize.Value, settings.MapSize.Value);
            }

            WindowHeight = MapSize * 49 > 600 ? MapSize * 49 : 600;
            WindowWidth = MapSize * 49 + 200;
            ControlPanelStartsAtX = MapSize * 49 + 10;

            if (settings.TimeControlEnabled)
            {
                TimeControlEnabled = true;
                TimeIncrementSeconds = settings.TimeIncrementSeconds;
                Colors.ForEach(e => e.TimeMiliseconds = settings.StartTimeMinutes.Value * 6000);
            }

            AddDefaultButtons();

            Buttons.Add(new Button(textureLibrary, fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, WindowHeight - 140, 150, 35), ButtonType.BlackGoldIncrease, "Increase B gold"));
            Buttons.Add(new Button(textureLibrary, fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, WindowHeight - 100, 150, 35), ButtonType.BlackGoldDecrease, "Decrease B gold"));
        }
    }
}
