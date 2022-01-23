using System;
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

        public IEnumerable<Square> FindLegalDestinationSquares(Piece piece, Square sourceSquare, bool checkingForChecks = false)
        {
            var legalSquares = new List<Square>();

            if (!checkingForChecks)
            {
                (bool isPinned, List<Square> legalSquaresDespitePin) = CheckForPinToKing(sourceSquare, piece);

                if (isPinned)
                {
                    return legalSquaresDespitePin;
                }
            }

            if (piece is Pawn)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.North, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.East, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.South, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.West, sourceSquare, checkingForChecks, 1));

                legalSquares.RemoveAll(e => e.Object != null && e.Object is Piece);

                var legalCaptures = new List<Square>();
                legalCaptures.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, sourceSquare, checkingForChecks, 1));
                legalCaptures.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, sourceSquare, checkingForChecks, 1));
                legalCaptures.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, sourceSquare, checkingForChecks, 1));
                legalCaptures.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, sourceSquare, checkingForChecks, 1));

                legalCaptures.RemoveAll(e => e.Object == null || !(e.Object is Piece occupyingPiece && occupyingPiece.IsWhite != piece.IsWhite));

                legalSquares.AddRange(legalCaptures);
            }
            else if (piece is King)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.North, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.East, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.South, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.West, sourceSquare, checkingForChecks, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, sourceSquare, checkingForChecks, 1));

                if (!checkingForChecks) // Prevents infinite recursion
                {
                    legalSquares.RemoveAll(e => FindAttacksForColor(!piece.IsWhite).Contains(e));
                }
            }
            else if (piece is Rook)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.North, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.East, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.South, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.West, sourceSquare, checkingForChecks));
            }
            else if (piece is Bishop)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, sourceSquare, checkingForChecks));
            }
            else if (piece is Queen)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.North, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.East, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.South, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.West, sourceSquare, checkingForChecks));
                legalSquares.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, sourceSquare, checkingForChecks));
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
                    || (!checkingForChecks && e.Object != null && e.Object is Piece occupyingPiece && occupyingPiece.IsWhite == piece.IsWhite));
            }

            if (!(piece is King) && !checkingForChecks) // Prevents infinite recursion
            {
                Square kingSquare = _map.Squares.Single(e => e.Object is King king && king.IsWhite == piece.IsWhite);

                List<Square> checkingSquares = FindCheckingSourceSquares(kingSquare);

                if (checkingSquares.Any())
                {
                    var tempList = legalSquares.ToList();

                    foreach (var destinationSquare in legalSquares)
                    {
                        if (!DestinationFixesCheck(checkingSquares, destinationSquare, piece.IsWhite))
                        {
                            tempList.Remove(destinationSquare);
                        }
                    }

                    legalSquares = tempList;
                }
            }

            return legalSquares;
        }

        public IEnumerable<Square> FindLegalDestinationsForPiecePlacement(bool pieceIsWhite, bool placingPawn = false, bool recurring = false)
        {
            var legalDestinations = new List<Square>();

            Square activePlayerKingSquare = _map.Squares.Single(e => e.Object != null
                && e.Object is Piece piece
                && piece.IsWhite == pieceIsWhite
                && piece is King);

            legalDestinations.AddRange(FindLegalPiecePlacementsAroundSquare(activePlayerKingSquare).ToList());

            if (placingPawn)
            {
                List<Square> activePlayerOtherPieceSquares = _map.Squares
                    .Where(e => e.Object != null
                        && e.Object is Piece piece
                        && piece.IsWhite == pieceIsWhite
                        && !(piece is Pawn)
                        && !(piece is King))
                    .ToList();

                foreach (Square square in activePlayerOtherPieceSquares)
                {
                    legalDestinations.AddRange(FindLegalPiecePlacementsAroundSquare(square));
                }
            }

            if (!recurring)
            {
                Square kingSquare = _map.Squares.Single(e => e.Object is King king && king.IsWhite == pieceIsWhite);

                List<Square> checkingSquares = FindCheckingSourceSquares(kingSquare);

                if (checkingSquares.Any())
                {
                    var tempList = legalDestinations.ToList();

                    foreach (var destinationSquare in legalDestinations)
                    {
                        if (!DestinationFixesCheck(checkingSquares, destinationSquare, pieceIsWhite))
                        {
                            tempList.Remove(destinationSquare);
                        }
                    }

                    legalDestinations = tempList;
                }
            }

            return legalDestinations;
        }

        private IEnumerable<Square> FindLegalPiecePlacementsAroundSquare(Square square)
        {
            var legalSquares = new List<Square>();

            legalSquares.AddRange(FindLegalSquaresVector((Piece)square.Object, Direction.North, square, true, 1));
            legalSquares.AddRange(FindLegalSquaresVector((Piece)square.Object, Direction.NorthEast, square, true, 1));
            legalSquares.AddRange(FindLegalSquaresVector((Piece)square.Object, Direction.East, square, true, 1));
            legalSquares.AddRange(FindLegalSquaresVector((Piece)square.Object, Direction.SouthEast, square, true, 1));
            legalSquares.AddRange(FindLegalSquaresVector((Piece)square.Object, Direction.South, square, true, 1));
            legalSquares.AddRange(FindLegalSquaresVector((Piece)square.Object, Direction.SouthWest, square, true, 1));
            legalSquares.AddRange(FindLegalSquaresVector((Piece)square.Object, Direction.West, square, true, 1));
            legalSquares.AddRange(FindLegalSquaresVector((Piece)square.Object, Direction.NorthWest, square, true, 1));

            return legalSquares.Where(e => e.Object == null);
        }

        public IEnumerable<Square> FindAttacksForColor(bool isWhite)
        {
            var legalAttacks = new List<Square>();

            foreach (Square pieceSquare in _map.Squares.Where(e => e.Object != null && e.Object is Piece piece && piece.IsWhite == isWhite))
            {
                Piece piece = (Piece)pieceSquare.Object;

                if (piece is Pawn)
                {
                    legalAttacks.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, pieceSquare, true, 1));
                    legalAttacks.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, pieceSquare, true, 1));
                    legalAttacks.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, pieceSquare, true, 1));
                    legalAttacks.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, pieceSquare, true, 1));
                }
                else
                {
                    legalAttacks.AddRange(FindLegalDestinationSquares(piece, pieceSquare, true));
                }
            }

            return legalAttacks;
        }

        /// <summary>
        /// CheckingForChecks is for when we are finding legal moves for the OTHER player to see if we can move our king somewhere.
        /// <br/>In that case we pretend we CAN capture our own pieces (recapture in case the king captures them), and ignore the obstacle of the king we are checking so that it can't move away from us on that same diagonal or file.
        /// </summary>
        private IEnumerable<Square> FindLegalSquaresVector(Piece piece, Direction direction, Square sourceSquare, bool checkingForChecks = false, int? maxSteps = null)
        {
            var legalSquares = new List<Square>();

            Square currentSquare = sourceSquare;
            int stepsTaken = 0;

            while (stepsTaken != maxSteps)
            {
                currentSquare = GetNextSquare(currentSquare, direction);

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
                    if (currentSquare.Object != null 
                        && currentSquare.Object is Piece occupyingPiece 
                        && occupyingPiece.IsWhite == piece.IsWhite
                        && !checkingForChecks)
                    {
                        break;
                    }

                    legalSquares.Add(currentSquare);
                    break;
                }

                if (currentSquare.Object != null)
                {
                    if (currentSquare.Object is Piece occupyingPiece)
                    {
                        if (occupyingPiece.IsWhite == piece.IsWhite && !checkingForChecks)
                        {
                            break;
                        }
                        else if (occupyingPiece.IsWhite != piece.IsWhite && occupyingPiece is King && checkingForChecks)
                        {
                            stepsTaken++;
                            legalSquares.Add(currentSquare);
                            continue;
                        }
                    }

                    legalSquares.Add(currentSquare);
                    break;
                }

                stepsTaken++;
                legalSquares.Add(currentSquare);
            }

            return legalSquares;
        }

        private Square GetNextSquare(Square sourceSquare, Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return _map.GetSquareByCoordinates(sourceSquare.X, sourceSquare.Y - 1);
                case Direction.NorthEast: return _map.GetSquareByCoordinates(sourceSquare.X + 1, sourceSquare.Y - 1);
                case Direction.East: return _map.GetSquareByCoordinates(sourceSquare.X + 1, sourceSquare.Y);
                case Direction.SouthEast: return _map.GetSquareByCoordinates(sourceSquare.X + 1, sourceSquare.Y + 1);
                case Direction.South: return _map.GetSquareByCoordinates(sourceSquare.X, sourceSquare.Y + 1);
                case Direction.SouthWest: return _map.GetSquareByCoordinates(sourceSquare.X - 1, sourceSquare.Y + 1);
                case Direction.West: return _map.GetSquareByCoordinates(sourceSquare.X - 1, sourceSquare.Y);
                case Direction.NorthWest: return _map.GetSquareByCoordinates(sourceSquare.X - 1, sourceSquare.Y - 1);
                default: return null;
            };
        }

        private bool DestinationFixesCheck(List<Square> checkingSourceSquares, Square destinationSquare, bool kingIsWhite)
        {
            if (checkingSourceSquares.Count > 1)
            {
                // Only the king can move
                return false;
            }

            if (destinationSquare == checkingSourceSquares[0])
            {
                return true;
            }

            Piece checkingPiece = (Piece)checkingSourceSquares[0].Object;

            if (checkingPiece is Knight)
            {
                return false;
            }

            var allDirections = new List<Direction>
            {
                Direction.North,
                Direction.NorthEast,
                Direction.East,
                Direction.SouthEast,
                Direction.South,
                Direction.SouthWest,
                Direction.West,
                Direction.NorthWest
            };

            Direction kingDirection = Direction.North;

            foreach (Direction direction in allDirections)
            {
                if (IsKingFile(checkingSourceSquares[0], kingIsWhite, direction))
                {
                    kingDirection = direction;
                }
            }

            var currentSquare = checkingSourceSquares[0];

            while (!(currentSquare.Object is King))
            {
                currentSquare = GetNextSquare(currentSquare, kingDirection);

                if (currentSquare == destinationSquare)
                {
                    return true;
                }
            }

            return false;
        }

        private List<Square> FindCheckingSourceSquares(Square kingSquare)
        {
            bool checkingColorIsWhite = !((Piece)kingSquare.Object).IsWhite;

            var checkingSquares = new List<Square>();

            foreach (Square pieceSquare in _map.Squares.Where(e => e.Object != null && e.Object is Piece piece && piece.IsWhite == checkingColorIsWhite))
            {
                var legalAttacks = new List<Square>();
                
                Piece piece = (Piece)pieceSquare.Object;

                if (piece is Pawn)
                {
                    legalAttacks.AddRange(FindLegalSquaresVector(piece, Direction.NorthEast, pieceSquare, true, 1));
                    legalAttacks.AddRange(FindLegalSquaresVector(piece, Direction.SouthEast, pieceSquare, true, 1));
                    legalAttacks.AddRange(FindLegalSquaresVector(piece, Direction.SouthWest, pieceSquare, true, 1));
                    legalAttacks.AddRange(FindLegalSquaresVector(piece, Direction.NorthWest, pieceSquare, true, 1));
                }
                else
                {
                    legalAttacks.AddRange(FindLegalDestinationSquares(piece, pieceSquare, true));
                }

                if (legalAttacks.Contains(kingSquare))
                {
                    checkingSquares.Add(pieceSquare);
                }
            }

            return checkingSquares;
        }

        /// <summary>
        /// If the piece is pinned to its own king, it can't move except on that file/diagonal
        /// </summary>
        private (bool isPinned, List<Square> legalMovesDespitePin) CheckForPinToKing(Square sourceSquare, Piece piece)
        {
            if (sourceSquare.Type == SquareType.DirtMine
                || sourceSquare.Type == SquareType.GrassMine
                || sourceSquare.Type == SquareType.DirtTrees
                || sourceSquare.Type == SquareType.GrassTrees)
            {
                return (false, null);
            }

            var allDirections = new List<Direction>
            {
                Direction.North,
                Direction.NorthEast,
                Direction.East,
                Direction.SouthEast,
                Direction.South,
                Direction.SouthWest,
                Direction.West,
                Direction.NorthWest
            };

            Direction? kingDirection = null;

            foreach (Direction direction in allDirections)
            {
                if (IsKingFile(sourceSquare, piece.IsWhite, direction))
                {
                    kingDirection = direction;
                }
            }

            if (kingDirection == null)
            {
                return (false, null);
            }

            Direction oppositeDirection = (int)kingDirection.Value <= 3 ? kingDirection.Value + 4 : kingDirection.Value - 4;

            if (!FileHasPinningPiece(sourceSquare, !piece.IsWhite, oppositeDirection))
            {
                return (false, null);
            }

            if (piece is Knight)
            {
                return (true, new List<Square>());
            }

            var legalSquares = new List<Square>();

            if (piece is Queen)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, kingDirection.Value, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, oppositeDirection, sourceSquare));
            }
            else if (piece is Rook && (int)kingDirection.Value % 2 == 0)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, kingDirection.Value, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, oppositeDirection, sourceSquare));
            }
            else if (piece is Bishop && (int)kingDirection.Value % 2 == 1)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, kingDirection.Value, sourceSquare));
                legalSquares.AddRange(FindLegalSquaresVector(piece, oppositeDirection, sourceSquare));
            }
            else if (piece is Pawn && (int)kingDirection.Value % 2 == 0)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, kingDirection.Value, sourceSquare, false, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, oppositeDirection, sourceSquare, false, 1));

                legalSquares.RemoveAll(e => e.Object != null && e.Object is Piece);
            }
            else if (piece is Pawn && (int)kingDirection.Value % 2 == 1)
            {
                legalSquares.AddRange(FindLegalSquaresVector(piece, kingDirection.Value, sourceSquare, false, 1));
                legalSquares.AddRange(FindLegalSquaresVector(piece, oppositeDirection, sourceSquare, false, 1));

                legalSquares.RemoveAll(e => e.Object == null || !(e.Object is Piece occupyingPiece && occupyingPiece.IsWhite != piece.IsWhite));
            }

            return (true, legalSquares);
        }

        private bool IsKingFile(Square sourceSquare, bool kingIsWhite, Direction direction)
        {
            var currentSquare = sourceSquare;

            while (true)
            {
                currentSquare = GetNextSquare(currentSquare, direction);

                if (currentSquare == null)
                {
                    return false;
                }

                if (currentSquare.Type == SquareType.DirtRocks 
                    || currentSquare.Type == SquareType.GrassRocks)
                {
                    return false;
                }

                if (currentSquare.Object != null)
                {
                    if (currentSquare.Object is King king && king.IsWhite == kingIsWhite)
                    {
                        return true;
                    }

                    return false;
                }

                if (currentSquare.Type == SquareType.DirtMine
                    || currentSquare.Type == SquareType.GrassMine
                    || currentSquare.Type == SquareType.DirtTrees
                    || currentSquare.Type == SquareType.GrassTrees)
                {
                    return false;
                }
            }
        }

        private bool FileHasPinningPiece(Square sourceSquare, bool pinningPieceIsWhite, Direction direction)
        {
            var currentSquare = sourceSquare;

            while (true)
            {
                currentSquare = GetNextSquare(currentSquare, direction);

                if (currentSquare == null)
                {
                    return false;
                }

                if (currentSquare.Type == SquareType.DirtRocks || currentSquare.Type == SquareType.GrassRocks)
                {
                    return false;
                }

                if (currentSquare.Object != null)
                {
                    if (currentSquare.Object is Piece piece && piece.IsWhite == pinningPieceIsWhite)
                    {
                        if (piece is Queen)
                        {
                            return true;
                        }

                        if (direction == Direction.North
                            || direction == Direction.South
                            || direction == Direction.East
                            || direction == Direction.West)
                        {
                            return piece is Rook;
                        }
                        
                        if (direction == Direction.NorthEast
                            || direction == Direction.SouthEast
                            || direction == Direction.SouthWest
                            || direction == Direction.NorthWest)
                        {
                            return piece is Bishop;
                        }
                    }

                    return false;
                }

                if (currentSquare.Type == SquareType.DirtMine
                    || currentSquare.Type == SquareType.GrassMine
                    || currentSquare.Type == SquareType.DirtTrees
                    || currentSquare.Type == SquareType.GrassTrees)
                {
                    return false;
                }
            }
        }
    }
}
