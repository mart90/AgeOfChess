using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AgeOfChess
{
    class TextBox
    {
        public string Text { get; set; }
        public bool IsSelected { get; set; }
        public TextBoxType Type { get; set; }

        private readonly Texture2D _texture;
        private readonly Rectangle _location;

        protected readonly Rectangle ObjectLocation;
        protected readonly Rectangle TextLocation;

        protected readonly TextureLibrary TextureLibrary;
        protected readonly FontLibrary FontLibrary;
    }
}
