using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpticComa_Types;

namespace OpticComa_Main
{
    public class TileSystem
    {
        Vector2 Size;
        Texture2D SpriteSheet;
        bool[][][] TilePresent;
        Vector2[][] WhichTexture;
        int sizeOfTex;
        LayerDepth Layer = LayerDepth.MidgroundTiles;

        public TileSystem(Texture2D t, Vector2 s, List<Vector2> w, bool[][][] b, Vector2[][] f, int texsize, LayerDepth l)
        {
            SpriteSheet = t; Size = s;
            Layer = l;
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 locationOffset)
        {
            for (int i = 0; i <= TilePresent.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= TilePresent.GetUpperBound(1); j++)
                {
                    if (TilePresent[i][j][0])
                    {
                        Rectangle rect = new Rectangle((int)WhichTexture[i][j].X, (int)WhichTexture[i][j].Y, sizeOfTex, sizeOfTex);

                        spriteBatch.Draw
                        (
                            SpriteSheet,
                            new Vector2(i + locationOffset.X, j + locationOffset.Y),
                            rect,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            (float)Layer / 10f
                        );
                    }
                }
            }
        }
    }

    public class OldTileSystem
    {
        private int _width, _height;
        private int _rows, _columns;
        private int[,] _chosenRow, _chosenColumn;
        private List<Vector2> _tiles = new List<Vector2>();
        private Random _random = new Random();
        private Vector2 _location;
        private Texture2D _texture;

        //This method is called when you make a new TileSystem.
        public OldTileSystem(Texture2D t, int numRows, int numColumns, int level, Vector2 levelSize, List<Vector2> gT)
        {
            _tiles = gT;
            _texture = t;
            _rows = numRows;
            _columns = numColumns;
            _chosenRow = new int[(int)levelSize.X, (int)levelSize.Y];
            _chosenColumn = new int[(int)levelSize.X, (int)levelSize.Y];
            _width = 0;
            _height = 0;
            _location = Vector2.Zero;

            for (int i = 0; i < levelSize.X; i++)
            {
                for (int j = 0; j < levelSize.Y; j++)
                {
                    _chosenRow[i, j] = _random.Next(0, _rows);
                    _chosenColumn[i, j] = _random.Next(0, _columns);
                }
            }
        }

        //Calling draw to draw our tiles across the entire screen.
        public void Draw(SpriteBatch spriteBatch, Vector2 locOffset, Vector2 levelSize)
        {
            _width = _texture.Width / _columns;
            _height = _texture.Height / _rows;
            for (int i = 0; i <= levelSize.X; i+=32)
            {
                for (int j = 0; j <= levelSize.Y; j+=32)
                {
                    if (i % _width == 0 && j % _height == 0 && (_tiles.Contains(new Vector2(i / _width, j / _height))))
                    {
                        _location.X = i;
                        _location.Y = j;

                        Rectangle sourceRectangle = new Rectangle(_width * _chosenColumn[i, j], _height * _chosenRow[i, j], _width, _height);

                        spriteBatch.Draw
                        (
                            _texture,
                            new Vector2(_location.X + locOffset.X, _location.Y + locOffset.Y),
                            sourceRectangle,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            (float)LayerDepth.MidgroundTiles / 10f
                        );
                    }
                }
            }
        }

    }
}
