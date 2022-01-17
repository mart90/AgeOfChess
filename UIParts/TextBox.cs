using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AgeOfChess
{
    class TextBox : IUiPart
    {
        public string Text { get; set; }
        public bool HasFocus { get; set; }
        public TextBoxType Type { get; set; }

        private readonly string _description;

        private readonly Texture2D _texture;
        private readonly Rectangle _location;
        private readonly Rectangle _descriptionLocation;
        private readonly Rectangle _textLocation;

        protected readonly FontLibrary FontLibrary;

        public TextBox(TextureLibrary textureLibrary, FontLibrary fontLibrary, Rectangle location, TextBoxType type, string description, string initialText = null)
        {
            _texture = textureLibrary.TextBoxTexture;
            FontLibrary = fontLibrary;

            Type = type;
            _location = location;
            
            _descriptionLocation = new Rectangle(
                _location.X - description.Length * 9,
                _location.Y + 4,
                description.Length * 9,
                15);

            _textLocation = new Rectangle(
                _location.X + 2,
                _location.Y + 2,
                _location.Width,
                10);

            _description = description;
            Text = initialText ?? "";
        }

        public Point Center => _location.Center;

        public bool LocationIncludesPoint(Point point) => _location.Contains(point);

        public void Focus()
        {
            HasFocus = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _location, Color.White);
            spriteBatch.DrawString(FontLibrary.DefaultFont, _description, new Vector2(_descriptionLocation.X, _descriptionLocation.Y), Color.Black);

            if (HasFocus)
            {
                spriteBatch.DrawString(FontLibrary.DefaultFont, Text + "_", new Vector2(_textLocation.X, _textLocation.Y), Color.White);
            }
            else
            {
                spriteBatch.DrawString(FontLibrary.DefaultFont, Text, new Vector2(_textLocation.X, _textLocation.Y), Color.White);
            }
        }
    }
}
