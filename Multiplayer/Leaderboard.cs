using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgeOfChess
{
    class Leaderboard : IUiWindow
    {
        public int HeightPixels { get; }
        public int WidthPixels { get; }
        public AppUIState CorrespondingUiState { get; }
        public List<IUiPart> UiParts { get; }
        public AppUIState? NewUiState { get; set; }
        public TextNotification TextNotification { get; private set; }
        public bool IsRefreshed { get; set; }

        private List<LeaderboardEntryDto> _orderedRanking;
        private string _ownRankString;

        private readonly FontLibrary _fontLibrary;
        private readonly MultiplayerApiClient _apiClient;

        public Leaderboard(TextureLibrary textureLibrary, FontLibrary fontLibrary, MultiplayerApiClient apiClient)
        {
            CorrespondingUiState = AppUIState.ViewingLeaderboard;

            HeightPixels = 600;
            WidthPixels = 600;

            UiParts = new List<IUiPart>
            {
                new Button(textureLibrary, fontLibrary, new Rectangle(WidthPixels - 160, HeightPixels - 70, 120, 35), ButtonType.Back, "Back")
            };

            _apiClient = apiClient;
            _fontLibrary = fontLibrary;
            _ownRankString = null;
        }

        public void Update(SpriteBatch spriteBatch)
        {
            if (!IsRefreshed)
            {
                LeaderboardDto dto = _apiClient.GetLeaderboard();

                _orderedRanking = dto.Entries.OrderByDescending(e => e.Rating).ToList();

                if (dto.OurRank != null)
                {
                    _ownRankString = $"{dto.OurRank}/{dto.TotalUsers}";
                }
            }

            spriteBatch.DrawString(_fontLibrary.DefaultFontBold, "--------------------------- Leaderboard ---------------------------", new Vector2(20, 20), Color.Black);

            foreach (IUiPart uiPart in UiParts)
            {
                uiPart.Draw(spriteBatch);
            }


            for (int i = 0; i < _orderedRanking.Count(); i++)
            {
                int x = i > 14 ? 300 : 20;
                int y = 60 + (i > 14 ? i - 15 : i) * 30;

                spriteBatch.DrawString(
                    _fontLibrary.DefaultFont,
                    $"{i + 1}. {_orderedRanking[i].Username} ({Math.Round(_orderedRanking[i].Rating)})",
                    new Vector2(x, y),
                    Color.Black);
            }

            if (_ownRankString != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFont, $"My rank: {_ownRankString}", new Vector2(20, 560), Color.Black);
            }

            IsRefreshed = true;
        }

        public void ClickUiPartByLocation(Point location)
        {
            var uiPart = this.GetUiPartByLocation(location);

            if (uiPart == null)
            {
                return;
            }

            if (uiPart is Button btn && btn.Type == ButtonType.Back)
            {
                NewUiState = AppUIState.InMenu;
            }
        }
    }
}
