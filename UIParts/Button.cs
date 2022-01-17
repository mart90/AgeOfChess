using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AgeOfChess
{
    class Button : IUiPart
    {
        public string Text { get; set; }
        public bool IsSelected { get; set; }
        public ButtonType Type { get; set; }
        public bool IsEnabled { get; set; }

        private readonly Texture2D _texture;
        private readonly Rectangle _location;

        protected readonly Rectangle ObjectLocation;
        protected readonly Rectangle TextLocation;

        protected readonly TextureLibrary TextureLibrary;
        protected readonly FontLibrary FontLibrary;

        public Button(TextureLibrary textureLibrary, FontLibrary fontLibrary, Rectangle location, ButtonType type, string text = null, bool isEnabled = true)
        {
            TextureLibrary = textureLibrary;
            FontLibrary = fontLibrary;

            _texture = textureLibrary.GetButtonTextureByType(type);
            _location = new Rectangle(location.X, location.Y, location.Width, location.Height);

            Type = type;
            Text = text;
            IsEnabled = isEnabled;

            ObjectLocation = new Rectangle(
                (int)Math.Floor(_location.X + _location.Width * 0.38), 
                _location.Y + 1, 
                _location.Height - 2, 
                _location.Height - 2);

            TextLocation = new Rectangle(
                _location.X + _location.Width / 10, 
                _location.Y + _location.Height / 4, 
                _location.Width - _location.Width / 10, 
                _location.Height - _location.Height / 4);
        }

        public Point Center => _location.Center;

        public bool LocationIncludesPoint(Point point) => _location.Contains(point);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!IsEnabled)
            {
                return;
            }

            spriteBatch.Draw(_texture, _location, Color.White);

            if (Text != null)
            {
                spriteBatch.DrawString(FontLibrary.DefaultFont, Text, new Vector2(TextLocation.X, TextLocation.Y), Color.White);
            }
        }
    }
}
