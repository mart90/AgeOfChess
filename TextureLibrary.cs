using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgeOfChess
{
    class TextureLibrary
    {
        private readonly List<SquareTexture> _squareTextures;
        private readonly List<PlaceableObjectTexture> _objectTextures;
        private readonly List<SquareColorTexture> _colorTextures;
        private readonly List<ButtonTexture> _buttonTextures;

        public TextureLibrary(ContentManager contentManager)
        {
            _squareTextures = new List<SquareTexture>();
            _objectTextures = new List<PlaceableObjectTexture>();
            _colorTextures = new List<SquareColorTexture>();
            _buttonTextures = new List<ButtonTexture>();

            AddSquareTextures(contentManager);
            AddObjectTextures(contentManager);
            AddColorTextures(contentManager);
            AddButtonTextures(contentManager);
        }

        public Texture2D GetObjectTextureByType(Type pieceType)
        {
            return _objectTextures.Single(e => e.PlaceableObjectType == pieceType).Texture;
        }

        public Texture2D GetSquareTextureByType(SquareType squareType)
        {
            return _squareTextures.Single(e => e.SquareType == squareType).Texture;
        }

        public Texture2D GetSquareColorByType(SquareColor tileColor)
        {
            return _colorTextures.Single(e => e.SquareColor == tileColor).Texture;
        }

        public Texture2D GetButtonTextureByType(ButtonType type)
        {
            var texture = _buttonTextures.SingleOrDefault(e => e.ButtonType == type)?.Texture;
            return texture ?? _buttonTextures.Single(e => e.ButtonType == ButtonType.Default).Texture;
        }

        private void AddSquareTextures(ContentManager contentManager)
        {
            _squareTextures.Add(new SquareTexture
            {
                SquareType = SquareType.Dirt,
                Texture = contentManager.Load<Texture2D>("squares/dirt")
            });
            _squareTextures.Add(new SquareTexture
            {
                SquareType = SquareType.DirtRocks,
                Texture = contentManager.Load<Texture2D>("squares/dirt_rocks")
            });
            _squareTextures.Add(new SquareTexture
            {
                SquareType = SquareType.DirtMine,
                Texture = contentManager.Load<Texture2D>("squares/dirt_mine")
            });
            _squareTextures.Add(new SquareTexture
            {
                SquareType = SquareType.DirtTrees,
                Texture = contentManager.Load<Texture2D>("squares/dirt_trees")
            });
            _squareTextures.Add(new SquareTexture
            {
                SquareType = SquareType.Grass,
                Texture = contentManager.Load<Texture2D>("squares/grass")
            });
            _squareTextures.Add(new SquareTexture
            {
                SquareType = SquareType.GrassRocks,
                Texture = contentManager.Load<Texture2D>("squares/grass_rocks")
            });
            _squareTextures.Add(new SquareTexture
            {
                SquareType = SquareType.GrassMine,
                Texture = contentManager.Load<Texture2D>("squares/grass_mine")
            });
            _squareTextures.Add(new SquareTexture
            {
                SquareType = SquareType.GrassTrees,
                Texture = contentManager.Load<Texture2D>("squares/grass_trees")
            });
        }

        private void AddObjectTextures(ContentManager contentManager)
        {
            // Resources
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(Treasure),
                Texture = contentManager.Load<Texture2D>("objects/treasure")
            });

            // Pieces
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(WhiteQueen),
                Texture = contentManager.Load<Texture2D>("objects/w_queen")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(BlackQueen),
                Texture = contentManager.Load<Texture2D>("objects/b_queen")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(WhiteKing),
                Texture = contentManager.Load<Texture2D>("objects/w_king")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(BlackKing),
                Texture = contentManager.Load<Texture2D>("objects/b_king")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(WhiteKnight),
                Texture = contentManager.Load<Texture2D>("objects/w_knight")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(BlackKnight),
                Texture = contentManager.Load<Texture2D>("objects/b_knight")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(WhiteRook),
                Texture = contentManager.Load<Texture2D>("objects/w_rook")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(BlackRook),
                Texture = contentManager.Load<Texture2D>("objects/b_rook")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(WhiteBishop),
                Texture = contentManager.Load<Texture2D>("objects/w_bishop")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(BlackBishop),
                Texture = contentManager.Load<Texture2D>("objects/b_bishop")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(WhitePawn),
                Texture = contentManager.Load<Texture2D>("objects/w_pawn")
            });
            _objectTextures.Add(new PlaceableObjectTexture
            {
                PlaceableObjectType = typeof(BlackPawn),
                Texture = contentManager.Load<Texture2D>("objects/b_pawn")
            });
        }

        private void AddColorTextures(ContentManager contentManager)
        {
            _colorTextures.Add(new SquareColorTexture
            {
                SquareColor = SquareColor.Blue,
                Texture = contentManager.Load<Texture2D>("colors/blue")
            });
            _colorTextures.Add(new SquareColorTexture
            {
                SquareColor = SquareColor.Red,
                Texture = contentManager.Load<Texture2D>("colors/red")
            });
            _colorTextures.Add(new SquareColorTexture
            {
                SquareColor = SquareColor.Purple,
                Texture = contentManager.Load<Texture2D>("colors/purple")
            });
            _colorTextures.Add(new SquareColorTexture
            {
                SquareColor = SquareColor.Orange,
                Texture = contentManager.Load<Texture2D>("colors/orange")
            });
            _colorTextures.Add(new SquareColorTexture
            {
                SquareColor = SquareColor.Green,
                Texture = contentManager.Load<Texture2D>("colors/green")
            });
        }

        private void AddButtonTextures(ContentManager contentManager)
        {
            _buttonTextures.Add(new ButtonTexture
            {
                ButtonType = ButtonType.Default,
                Texture = contentManager.Load<Texture2D>("buttons/default")
            });
        }
    }
}
