using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Optic_Coma
{
    public class TileSystem
    {
        int width, height;
        int rows, columns;
        int[,] chosenRow, chosenColumn;
        List<Vector2> goodTiles = new List<Vector2>();
        List<Vector2> borderTiles = new List<Vector2>();
        Random random = new Random();
        Vector2 location;
        Texture2D texture;
        //This method is called when you make a new TileSystem.
        public TileSystem(Texture2D borderTexture, int numRows, int numColumns, int level, Vector2 levelSize)
        {
            texture = borderTexture;
            rows = numRows;           
            columns = numColumns;
            chosenRow = new int[(int)levelSize.X, (int)levelSize.Y];
            chosenColumn = new int[(int)levelSize.X, (int)levelSize.Y];
            width = 0;
            height = 0;
            location = Vector2.Zero;

            for (int i = 0; i < levelSize.X; i++)
            {
                for (int j = 0; j < levelSize.Y; j++)
                {
                    chosenRow[i, j] = random.Next(0, rows);
                    chosenColumn[i, j] = random.Next(0, columns);
                }
            }
            //First, I draw tiles around the level border.
            //top
            for (int i = 0; i < Math.Floor(levelSize.X / (borderTexture.Width / columns)); i++)
            {
                borderTiles.Add(new Vector2(i, 0));
            }
            //bottom
            for (int i = 0; i < Math.Floor(levelSize.X / (borderTexture.Width / columns)); i++)
            {
                borderTiles.Add(new Vector2(i, (levelSize.Y / (borderTexture.Height / rows)) - 1));
            }
            //left 
            for (int j = 0; j < Math.Floor(levelSize.Y / (borderTexture.Height / rows)); j++)
            {
                borderTiles.Add(new Vector2(0, j));
            }
            //right
            for (int j = 0; j < Math.Floor(levelSize.Y / (borderTexture.Height / rows)); j++)
            {
                borderTiles.Add(new Vector2(((levelSize.X / (borderTexture.Width / columns)) - 1), j));
            }
            //Each level will have a different layout of goodtiles.
            if (level == 1)
            {
                for (int i = 10; i < 30; i++)
                {
                    for (int j = 3; j < 5; j++)
                    {
                        goodTiles.Add(new Vector2(i, j));
                    }
                }

                for (int i = 5; i < 30; i++)
                {
                    for (int j = 5; j < 10; j++)
                    {
                        goodTiles.Add(new Vector2(i, j));
                    }
                }
            }
        }

        //Calling draw to draw our tiles across the entire screen.
        public void Draw(SpriteBatch spriteBatch, Vector2 locOffset, Vector2 LevelSize)
        {
            width = texture.Width / columns;
            height = texture.Height / rows;
            for (int i = 0; i < LevelSize.X; i++)
            {
                for (int j = 0; j < LevelSize.Y; j++)
                {
                    if (i % width == 0 && j % height == 0 && (goodTiles.Contains(new Vector2(i / width, j / height))))
                    {
                        location.X = i;
                        location.Y = j;

                        Rectangle sourceRectangle = new Rectangle(width * chosenColumn[i, j], height * chosenRow[i, j], width, height);

                        spriteBatch.Draw
                        (
                            texture,
                            new Vector2(location.X + locOffset.X, location.Y + locOffset.Y),
                            sourceRectangle,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            ScreenManager.Instance.TileLayer
                        );
                    }
                    else if (i % width == 0 && j % height == 0 && (borderTiles.Contains(new Vector2(i / width, j / height))))
                    {
                        location.X = i;
                        location.Y = j;

                        Rectangle sourceRectangle = new Rectangle(width * chosenColumn[i, j], height * chosenRow[i, j], width, height);

                        spriteBatch.Draw
                        (
                            texture,
                            new Vector2(location.X + locOffset.X, location.Y + locOffset.Y),
                            sourceRectangle,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            ScreenManager.Instance.TileLayer
                        );
                    }
                }
            }
        }
    }
}
