using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AgeOfChess
{
    class SinglePlayerGame : Game
    {
        public SinglePlayerGame(SinglePlayerGameSettings settings, TextureLibrary textureLibrary, FontLibrary fontLibrary) : base(textureLibrary, fontLibrary)
        {
            Colors = new List<PieceColor>()
            {
                new PieceColor(true, "you"),
                new PieceColor(false, "you")
            };

            MapGenerator mapGenerator = new MapGenerator(textureLibrary, 12);

            if (settings.MapSeed != null)
            {
                Map = mapGenerator.GenerateFromSeed(settings.MapSeed);
            }
            else
            {
                Map = mapGenerator.GenerateMap(settings.MapSize.Value, settings.MapSize.Value);
            }

            HeightPixels = MapSize * 49 > 600 ? MapSize * 49 : 600;
            WidthPixels = MapSize * 49 + 200;
            ControlPanelStartsAtX = MapSize * 49 + 10;

            if (settings.TimeControlEnabled)
            {
                TimeControlEnabled = true;
                LastMoveTimeStamp = DateTime.Now;
                TimeIncrementSeconds = settings.TimeIncrementSeconds;
                Colors.ForEach(e => e.TimeMiliseconds = settings.StartTimeMinutes.Value * 60000);
            }

            AddDefaultButtons();

            Buttons.Add(new Button(textureLibrary, fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, HeightPixels - 140, 150, 35), ButtonType.BlackGoldIncrease, "Increase B gold"));
            Buttons.Add(new Button(textureLibrary, fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, HeightPixels - 100, 150, 35), ButtonType.BlackGoldDecrease, "Decrease B gold"));
        }
    }
}
