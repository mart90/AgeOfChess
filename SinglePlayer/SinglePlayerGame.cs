using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
                Map = mapGenerator.GenerateMap(settings.BoardSize.Value, settings.BoardSize.Value);
            }

            HeightPixels = MapSize * 49 > 700 ? MapSize * 49 : 700;
            WidthPixels = MapSize * 49 + 220;
            ControlPanelStartsAtX = MapSize * 49 + 10;

            if (settings.TimeControlEnabled)
            {
                TimeControlEnabled = true;
                LastMoveTimeStamp = DateTime.Now;
                TimeIncrementSeconds = settings.TimeIncrementSeconds;
                Colors.ForEach(e => e.TimeMiliseconds = settings.StartTimeMinutes.Value * 60000);
            }

            AddDefaultButtons();

            UiParts.Add(new Button(textureLibrary, fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 480, 150, 35), ButtonType.BlackGoldIncrease, "Increase B gold"));
            UiParts.Add(new Button(textureLibrary, fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 520, 150, 35), ButtonType.BlackGoldDecrease, "Decrease B gold"));
        }

        public override void EndTurn()
        {
            base.EndTurn();

            if (TimeControlEnabled)
            {
                PieceColor previousActiveColor = Colors.Single(e => !e.IsActive);

                if (!previousActiveColor.IsWhite || FirstMoveMade) 
                {
                    previousActiveColor.TimeMiliseconds -= (int)Math.Floor((DateTime.Now - LastMoveTimeStamp.Value).TotalMilliseconds);
                    
                    if (TimeIncrementSeconds != null)
                    {
                        previousActiveColor.TimeMiliseconds += TimeIncrementSeconds.Value * 1000;
                    }
                }

                LastMoveTimeStamp = DateTime.Now;
            }
        }
    }
}
