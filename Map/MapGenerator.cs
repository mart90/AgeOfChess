using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgeOfChess
{
    class MapGenerator
    {
        private readonly TextureLibrary _textureLibrary;
        private readonly int _squareDimensions;

        private Map _map;

        public MapGenerator(TextureLibrary textureLibrary, int squareRelativeSize)
        {
            _textureLibrary = textureLibrary;
            _squareDimensions = squareRelativeSize * 4;
        }

        public Map GenerateMap(int width, int height)
        {
            if (width % 2 != 0 || height % 2 != 0)
            {
                throw new Exception("No odd dimensions allowed");
            }

            if (width < 8 || height < 8 || width > 20 || height > 20)
            {
                throw new Exception("Map size can be between 8x8 and 20x20");
            }

            _map = new Map(width, height);

            MakeBaseSquares();

            AddRandomlyGeneratedSquares(SquareType.DirtRocks, 0.05);
            AddRandomlyGeneratedSquares(SquareType.GrassRocks, 0.05);
            AddRandomlyGeneratedSquares(SquareType.DirtTrees, 0.05);
            AddRandomlyGeneratedSquares(SquareType.GrassTrees, 0.05);
            AddRandomlyGeneratedSquares(SquareType.DirtMine, 0.01);
            AddRandomlyGeneratedSquares(SquareType.GrassMine, 0.01);

            AddRandomlyGeneratedGaiaObjects<Treasure>(0.02);

            SpawnKings();

            MirrorGeneratedHalf();

            _map.SetSeed();

            return _map;
        }

        public Map GenerateFromSeed(string seed)
        {
            string[] wxh = seed.Split('_')[0].Split('x');

            _map = new Map(int.Parse(wxh[0]), int.Parse(wxh[1]))
            {
                Seed = seed
            };

            MakeBaseSquares();

            int currentSquareId = 0;

            foreach (char c in seed.Split('_')[1])
            {
                if (int.TryParse(c.ToString(), out int emptySquares))
                {
                    currentSquareId += emptySquares;
                    continue;
                }

                Square currentSquare = _map.Squares.Single(e => e.Id == currentSquareId);

                if (c == 'k')
                {
                    currentSquare.SetObject(new WhiteKing(_textureLibrary));
                }
                else if (c == 't')
                {
                    currentSquare.SetObject(new Treasure(_textureLibrary));
                }
                else if (c == 'm')
                {
                    currentSquare.SetType(currentSquare.Type == SquareType.Dirt ? SquareType.DirtMine : SquareType.GrassMine);
                }
                else if (c == 'r')
                {
                    currentSquare.SetType(currentSquare.Type == SquareType.Dirt ? SquareType.DirtRocks : SquareType.GrassRocks);
                }
                else if (c == 'f')
                {
                    currentSquare.SetType(currentSquare.Type == SquareType.Dirt ? SquareType.DirtTrees : SquareType.GrassTrees);
                }

                currentSquareId++;
            }

            MirrorGeneratedHalf();

            return _map;
        }

        public bool ValidateSeed(string seed)
        {
            if (!seed.Contains('k'))
            {
                return false;
            }

            try
            {
                GenerateFromSeed(seed);
                _map = null;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void SpawnKings()
        {
            var whiteKingSquare = _map.GetRandomEmptySquare(0, _map.Height * _map.Width / 2 - 1);
            whiteKingSquare.SetObject(new WhiteKing(_textureLibrary));

            var blackKingSquare = _map.Squares.Single(e => e.Id == _map.Height * _map.Width - 1 - whiteKingSquare.Id);
            blackKingSquare.SetObject(new BlackKing(_textureLibrary));
        }

        public void AddRandomlyGeneratedSquares(SquareType squareType, double fractionOfMap)
        {
            int amountToAdd = (int)Math.Round(fractionOfMap * _map.Squares.Count * 0.5);

            if (amountToAdd == 0)
            {
                amountToAdd = 1;
            }

            var unoccupiedType = squareType == SquareType.DirtRocks || squareType == SquareType.DirtMine || squareType == SquareType.DirtTrees ? SquareType.Dirt : SquareType.Grass;

            for (var i = 0; i < amountToAdd; i++)
            {
                _map.GetRandomSquareOfType(unoccupiedType, 0, _map.Height * _map.Width / 2 - 1).SetType(squareType);
            }
        }

        public void AddRandomlyGeneratedGaiaObjects<T>(double fractionOfEmptySquares) where T : GaiaObject
        {
            IEnumerable<Square> emptySquares = _map.GetSquaresByType(SquareType.Dirt).Concat(_map.GetSquaresByType(SquareType.Grass));
            int amountToAdd = (int)Math.Round(fractionOfEmptySquares * emptySquares.Count());

            if (amountToAdd < 2)
            {
                amountToAdd = 2;
            }

            for (var i = 0; i < amountToAdd; i++)
            {
                GaiaObject obj = (T)Activator.CreateInstance(typeof(T), _textureLibrary);
                _map.GetRandomEmptySquare(0, _map.Height * _map.Width / 2 - 1).SetObject(obj);
            }
        }

        private void MakeBaseSquares()
        {
            for (var y = 0; y < _map.Height; y++)
            {
                for (var x = 0; x < _map.Width; x++)
                {
                    var squareLocation = new Point(_squareDimensions * x + x, y * _squareDimensions + y);
                    var squareSize = new Point(_squareDimensions, _squareDimensions);

                    var square = new Square(x, y, y * _map.Width + x, _textureLibrary, new Rectangle(squareLocation, squareSize));

                    SquareType squareType;
                    if (y % 2 == 0) // Even row
                    {
                        squareType = x % 2 == 0 ? SquareType.Grass : SquareType.Dirt;
                    }
                    else // Odd row
                    {
                        squareType = x % 2 == 0 ? SquareType.Dirt : SquareType.Grass;
                    }

                    square.SetType(squareType);

                    _map.Squares.Add(square);
                }
            }
        }

        private void MirrorGeneratedHalf()
        {
            foreach (var square in _map.Squares.Where(e => e.Id < _map.Height * _map.Width / 2 - 1))
            {
                SquareType type = square.Type;

                Square mirrorSquare = _map.Squares.Single(e => e.Id == _map.Height * _map.Width - 1 - square.Id);
                mirrorSquare.SetType(square.Type);

                if (square.Object != null) 
                {
                    if (square.Object is GaiaObject)
                    {
                        GaiaObject obj = (GaiaObject)Activator.CreateInstance(square.Object.GetType(), _textureLibrary);
                        mirrorSquare.SetObject(obj);
                    }
                    else if (square.Object is King)
                    {
                        mirrorSquare.SetObject(new BlackKing(_textureLibrary));
                    }
                }
            }
        }
    }
}