using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Optic_Coma
{
    class sTitleFlicker
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame = 0;
        private int totalFrames;
        float timeSinceLastFrameA = 0f;
        float timeSinceLastFrameB = 0f;
        Random maxTime = new Random();
        float maxTimeX = 2;
        //in the update method
        
        public sTitleFlicker(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
        }

        public void Update(GameTime gameTime)
        {
            timeSinceLastFrameA += (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceLastFrameB += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentFrame == 0 && timeSinceLastFrameA == maxTimeX)
            {
                timeSinceLastFrameA = 0;
                currentFrame++;
            }
            else if(currentFrame == 1 && timeSinceLastFrameB - maxTimeX >= 1)
            {
                currentFrame = 0;
                timeSinceLastFrameB = 0;
                maxTimeX = (maxTime.Next(10, 400)) / 100;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw
            (
                Texture,
                new Vector2(destinationRectangle.Left, destinationRectangle.Top),
                sourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                ScreenManager.Instance.BGLayer - 0.01f
            );
        }
    }
}
