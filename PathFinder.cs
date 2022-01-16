using System.Collections.Generic;
using System.Linq;

namespace AgeOfChess
{
    class PathFinder
    {
        private readonly Map _map;

        public PathFinder(Map map)
        {
            _map = map;
        }

        public IEnumerable<Square> FindLegalDestinationSquares(Piece piece, Square sourceSquare, bool nestedCall = false)
        {
            var legalSquares = new List<Square>();

            if (piece is Pawn)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.North, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.East, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.South, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.West, sourceSquare, 1));

                legalSquares.RemoveAll(e => e.Object != null && e.Object is Piece);

                var legalCaptures = new List<Square>();
                legalCaptures.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, sourceSquare, 1));
                legalCaptures.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, sourceSquare, 1));
                legalCaptures.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, sourceSquare, 1));
                legalCaptures.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, sourceSquare, 1));

                legalCaptures.RemoveAll(e => e.Object == null || !(e.Object is Piece occupyingPiece && occupyingPiece.IsWhite != piece.IsWhite));

                legalSquares.AddRange(legalCaptures);
            }
            else if (piece is King)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.North, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.East, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.South, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.West, sourceSquare, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, sourceSquare, 1));

                if (!nestedCall) // Prevents recursion if 2 kings are facing off
                {
                    FindAllLegalMovesByColor(!piece.IsWhite);
                }
            }
            else if (piece is Rook)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.North, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.East, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.South, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.West, sourceSquare));
            }
            else if (piece is Bishop)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, sourceSquare));
            }
            else if (piece is Queen)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.North, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.East, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.South, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.West, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, sourceSquare));
            }
            else if (piece is Knight)
            {
                legalSquares.Add(_map.GetSquareByCoordinates(sourceSquare.X + 1, sourceSquare.Y + 2));
                legalSquares.Add(_map.GetSquareByCoordinates(sourceSquare.X + 1, sourceSquare.Y - 2));
                legalSquares.Add(_map.GetSquareByCoordinates(sourceSquare.X - 1, sourceSquare.Y + 2));
                legalSquares.Add(_map.GetSquareByCoordinates(sourceSquare.X - 1, sourceSquare.Y - 2));
                legalSquares.Add(_map.GetSquareByCoordinates(sourceSquare.X + 2, sourceSquare.Y + 1));
                legalSquares.Add(_map.GetSquareByCoordinates(sourceSquare.X + 2, sourceSquare.Y - 1));
                legalSquares.Add(_map.GetSquareByCoordinates(sourceSquare.X - 2, sourceSquare.Y + 1));
                legalSquares.Add(_map.GetSquareByCoordinates(sourceSquare.X - 2, sourceSquare.Y - 1));

                legalSquares.RemoveAll(e => e == null 
                    || e.Type == SquareType.DirtRocks 
                    || e.Type == SquareType.GrassRocks 
                    || (e.Object != null && e.Object is Piece occupyingPiece && occupyingPiece.IsWhite == piece.IsWhite));
            }

            return legalSquares;
        }

        public IEnumerable<Square> FindLegalSquaresVector(Piece piece, Direction direction, Square sourceSquare, int? maxSteps = null)
        {
            var legalSquares = new List<Square>();

            Square currentSquare = sourceSquare;
            int stepsTaken = 0;

            while (stepsTaken != maxSteps)
            {
                switch (direction)
                {
                    case Direction.North: currentSquare = _map.GetSquareByCoordinates(currentSquare.X, currentSquare.Y - 1); break;
                    case Direction.NorthEast: currentSquare = _map.GetSquareByCoordinates(currentSquare.X + 1, currentSquare.Y - 1); break;
                    case Direction.East: currentSquare = _map.GetSquareByCoordinates(currentSquare.X + 1, currentSquare.Y); break;
                    case Direction.SouthEast: currentSquare = _map.GetSquareByCoordinates(currentSquare.X + 1, currentSquare.Y + 1); break;
                    case Direction.South: currentSquare = _map.GetSquareByCoordinates(currentSquare.X, currentSquare.Y + 1); break;
                    case Direction.SouthWest: currentSquare = _map.GetSquareByCoordinates(currentSquare.X - 1, currentSquare.Y + 1); break;
                    case Direction.West: currentSquare = _map.GetSquareByCoordinates(currentSquare.X - 1, currentSquare.Y); break;
                    case Direction.NorthWest: currentSquare = _map.GetSquareByCoordinates(currentSquare.X - 1, currentSquare.Y - 1); break;
                }

                if (currentSquare == null)
                {
                    break;
                }

                if (currentSquare.Type == SquareType.DirtRocks || currentSquare.Type == SquareType.GrassRocks)
                {
                    break;
                }

                if (currentSquare.Type == SquareType.DirtMine
                    || currentSquare.Type == SquareType.GrassMine
                    || currentSquare.Type == SquareType.DirtTrees
                    || currentSquare.Type == SquareType.GrassTrees)
                {
                    legalSquares.Add(currentSquare);
                    break;
                }

                if (currentSquare.Object != null)
                {
                    if (currentSquare.Object is Piece occupyingPiece)
                    {
                        if (occupyingPiece.IsWhite != piece.IsWhite)
                        {
                            legalSquares.Add(currentSquare);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        legalSquares.Add(currentSquare);
                        break;
                    }
                }

                stepsTaken++;
                legalSquares.Add(currentSquare);
            }

            return legalSquares;
        }

        public IEnumerable<Square> FindAllLegalMovesByColor(bool isWhite)
        {
            foreach (Square pieceSquare in _map.Squares.Where(e => e.Object != null && e.Object is Piece piece && piece.IsWhite == isWhite))
            {
                Piece piece = (Piece)pieceSquare.Object;


            }
        }
    }
}
