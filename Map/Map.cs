using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgeOfChess
{
    class Map
    {
        public List<Square> Squares { get; }
        public int Width { get; }
        public int Height { get; }
        public string Seed { get; set; }

        private readonly Random _random;

        public Map(int width, int height)
        {
            _random = new Random();

            Width = width;
            Height = height;

            Squares = new List<Square>();
        }

        public Square SelectedSquare => Squares.SingleOrDefault(e => e.IsSelected);

        public Square GetSquareByCoordinates(int x, int y) => Squares.SingleOrDefault(e => e.X == x && e.Y == y);

        public void SetSeed()
        {
            string seedString = $"{Width}x{Height}_";

            int consecutiveEmptySquares = 0;

            foreach (Square square in Squares.Where(e => e.Id < Width * Height / 2 - 1).OrderBy(e => e.Id))
            {
                if (square.Object == null && (square.Type == SquareType.Dirt || square.Type == SquareType.Grass))
                {
                    consecutiveEmptySquares++;

                    if (consecutiveEmptySquares == 9)
                    {
                        seedString += "9";
                        consecutiveEmptySquares = 0;
                    }

                    continue;
                }
                else if (consecutiveEmptySquares != 0)
                {
                    seedString += consecutiveEmptySquares.ToString();
                    consecutiveEmptySquares = 0;
                }

                if (square.Object != null)
                {
                    seedString += square.Object is King ? "k" : "t";
                }
                else if (square.Type == SquareType.GrassMine || square.Type == SquareType.DirtMine)
                {
                    seedString += "m";
                }
                else if (square.Type == SquareType.DirtRocks || square.Type == SquareType.GrassRocks)
                {
                    seedString += "r";
                }
                else if (square.Type == SquareType.DirtTrees || square.Type == SquareType.GrassTrees)
                {
                    seedString += "f";
                }
            }
            if (consecutiveEmptySquares != 0)
            {
                seedString += consecutiveEmptySquares.ToString();
            }

            Seed = seedString;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var square in Squares)
            {
                square.Draw(spriteBatch);
            }
        }

        public IEnumerable<Square> GetSquaresByType(SquareType squareType)
        {
            return Squares.Where(e => e.Type == squareType);
        }

        public IEnumerable<Square> GetMines()
        {
            return GetSquaresByType(SquareType.DirtMine).Concat(GetSquaresByType(SquareType.GrassMine));
        }

        public Square GetRandomSquareOfType(SquareType squareType, int startingSquare = 0, int endingSquare = 0)
        {
            List<Square> squares = GetSquaresByType(squareType).ToList();

            if (startingSquare != 0)
            {
                squares.RemoveAll(e => e.Id < startingSquare);
            }
            if (endingSquare != 0)
            {
                squares.RemoveAll(e => e.Id > endingSquare);
            }

            var randomTileNumber = _random.Next(0, squares.Count() - 1);
            return squares[randomTileNumber];
        }

        public Square GetRandomEmptySquare(int startingSquare = 0, int endingSquare = 0)
        {
            var randomEmptyType = _random.Next(0, 1) == 0 ? SquareType.Dirt : SquareType.Grass;
            return GetRandomSquareOfType(randomEmptyType, startingSquare, endingSquare);
        }

        public IEnumerable<Square> FindLegalDestinationsForPiece(Piece piece, Square sourceSquare)
        {
            return new PathFinder(this).FindLegalDestinationSquares(piece, sourceSquare);
        }
    }
}
