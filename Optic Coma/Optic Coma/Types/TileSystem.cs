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
        List<Vector2> tiles = new List<Vector2>();
        Random random = new Random();
        Vector2 location;
        Texture2D texture;

        //This method is called when you make a new TileSystem.
        public TileSystem(Texture2D t, int numRows, int numColumns, int level, Vector2 levelSize, List<Vector2> gT)
        {
            tiles = gT;
            texture = t;
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
        }

        //Calling draw to draw our tiles across the entire screen.
        public void Draw(SpriteBatch spriteBatch, Vector2 locOffset, Vector2 LevelSize)
        {
            width = texture.Width / columns;
            height = texture.Height / rows;
            for (int i = 0; i <= LevelSize.X; i+=32)
            {
                for (int j = 0; j <= LevelSize.Y; j+=32)
                {
                    if (i % width == 0 && j % height == 0 && (tiles.Contains(new Vector2(i / width, j / height))))
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
