using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TextCopy;

namespace AgeOfChess
{
    class MultiplayerGame : Game
    {
        public int Id { get; set; }

        private bool _gameplayUiDisabled;
        private int _ourBid;
        private readonly User _us;
        private User _opponent;
        private int _timeSpentBiddingMs;

        private readonly MultiplayerApiClient _apiClient;

        private DateTime _lastPoll;

        public string MapSeed => Map.Seed;

        public PieceColor OurColor => Colors.SingleOrDefault(e => e.IsUs);

        public PieceColor OpponentColor => Colors.SingleOrDefault(e => !e.IsUs);

        public MultiplayerGame(TextureLibrary textureLibrary, FontLibrary fontLibrary, MultiplayerApiClient apiClient, MultiplayerGameSettings settings) : base(textureLibrary, fontLibrary)
        {
            _apiClient = apiClient;

            _us = _apiClient.AuthenticatedUser;

            Colors = new List<PieceColor>()
            {
                new PieceColor(true, "unknown"),
                new PieceColor(false, "unknown")
            };

            MapGenerator mapGenerator = new MapGenerator(textureLibrary, 12);

            // TODO we're incorrectly remembering the map seed from the previous game?

            if (settings.MapSeed != null)
            {
                Map = mapGenerator.GenerateFromSeed(settings.MapSeed);
            }
            else
            {
                // Joining player generates the map
                Map = mapGenerator.GenerateMap(settings.BoardSize.Value, settings.BoardSize.Value);
            }

            HeightPixels = MapSize * 49 > 700 ? MapSize * 49 : 700;
            WidthPixels = MapSize * 49 + 220;
            ControlPanelStartsAtX = MapSize * 49 + 10;

            if (settings.BiddingEnabled)
            {
                State = GameState.Bidding;

                UiParts.AddRange(new List<IUiPart>()
                {
                    new TextBox(textureLibrary, fontLibrary, new Rectangle(ControlPanelStartsAtX + 55, 335, 40, 25), TextBoxType.Bid, "Bid:", "10"),
                    new Button(textureLibrary, fontLibrary, new Rectangle(ControlPanelStartsAtX + 105, 332, 65, 30), ButtonType.SubmitBid, "Submit")
                });
            }
            else
            {
                Black.Gold = 10;
            }

            if (settings.TimeControlEnabled)
            {
                TimeControlEnabled = true;
                LastMoveTimeStamp = DateTime.Now;
                TimeIncrementSeconds = settings.TimeIncrementSeconds;
                Colors.ForEach(e => e.TimeMiliseconds = settings.StartTimeMinutes.Value * 60000);
            }

            _lastPoll = DateTime.Now.AddMinutes(-1);

            AddDefaultButtons();

            UiParts.Single(e => e is Button btn && btn.Type == ButtonType.Back).IsEnabled = false;

            UiParts.Add(new Button(textureLibrary, fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 480, 150, 35), ButtonType.Resign, "Resign", false));
        }

        public void SetOpponent()
        {
            _opponent = _apiClient.GetOpponent(Id);
        }

        public override void ClickUiPartByLocation(Point location)
        {
            var uiPart = this.GetUiPartByLocation(location);

            if (uiPart is Button b && b.Type == ButtonType.Resign)
            {
                Resign();

                return;
            }

            if (State == GameState.Bidding && !_gameplayUiDisabled)
            {
                if (uiPart == null)
                {
                    return;
                }

                if (uiPart is Button btn && btn.Type == ButtonType.SubmitBid)
                {
                    SubmitBid();
                    State = GameState.WaitingForOpponentBid;
                    _gameplayUiDisabled = true;
                }
                else if (uiPart is TextBox tb && tb.Type == TextBoxType.Bid)
                {
                    tb.Focus();
                }
            }
            else if (!_gameplayUiDisabled)
            {
                base.ClickUiPartByLocation(location);
            }
            else
            {
                if (uiPart == null)
                {
                    return;
                }

                if (uiPart is Button btn)
                {
                    if (btn.Type == ButtonType.CopyMapSeed)
                    {
                        ClipboardService.SetText(Map.Seed);

                        TextNotification = new TextNotification
                        {
                            Color = Color.Green,
                            Message = "Copied"
                        };
                    }
                    else if (btn.Type == ButtonType.Back)
                    {
                        NewUiState = AppUIState.InMenu;
                    }
                }
            }
        }

        private void Resign()
        {
            PieceColor winner = OpponentColor;

            Result = winner.IsWhite ? "w+r" : "b+r";

            TextNotification = new TextNotification
            {
                Color = Color.Green,
                Message = winner.IsWhite ? "White wins by resignation" : "Black wins by resignation"
            };

            _apiClient.MakeMove(new MakeMoveDto
            {
                GameId = Id,
                DestinationSquare = "r",
                MoveNumber = MoveList.Count + 1,
                IsWhite = OurColor.IsWhite
            });

            EndGame();
        }

        protected override void EndGame()
        {
            UiParts.Single(e => e is Button btn && btn.Type == ButtonType.Back).IsEnabled = true;

            if (OurColor.IsWhite)
            {
                // White communicates the result to server
                _apiClient.SetResult(Id, Result);
            }

            _gameplayUiDisabled = true;
            
            GameEnded = true;
        }

        private void SubmitBid()
        {
            if (!int.TryParse(this.TextBoxValueByType(TextBoxType.Bid), out int bid))
            {
                TextNotification = new TextNotification
                {
                    Color = Color.Red,
                    Message = "Invalid bid"
                };
            }

            if (TimeControlEnabled)
            {
                _timeSpentBiddingMs = (int)Math.Floor((DateTime.Now - LastMoveTimeStamp.Value).TotalMilliseconds);
            }

            _ourBid = bid;

            _apiClient.MakeBid(Id, bid);

            UiParts.Single(e => e is TextBox tb && tb.Type == TextBoxType.Bid).IsEnabled = false;
            UiParts.Single(e => e is Button btn && btn.Type == ButtonType.SubmitBid).IsEnabled = false;
        }

        public override void Update(SpriteBatch spriteBatch)
        {
            base.Update(spriteBatch);

            if (State != GameState.Bidding && State != GameState.WaitingForOpponentBid && !Colors.Any(e => e.IsUs))
            {
                RandomizeColors();

                if (OurColor.IsWhite)
                {
                    _apiClient.SetWhite(Id);
                    _gameplayUiDisabled = false;
                    UiParts.Single(e => e is Button btn && btn.Type == ButtonType.Resign).IsEnabled = true;
                }
                else
                {
                    _gameplayUiDisabled = true;
                }
            }

            if (!_gameplayUiDisabled || GameEnded)
            {
                return;
            }

            if ((DateTime.Now - _lastPoll).TotalSeconds < 1)
            {
                return;
            }

            _lastPoll = DateTime.Now;

            if (State == GameState.WaitingForOpponentBid)
            {
                int? latestBid = _apiClient.GetOpponent(Id).Bid;

                if (latestBid != null)
                {
                    _opponent.Bid = latestBid;

                    SetColorsAfterBidding(latestBid.Value);

                    if (TimeControlEnabled)
                    {
                        OurColor.TimeMiliseconds -= _timeSpentBiddingMs;
                        PushOurTimeToServer();
                    }

                    _gameplayUiDisabled = !OurColor.IsWhite;

                    _lastPoll = DateTime.Now.AddMinutes(-1);

                    State = GameState.Default;
                }
            }
            else
            {
                PollLatestMoveDto dto = _apiClient.LatestMove(Id);

                if (dto == null)
                {
                    return;
                }

                if (dto.MoveNumber == MoveList.Count + 1)
                {
                    if (dto.DestinationSquare == "r")
                    {
                        // Opponent resigned
                        PieceColor winner = OurColor;

                        Result = winner.IsWhite ? "w+r" : "b+r";

                        TextNotification = new TextNotification
                        {
                            Color = Color.Green,
                            Message = winner.IsWhite ? "White wins by resignation" : "Black wins by resignation"
                        };

                        EndGame();
                    }
                    else if (dto.DestinationSquare == "t")
                    {
                        // Opponent ran out of time
                        PieceColor winner = OurColor;

                        Result = winner.IsWhite ? "w+t" : "b+t";

                        TextNotification = new TextNotification
                        {
                            Color = Color.Green,
                            Message = winner.IsWhite ? "White wins on time" : "Black wins on time"
                        };

                        EndGame();
                    }
                    else
                    {
                        MakeOpponentMove(dto.ToMove());

                        _gameplayUiDisabled = false;

                        if (TimeControlEnabled)
                        {
                            ActiveColor.TimeMiliseconds = ActiveColor.IsWhite ? dto.WhiteTimeMs.Value : dto.BlackTimeMs.Value;
                        }

                        _lastPoll = DateTime.Now.AddMinutes(-1);

                        EndTurn();
                        StartNewTurn();
                    }
                }
            }
        }

        protected override void HandleTimeRanOut(PieceColor color)
        {
            if (GameEnded)
            {
                return;
            }

            if (!color.IsUs)
            {
                // Opponent's client will handle this
                return;
            }

            LoseOnTime();
        }

        private void LoseOnTime()
        {
            PieceColor winner = OpponentColor;

            Result = winner.IsWhite ? "w+t" : "b+t";

            TextNotification = new TextNotification
            {
                Color = Color.Green,
                Message = winner.IsWhite ? "White wins on time" : "Black wins on time"
            };

            _apiClient.MakeMove(new MakeMoveDto
            {
                GameId = Id,
                DestinationSquare = "t",
                MoveNumber = MoveList.Count + 1,
                IsWhite = OurColor.IsWhite
            });

            EndGame();
        }

        private void MakeOpponentMove(Move move)
        {
            Square destinationSquare = Map.GetSquareByCoordinates(move.DestinationSquareX, move.DestinationSquareY);

            if (move.SourceSquareX == null)
            {
                Type pieceType = null;

                switch (move.PiecePlaced)
                {
                    case "p": pieceType = typeof(Pawn); break;
                    case "n": pieceType = typeof(Knight); break;
                    case "b": pieceType = typeof(Bishop); break;
                    case "r": pieceType = typeof(Rook); break;
                    case "q": pieceType = typeof(Queen); break;
                }

                PlacePiece(destinationSquare, pieceType);
            }
            else
            {
                Square sourceSquare = Map.GetSquareByCoordinates(move.SourceSquareX.Value, move.SourceSquareY.Value);

                MovePiece(sourceSquare, destinationSquare);
            }
        }

        private void PushOurTimeToServer()
        {
            _apiClient.SetTime(OurColor.IsWhite, Id, OurColor.TimeMiliseconds);
        }

        private void PushOurMoveToServer()
        {
            Move move = MoveList.Last();

            _apiClient.MakeMove(new MakeMoveDto
            {
                GameId = Id,
                IsWhite = OurColor.IsWhite,
                SourceSquare = move.SourceSquareStr,
                DestinationSquare = move.DestinationSquareStr,
                MoveNumber = MoveList.Count,
                PiecePlaced = move.PiecePlaced,
                ObjectCaptured = move.ObjectCaptured,
                TimeMs = TimeControlEnabled ? OurColor.TimeMiliseconds : (int?)null
            });
        }

        public override void EndTurn()
        {
            base.EndTurn();

            PieceColor previousActiveColor = Colors.Single(e => !e.IsActive);

            if (TimeControlEnabled)
            {
                if (previousActiveColor == OurColor && FirstMoveMade)
                {
                    OurColor.TimeMiliseconds -= (int)Math.Floor((DateTime.Now - LastMoveTimeStamp.Value).TotalMilliseconds);
                    OurColor.TimeMiliseconds += TimeIncrementSeconds.Value * 1000;
                }

                LastMoveTimeStamp = DateTime.Now;
            }

            if (previousActiveColor == OurColor)
            {
                PushOurMoveToServer();
                _gameplayUiDisabled = true;

                UiParts.Single(e => e is Button btn && btn.Type == ButtonType.Resign).IsEnabled = false;
            }
            else
            {
                UiParts.Single(e => e is Button btn && btn.Type == ButtonType.Resign).IsEnabled = true;
            }
        }

        private void RandomizeColors()
        {
            // Highest user id gets white if game id is odd, else lowest user id gets white
            // This randomizes it in a way where both clients reach the same result

            bool weAreWhite = Id % 2 == 1 ? _us.Id > _opponent.Id : _us.Id < _opponent.Id;

            Colors.Single(e => e.IsWhite == weAreWhite).IsUs = true;
            Black.Gold = _ourBid;

            OurColor.PlayedByStr = $"{_us.Username} ({Math.Round(_us.LastElo)})";
            OpponentColor.PlayedByStr = $"{_opponent.Username} ({Math.Round(_opponent.LastElo)})";
        }

        private void SetColorsAfterBidding(int opponentBid)
        {
            if (_ourBid == opponentBid)
            {
                RandomizeColors();
            }
            else
            {
                Colors.Single(e => e.IsWhite == _ourBid > opponentBid).IsUs = true;
                Black.Gold = _ourBid > opponentBid ? _ourBid : opponentBid;

                OurColor.PlayedByStr = $"{_us.Username} ({Math.Round(_us.LastElo)})";
                OpponentColor.PlayedByStr = $"{_opponent.Username} ({Math.Round(_opponent.LastElo)})";
            }

            if (OurColor.IsWhite)
            {
                _apiClient.SetWhite(Id);
                UiParts.Single(e => e is Button btn && btn.Type == ButtonType.Resign).IsEnabled = true;
            }

            TextNotification = new TextNotification
            {
                Color = Color.Green,
                Message = OurColor.IsWhite ? $"They bid {opponentBid}. We get white" : $"They bid {opponentBid}. We get black"
            };
        }
    }
}
