using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AgeOfChess
{
    class Square
    {
        public int X { get; }
        public int Y { get; }
        public int Id { get; }
        public SquareType Type { get; private set; }
        public bool IsSelected { get; set; }
        public PlaceableObject Object { get; private set; }

        private Texture2D _texture;
        private readonly Rectangle _location;
        private readonly Rectangle _objectLocation;
        private SquareColor _temporaryColor;

        private readonly TextureLibrary _textureLibrary;

        public Square(int x, int y, int id, TextureLibrary textureLibrary, Rectangle location)
        {
            X = x;
            Y = y;
            Id = id;
            _location = location;

            _textureLibrary = textureLibrary;

            _objectLocation = new Rectangle(
                _location.X + _location.Width / 8,
                _location.Y + _location.Height / 8,
                (int)Math.Floor(_location.Width * 0.75),
                (int)Math.Floor(_location.Height * 0.75));
        }

        public bool LocationIncludesPoint(Point point) => _location.Contains(point);

        public bool IsAccessible() => Object == null && Type == SquareType.Dirt;

        public void SetType(SquareType squareType)
        {
            Type = squareType;
            _texture = _textureLibrary.GetSquareTextureByType(squareType);
        }

        public void SetObject(PlaceableObject obj)
        {
            Object = obj;
        }

        public void ClearObject()
        {
            Object = null;
        }

        public void SetTemporaryColor(SquareColor color)
        {
            _temporaryColor = color;
        }

        public void ClearTemporaryColor()
        {
            _temporaryColor = SquareColor.None;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _location, Color.White);

            if (_temporaryColor != SquareColor.None)
            {
                spriteBatch.Draw(_textureLibrary.GetSquareColorByType(_temporaryColor), _location, Color.White);
            }

            if (Object != null)
            {
                if (IsSelected)
                {
                    spriteBatch.Draw(
                        _textureLibrary.GetSquareColorByType(SquareColor.Blue),
                        _location,
                        Color.White);
                }
                else if (_temporaryColor == SquareColor.None && Object is Piece && (Type == SquareType.DirtMine || Type == SquareType.GrassMine))
                {
                    spriteBatch.Draw(
                        _textureLibrary.GetSquareColorByType(SquareColor.Orange),
                        _location,
                        Color.White);
                }

                Object.Draw(spriteBatch, _objectLocation);
            }
        }
    }
}
