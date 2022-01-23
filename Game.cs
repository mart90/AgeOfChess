using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TextCopy;

namespace AgeOfChess
{
    abstract class Game : IUiWindow
    {
        public AppUIState CorrespondingUiState { get; }
        public List<PieceColor> Colors { get; set; }
        public GameState State { get; set; }
        public List<IUiPart> UiParts { get; }
        public TextNotification TextNotification { get; set; }
        public AppUIState? NewUiState { get; set; }
        public int HeightPixels { get; protected set; }
        public int WidthPixels { get; protected set; }
        public string Result { get; protected set; }
        public DateTime? LastMoveTimeStamp { get; protected set; }
        public List<Move> MoveList { get; }

        protected Map Map;
        protected readonly TextureLibrary TextureLibrary;
        protected readonly FontLibrary FontLibrary;

        protected bool TimeControlEnabled;
        protected int? TimeIncrementSeconds;
        protected int ControlPanelStartsAtX;
        protected bool FirstMoveMade;
        protected bool GameEnded;

        public Game(TextureLibrary textureLibrary, FontLibrary fontLibrary)
        {
            CorrespondingUiState = AppUIState.InGame;

            State = GameState.Default;

            TextureLibrary = textureLibrary;
            FontLibrary = fontLibrary;

            UiParts = new List<IUiPart>();

            MoveList = new List<Move>();
        }

        public int MapSize => Map.Width;

        public PieceColor ActiveColor => Colors.Single(e => e.IsActive);

        public PieceColor InActiveColor => Colors.Single(e => !e.IsActive);

        public PieceColor GetPieceColor(Piece piece) => Colors.Single(e => e.IsWhite == piece.IsWhite);

        public Square GetSquareByLocation(Point location) => Map.Squares.SingleOrDefault(e => e.LocationIncludesPoint(location));

        public Button SelectedButton => (Button)UiParts.SingleOrDefault(e => e is Button button && button.IsSelected);

        public PieceColor White => Colors.Single(e => e.IsWhite);

        public PieceColor Black => Colors.Single(e => !e.IsWhite);

        protected virtual void EndGame()
        {
            GameEnded = true;
        }

        public virtual void EndTurn()
        {
            PieceColor previousActiveColor = ActiveColor;

            previousActiveColor.IsActive = false;
            Colors.Single(e => e != previousActiveColor).IsActive = true;

            foreach (Square square in Map.GetMines())
            {
                if (square.Object is Piece piece)
                {
                    GetPieceColor(piece).Gold += 3;
                }
            }
        }

        public void StartNewTurn()
        {
            ClearSelection();

            if (!FirstMoveMade)
            {
                FirstMoveMade = true;
            }

            PieceColor winningColor = null;

            if (CheckForMate())
            {
                winningColor = InActiveColor;

                TextNotification = new TextNotification
                {
                    Color = Color.Green,
                    Message = winningColor.IsWhite ? "White wins by mate" : "Black wins by mate"
                };
            }

            if (winningColor == null)
            {
                winningColor = CheckForGoldVictory();

                if (winningColor != null)
                {
                    TextNotification = new TextNotification
                    {
                        Color = Color.Green,
                        Message = winningColor.IsWhite ? "White wins by gold count" : "Black wins by gold count"
                    };
                }
            }

            if (winningColor != null)
            {
                EndGame();
            }
        }

        private PieceColor CheckForGoldVictory()
        {
            IEnumerable<PieceColor> colorsWithWinningGold = Colors.Where(e => e.Gold > 150);

            if (colorsWithWinningGold.Any())
            {
                PieceColor winningColor = null;

                if (colorsWithWinningGold.Count() == 1)
                {
                    winningColor = colorsWithWinningGold.First();
                }
                else if (colorsWithWinningGold.Select(e => e.Gold).Distinct().Count() == 2) // Else players are on equal gold
                {
                    winningColor = colorsWithWinningGold.Single(e => e.Gold == colorsWithWinningGold.Max(c => c.Gold));
                }

                if (winningColor != null)
                {
                    Result = winningColor.IsWhite ? "w+g" : "b+g";
                }

                return winningColor;
            }

            return null;
        }

        private bool CheckForMate()
        {
            var pathFinder = new PathFinder(Map);

            Square activeKingSquare = Map.Squares.Single(e => e.Object is King king && king.IsWhite == ActiveColor.IsWhite);

            var activeKingLegalMoves = pathFinder.FindLegalDestinationSquares((King)activeKingSquare.Object, activeKingSquare);

            var legalCheckSquares = pathFinder.FindAttacksForColor(!ActiveColor.IsWhite);

            if (legalCheckSquares.Contains(activeKingSquare))
            {
                if (!activeKingLegalMoves.Any())
                {
                    // Checkmate
                    activeKingSquare.SetObject(new WhiteFlag(TextureLibrary));
                    Result = ActiveColor.IsWhite ? "b+c" : "w+c";
                    return true;
                }
                else
                {
                    // Just check
                    activeKingSquare.SetTemporaryColor(SquareColor.Red);
                    return false;
                }
            }

            if (!activeKingLegalMoves.Any() && Map.Squares.Where(e => e.Object is Piece piece && piece.IsWhite == ActiveColor.IsWhite).Count() == 1)
            {
                if (Colors.Single(e => !e.IsActive).Gold < 15)
                {
                    // Stalemate
                    activeKingSquare.SetObject(new WhiteFlag(TextureLibrary));
                    Result = ActiveColor.IsWhite ? "b+s" : "w+s";
                    return true;
                }
            }

            return false;
        }

        public virtual void Update(SpriteBatch spriteBatch)
        {
            Map.Draw(spriteBatch);

            spriteBatch.DrawString(FontLibrary.DefaultFont, $"White gold: {White.Gold}", new Vector2(ControlPanelStartsAtX + 20, 10), Color.Black);
            spriteBatch.DrawString(FontLibrary.DefaultFont, $"Black gold: {Black.Gold}", new Vector2(ControlPanelStartsAtX + 20, 30), Color.Black);

            foreach (IUiPart uiPart in UiParts)
            {
                if (uiPart is PlacePieceButton placePieceButton)
                {
                    placePieceButton.Draw(spriteBatch, ActiveColor.IsWhite);
                }
                else
                {
                    uiPart.Draw(spriteBatch);
                }
            }

            if (TimeControlEnabled)
            {
                bool whiteClockRunning = (ActiveColor.IsWhite && FirstMoveMade) || State == GameState.Bidding;
                bool blackClockRunning = !ActiveColor.IsWhite;

                TimeSpan whiteTime = TimeSpan.FromMilliseconds(White.TimeMiliseconds - (whiteClockRunning ? (DateTime.Now - LastMoveTimeStamp.Value).TotalMilliseconds : 0));
                TimeSpan blackTime = TimeSpan.FromMilliseconds(Black.TimeMiliseconds - (blackClockRunning ? (DateTime.Now - LastMoveTimeStamp.Value).TotalMilliseconds : 0));

                if (whiteTime.TotalSeconds < 0)
                {
                    HandleTimeRanOut(White);
                    whiteTime = new TimeSpan(0);
                }
                else if (blackTime.TotalSeconds < 0)
                {
                    HandleTimeRanOut(Black);
                    blackTime = new TimeSpan(0);
                }

                string whiteTimeStr = whiteTime.ToString(@"hh\:mm\:ss");
                string blackTimeStr = blackTime.ToString(@"hh\:mm\:ss");

                spriteBatch.DrawString(ActiveColor.IsWhite ? FontLibrary.DefaultFontBold : FontLibrary.DefaultFont, whiteTimeStr, new Vector2(ControlPanelStartsAtX + 20, 310), Color.Black);
                spriteBatch.DrawString(!ActiveColor.IsWhite ? FontLibrary.DefaultFontBold : FontLibrary.DefaultFont, blackTimeStr, new Vector2(ControlPanelStartsAtX + 20, 410), Color.Black);
            }

            spriteBatch.DrawString(ActiveColor.IsWhite ? FontLibrary.DefaultFontBold : FontLibrary.DefaultFont, "White", new Vector2(ControlPanelStartsAtX + 20, 270), Color.Black);
            spriteBatch.DrawString(!ActiveColor.IsWhite ? FontLibrary.DefaultFontBold : FontLibrary.DefaultFont, "Black", new Vector2(ControlPanelStartsAtX + 20, 370), Color.Black);

            spriteBatch.DrawString(ActiveColor.IsWhite ? FontLibrary.DefaultFontBold : FontLibrary.DefaultFont, White.PlayedByStr, new Vector2(ControlPanelStartsAtX + 20, 290), Color.Black);
            spriteBatch.DrawString(!ActiveColor.IsWhite ? FontLibrary.DefaultFontBold : FontLibrary.DefaultFont, Black.PlayedByStr, new Vector2(ControlPanelStartsAtX + 20, 390), Color.Black);

            if (TextNotification != null)
            {
                spriteBatch.DrawString(FontLibrary.DefaultFontBold, TextNotification.Message, new Vector2(ControlPanelStartsAtX, HeightPixels - 25), TextNotification.Color);
            }
        }

        protected virtual void HandleTimeRanOut(PieceColor color) { }

        private bool AttemptMovePiece(Point location)
        {
            var destinationSquare = GetSquareByLocation(location);
            var sourceSquare = Map.SelectedSquare;

            if (sourceSquare == null || destinationSquare == null)
            {
                return false;
            }

            if (sourceSquare.Object is Piece piece)
            {
                var legalDestinationSquares = Map.FindLegalDestinationsForPiece(piece, sourceSquare);

                if (!legalDestinationSquares.Contains(destinationSquare))
                {
                    return false;
                }

                MovePiece(sourceSquare, destinationSquare);
                return true;
            }

            return false;
        }

        protected void MovePiece(Square sourceSquare, Square destinationSquare)
        {
            PlaceableObject objectToCapture = destinationSquare.Object;

            MoveList.Add(new Move(sourceSquare, destinationSquare, null, objectToCapture != null ? Move.ObjectToStringId(objectToCapture) : null));

            var piece = (Piece)sourceSquare.Object;

            sourceSquare.ClearObject();

            if (destinationSquare.Object is Treasure)
            {
                GetPieceColor(piece).Gold += 20;
            }

            destinationSquare.SetObject(piece);
        }

        private bool AttemptPlacePiece(Point location, Type pieceType)
        {
            ClearSelection();

            var destinationSquare = GetSquareByLocation(location);

            var pathFinder = new PathFinder(Map);

            if (destinationSquare != null && pathFinder.FindLegalDestinationsForPiecePlacement(ActiveColor.IsWhite, pieceType == typeof(Pawn)).Contains(destinationSquare))
            {
                PlacePiece(destinationSquare, pieceType);
                return true;
            }

            return false;
        }

        protected void PlacePiece(Square destinationSquare, Type pieceType)
        {
            MoveList.Add(new Move(null, destinationSquare, Move.ObjectTypeToStringId(pieceType)));

            Piece newPiece = null;

            if (pieceType == typeof(Queen))
            {
                if (ActiveColor.IsWhite) newPiece = new WhiteQueen(TextureLibrary); else newPiece = new BlackQueen(TextureLibrary);
            }
            if (pieceType == typeof(Rook))
            {
                if (ActiveColor.IsWhite) newPiece = new WhiteRook(TextureLibrary); else newPiece = new BlackRook(TextureLibrary);
            }
            if (pieceType == typeof(Bishop))
            {
                if (ActiveColor.IsWhite) newPiece = new WhiteBishop(TextureLibrary); else newPiece = new BlackBishop(TextureLibrary);
            }
            if (pieceType == typeof(Knight))
            {
                if (ActiveColor.IsWhite) newPiece = new WhiteKnight(TextureLibrary); else newPiece = new BlackKnight(TextureLibrary);
            }
            if (pieceType == typeof(Pawn))
            {
                if (ActiveColor.IsWhite) newPiece = new WhitePawn(TextureLibrary); else newPiece = new BlackPawn(TextureLibrary);
            }

            destinationSquare.SetObject(newPiece);

            var placePieceButton = (PlacePieceButton)UiParts.Single(e => e is PlacePieceButton ppb && ppb.PieceType == pieceType);
            ActiveColor.Gold -= placePieceButton.PieceCost;
        }

        private void SelectSquareByLocation(Point location)
        {
            var square = GetSquareByLocation(location);

            if (square == null)
            {
                return;
            }

            ClearSelection();

            if (square.Object is Piece piece && ActiveColor == GetPieceColor(piece))
            {
                State = GameState.PieceSelected;
                square.IsSelected = true;

                IEnumerable<Square> destinationSquares = Map.FindLegalDestinationsForPiece(piece, square);

                foreach (Square destinationSquare in destinationSquares)
                {
                    destinationSquare.SetTemporaryColor(SquareColor.Purple);
                }
            }
        }

        public virtual void ClickUiPartByLocation(Point location)
        {
            if (State == GameState.PieceSelected)
            {
                if (AttemptMovePiece(location))
                {
                    EndTurn();
                    StartNewTurn();
                }
                else
                {
                    ClearSelection();
                }
            }
            else if (State == GameState.PlacingPiece)
            {
                var placePieceButton = (PlacePieceButton)SelectedButton;

                if (AttemptPlacePiece(location, placePieceButton.PieceType))
                {
                    EndTurn();
                    StartNewTurn();
                }
                else
                {
                    ClearSelection();
                }
            }
            else if (location.X > MapSize * 49)
            {
                ClickControlPanelUiPartByLocation(location);
            }
            else
            {
                SelectSquareByLocation(location);
            }
        }

        private void ClickControlPanelUiPartByLocation(Point location)
        {
            var uiPart = this.GetUiPartByLocation(location);

            if (uiPart == null)
            {
                return;
            }

            ClearSelection();

            Button button = uiPart is Button b ? b : null;

            if (button == null)
            {
                return;
            }

            if (button is PlacePieceButton placePieceButton)
            {
                if (ActiveColor.Gold >= placePieceButton.PieceCost)
                {
                    State = GameState.PlacingPiece;
                    placePieceButton.IsSelected = true;

                    TextNotification = new TextNotification
                    {
                        Message = $"Placing {placePieceButton.PieceType.Name}",
                        Color = Color.Green
                    };

                    var pathFinder = new PathFinder(Map);

                    foreach (Square square in pathFinder.FindLegalDestinationsForPiecePlacement(ActiveColor.IsWhite, placePieceButton.PieceType == typeof(Pawn)))
                    {
                        square.SetTemporaryColor(SquareColor.Purple);
                    }
                }
                else
                {
                    TextNotification = new TextNotification
                    {
                        Message = "Not enough gold",
                        Color = Color.Red
                    };
                }
            }
            else if (button.Type == ButtonType.BlackGoldIncrease)
            {
                Colors.Single(e => !e.IsWhite).Gold += 1;
            }
            else if (button.Type == ButtonType.BlackGoldDecrease)
            {
                Colors.Single(e => !e.IsWhite).Gold -= 1;
            }
            else if (button.Type == ButtonType.CopyMapSeed)
            {
                ClipboardService.SetText(Map.Seed);
                
                TextNotification = new TextNotification
                {
                    Color = Color.Green,
                    Message = "Copied"
                };
            }
            else if (button.Type == ButtonType.Back)
            {
                NewUiState = AppUIState.InMenu;
            }
        }

        protected void ClearSelection()
        {
            var selectedSquare = Map.SelectedSquare;
            if (selectedSquare != null)
            {
                selectedSquare.IsSelected = false;
            }

            if (SelectedButton != null)
            {
                SelectedButton.IsSelected = false;
            }

            TextNotification = null;

            Map.Squares.ForEach(e => e.ClearTemporaryColor());

            foreach (IUiPart uiPart in UiParts.Where(e => e is TextBox))
            {
                ((TextBox)uiPart).HasFocus = false;
            }

            State = GameState.Default;
        }

        protected void AddDefaultButtons()
        {
            UiParts.Add(new PlacePieceButton(TextureLibrary, FontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 60, 150, 35), typeof(Queen)));
            UiParts.Add(new PlacePieceButton(TextureLibrary, FontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 100, 150, 35), typeof(Rook)));
            UiParts.Add(new PlacePieceButton(TextureLibrary, FontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 140, 150, 35), typeof(Bishop)));
            UiParts.Add(new PlacePieceButton(TextureLibrary, FontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 180, 150, 35), typeof(Knight)));
            UiParts.Add(new PlacePieceButton(TextureLibrary, FontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 220, 150, 35), typeof(Pawn)));

            UiParts.Add(new Button(TextureLibrary, FontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 440, 150, 35), ButtonType.CopyMapSeed, "Copy map seed"));
            UiParts.Add(new Button(TextureLibrary, FontLibrary, new Rectangle(ControlPanelStartsAtX + 15, HeightPixels - 80, 150, 35), ButtonType.Back, "Return to menu"));
        }
    }
}
