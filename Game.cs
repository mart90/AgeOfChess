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
        public GameUIState UserInterfaceState { get; set; }
        public List<Button> Buttons { get; }
        public TextNotification TextNotification { get; set; }
        public AppUIState? NewUiState { get; set; }
        public int HeightPixels { get; protected set; }
        public int WidthPixels { get; protected set; }
        public string Result { get; private set; }
        public DateTime? LastMoveTimeStamp { get; protected set; }

        protected Map Map;
        private readonly TextureLibrary _textureLibrary;
        private readonly FontLibrary _fontLibrary;

        protected bool TimeControlEnabled;
        protected int? TimeIncrementSeconds;
        protected int ControlPanelStartsAtX;

        public Game(TextureLibrary textureLibrary, FontLibrary fontLibrary)
        {
            CorrespondingUiState = AppUIState.InGame;

            UserInterfaceState = GameUIState.Default;

            _textureLibrary = textureLibrary;
            _fontLibrary = fontLibrary;

            Buttons = new List<Button>();
        }

        public int MapSize => Map.Width;

        public PieceColor ActiveColor => Colors.Single(e => e.IsActive);

        public PieceColor GetPieceColor(Piece piece) => Colors.Single(e => e.IsWhite == piece.IsWhite);

        public Square GetSquareByLocation(Point location) => Map.Squares.SingleOrDefault(e => e.LocationIncludesPoint(location));

        public Button SelectedButton => Buttons.SingleOrDefault(e => e.IsSelected);

        public void HandleLeftMouseClick(Point location)
        {
            if (UserInterfaceState == GameUIState.PieceSelected)
            {
                if (AttemptMovePiece(location))
                {
                    EndTurn();
                }
                else
                {
                    ClearSelection();
                }
            }
            else if (UserInterfaceState == GameUIState.PlacingPiece)
            {
                var placePieceButton = (PlacePieceButton)SelectedButton;

                if (AttemptPlacePiece(location, placePieceButton.PieceType))
                {
                    ActiveColor.Gold -= placePieceButton.PieceCost;
                    EndTurn();
                }
                else
                {
                    ClearSelection();
                }
            }
            else if (location.X > MapSize * 49)
            {
                ClickButtonByLocation(location);
            }
            else
            {
                SelectSquareByLocation(location);
            }
        }

        public void EndTurn()
        {
            PieceColor previousActiveColor = ActiveColor;

            previousActiveColor.IsActive = false;
            Colors.Single(e => e != previousActiveColor).IsActive = true;

            if (TimeControlEnabled) 
            {
                previousActiveColor.TimeMiliseconds -= (int)Math.Floor((DateTime.Now - LastMoveTimeStamp.Value).TotalMilliseconds);
                LastMoveTimeStamp = DateTime.Now;

                if (TimeIncrementSeconds != null)
                {
                    previousActiveColor.TimeMiliseconds += TimeIncrementSeconds.Value * 1000;
                }
            }

            foreach (Square square in Map.GetMines())
            {
                if (square.Object is Piece piece)
                {
                    GetPieceColor(piece).Gold += 3;
                }
            }

            StartNewTurn();
        }

        public void StartNewTurn()
        {
            ClearSelection();

            var pathFinder = new PathFinder(Map);

            Square activeKingSquare = Map.Squares.Single(e => e.Object is King king && king.IsWhite == ActiveColor.IsWhite);
            var activeKingLegalMoves = pathFinder.FindLegalDestinationSquares((King)activeKingSquare.Object, activeKingSquare);
            var legalCheckSquares = pathFinder.FindChecksForColor(!ActiveColor.IsWhite);

            if (legalCheckSquares.Contains(activeKingSquare))
            {
                if (!activeKingLegalMoves.Any())
                {
                    // Checkmate
                    activeKingSquare.SetObject(new WhiteFlag(_textureLibrary));
                }
                else
                {
                    activeKingSquare.SetTemporaryColor(SquareColor.Red);
                }
            }

            if (!activeKingLegalMoves.Any() && Map.Squares.Where(e => e.Object is Piece piece && piece.IsWhite == ActiveColor.IsWhite).Count() == 1)
            {
                if (Colors.Single(e => !e.IsActive).Gold < 25)
                {
                    // Stalemate
                    activeKingSquare.SetObject(new WhiteFlag(_textureLibrary));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Map.Draw(spriteBatch);

            spriteBatch.DrawString(_fontLibrary.DefaultFont, $"White gold: {Colors.Single(e => e.IsWhite).Gold}", new Vector2(ControlPanelStartsAtX + 20, 10), Color.Black);
            spriteBatch.DrawString(_fontLibrary.DefaultFont, $"Black gold: {Colors.Single(e => !e.IsWhite).Gold}", new Vector2(ControlPanelStartsAtX + 20, 30), Color.Black);

            foreach (Button button in Buttons)
            {
                if (button is PlacePieceButton placePieceButton)
                {
                    placePieceButton.Draw(spriteBatch, ActiveColor.IsWhite);
                }
                else
                {
                    button.Draw(spriteBatch);
                }
            }

            string whiteStr = $"White ({Colors.Single(e => e.IsWhite).PlayedBy})";
            string blackStr = $"Black ({Colors.Single(e => !e.IsWhite).PlayedBy})";

            spriteBatch.DrawString(ActiveColor.IsWhite ? _fontLibrary.DefaultFontBold : _fontLibrary.DefaultFont, whiteStr, new Vector2(ControlPanelStartsAtX + 20, 290), Color.Black);
            spriteBatch.DrawString(!ActiveColor.IsWhite ? _fontLibrary.DefaultFontBold : _fontLibrary.DefaultFont, blackStr, new Vector2(ControlPanelStartsAtX + 20, 350), Color.Black);

            if (TimeControlEnabled)
            {
                TimeSpan whiteTime = TimeSpan.FromMilliseconds(Colors.Single(e => e.IsWhite).TimeMiliseconds - (ActiveColor.IsWhite ? (DateTime.Now - LastMoveTimeStamp.Value).TotalMilliseconds : 0));
                TimeSpan blackTime = TimeSpan.FromMilliseconds(Colors.Single(e => !e.IsWhite).TimeMiliseconds - (!ActiveColor.IsWhite ? (DateTime.Now - LastMoveTimeStamp.Value).TotalMilliseconds : 0));

                string whiteTimeStr = whiteTime.ToString(@"hh\:mm\:ss");
                string blackTimeStr = blackTime.ToString(@"hh\:mm\:ss");

                spriteBatch.DrawString(ActiveColor.IsWhite ? _fontLibrary.DefaultFontBold : _fontLibrary.DefaultFont, whiteTimeStr, new Vector2(ControlPanelStartsAtX + 20, 310), Color.Black);
                spriteBatch.DrawString(!ActiveColor.IsWhite ? _fontLibrary.DefaultFontBold : _fontLibrary.DefaultFont, blackTimeStr, new Vector2(ControlPanelStartsAtX + 20, 370), Color.Black);
            }

            if (TextNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, TextNotification.Message, new Vector2(ControlPanelStartsAtX, HeightPixels - 25), TextNotification.Color);
            }
        }

        public bool AttemptMovePiece(Point location)
        {
            // TODO check for illegal moves (king check)

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

                sourceSquare.ClearObject();

                if (destinationSquare.Object is Treasure)
                {
                    GetPieceColor(piece).Gold += 20;
                }

                destinationSquare.SetObject(piece);

                return true;
            }

            return false;
        }

        public bool AttemptPlacePiece(Point location, Type pieceType)
        {
            ClearSelection();

            var destinationSquare = GetSquareByLocation(location);

            if (destinationSquare != null && FindLegalDestinationsForPiecePlacement(pieceType == typeof(Pawn)).Contains(destinationSquare))
            {
                Piece newPiece = null;

                if (pieceType == typeof(Queen))
                {
                    if (ActiveColor.IsWhite) newPiece = new WhiteQueen(_textureLibrary); else newPiece = new BlackQueen(_textureLibrary);
                }
                if (pieceType == typeof(Rook))
                {
                    if (ActiveColor.IsWhite) newPiece = new WhiteRook(_textureLibrary); else newPiece = new BlackRook(_textureLibrary);
                }
                if (pieceType == typeof(Bishop))
                {
                    if (ActiveColor.IsWhite) newPiece = new WhiteBishop(_textureLibrary); else newPiece = new BlackBishop(_textureLibrary);
                }
                if (pieceType == typeof(Knight))
                {
                    if (ActiveColor.IsWhite) newPiece = new WhiteKnight(_textureLibrary); else newPiece = new BlackKnight(_textureLibrary);
                }
                if (pieceType == typeof(Pawn))
                {
                    if (ActiveColor.IsWhite) newPiece = new WhitePawn(_textureLibrary); else newPiece = new BlackPawn(_textureLibrary);
                }

                destinationSquare.SetObject(newPiece);

                return true;
            }

            return false;
        }

        public IEnumerable<Square> FindLegalDestinationsForPiecePlacement(bool placingPawn = false)
        {
            var legalDestinations = new List<Square>();
            var pathFinder = new PathFinder(Map);

            Square activePlayerKingSquare = Map.Squares.Single(e => e.Object != null 
                && e.Object is Piece piece 
                && piece.IsWhite == ActiveColor.IsWhite 
                && piece is King);

            legalDestinations.AddRange(pathFinder.FindLegalPiecePlacementsAroundSquare(activePlayerKingSquare).ToList());

            if (placingPawn)
            {
                List<Square> activePlayerOtherPieceSquares = Map.Squares
                    .Where(e => e.Object != null
                        && e.Object is Piece piece
                        && piece.IsWhite == ActiveColor.IsWhite
                        && !(piece is Pawn)
                        && !(piece is King))
                    .ToList();

                foreach (Square square in activePlayerOtherPieceSquares)
                {
                    legalDestinations.AddRange(pathFinder.FindLegalPiecePlacementsAroundSquare(square));
                }
            }

            return legalDestinations;
        }

        public void SelectSquareByLocation(Point location)
        {
            var square = GetSquareByLocation(location);

            if (square == null)
            {
                return;
            }

            ClearSelection();

            if (square.Object is Piece piece && ActiveColor == GetPieceColor(piece))
            {
                UserInterfaceState = GameUIState.PieceSelected;
                square.IsSelected = true;

                IEnumerable<Square> destinationSquares = Map.FindLegalDestinationsForPiece(piece, square);

                foreach (Square destinationSquare in destinationSquares)
                {
                    destinationSquare.SetTemporaryColor(SquareColor.Purple);
                }
            }
        }

        public void ClickButtonByLocation(Point location)
        {
            var button = this.GetButtonByLocation(location);

            if (button == null)
            {
                return;
            }

            ClearSelection();

            if (button is PlacePieceButton placePieceButton)
            {
                if (ActiveColor.Gold >= placePieceButton.PieceCost)
                {
                    UserInterfaceState = GameUIState.PlacingPiece;
                    placePieceButton.IsSelected = true;

                    TextNotification = new TextNotification 
                    { 
                        Message = $"Placing {placePieceButton.PieceType.Name}",
                        Color = Color.Green
                    };

                    foreach (Square square in FindLegalDestinationsForPiecePlacement(placePieceButton.PieceType == typeof(Pawn)))
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
            }
        }

        public void ClearSelection()
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

            UserInterfaceState = GameUIState.Default;
        }

        protected void AddDefaultButtons()
        {
            Buttons.Add(new PlacePieceButton(_textureLibrary, _fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 60, 150, 35), typeof(Queen)));
            Buttons.Add(new PlacePieceButton(_textureLibrary, _fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 100, 150, 35), typeof(Rook)));
            Buttons.Add(new PlacePieceButton(_textureLibrary, _fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 140, 150, 35), typeof(Bishop)));
            Buttons.Add(new PlacePieceButton(_textureLibrary, _fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 180, 150, 35), typeof(Knight)));
            Buttons.Add(new PlacePieceButton(_textureLibrary, _fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, 220, 150, 35), typeof(Pawn)));

            Buttons.Add(new Button(_textureLibrary, _fontLibrary, new Rectangle(ControlPanelStartsAtX + 15, HeightPixels - 180, 150, 35), ButtonType.CopyMapSeed, "Copy map seed"));
        }
    }
}
