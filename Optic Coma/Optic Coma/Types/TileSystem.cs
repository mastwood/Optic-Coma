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

        Random random = new Random();
        Vector2 location;
        //This method is called when you make a new TileSystem.
        public TileSystem(int numRows, int numColumns)
        {
            rows = numRows;
            columns = numColumns;
            chosenRow = new int[(int)ScreenManager.Instance.Dimensions.X, (int)ScreenManager.Instance.Dimensions.Y];
            chosenColumn = new int[(int)ScreenManager.Instance.Dimensions.X, (int)ScreenManager.Instance.Dimensions.Y];
            width = 0;
            height = 0;
            location = Vector2.Zero;

            for (int i = 0; i < ScreenManager.Instance.Dimensions.X; i++)
            {
                for (int j = 0; j < ScreenManager.Instance.Dimensions.Y; j++)
                {
                    chosenRow[i, j] = random.Next(0, rows);
                    chosenColumn[i, j] = random.Next(0, columns);
                }
            }
        }

        //Calling draw to draw our tiles across the entire screen.
        public void Draw(Texture2D texture, SpriteBatch spriteBatch, List<Vector2> goodTiles)
        {
            width = texture.Width / columns;
            height = texture.Height / rows;
            for (int i = 0; i < ScreenManager.Instance.Dimensions.X; i++)
            {
                for (int j = 0; j < ScreenManager.Instance.Dimensions.Y; j++)
                {
                    if (i % width == 0 && j % height == 0 && (goodTiles.Contains(new Vector2(i / width, j / height))))
                    {
                        location.X = i;
                        location.Y = j;

                        Rectangle sourceRectangle = new Rectangle(width * chosenColumn[i, j], height * chosenRow[i, j], width, height);

                        spriteBatch.Draw
                        (
                            texture,
                            location,
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
