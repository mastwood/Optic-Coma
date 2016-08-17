using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Optic_Coma
{
    class Button
    {
        int rows = 4;
        int columns = 1;
        private int currentFrame;
        private int totalFrames;
        MouseState preMouse;

        public Button()
        {
            currentFrame = 0;
            totalFrames = rows * columns;
        }
        public struct textSize
        {
            public float X;
            public float Y;
        }
        public void Draw(Texture2D texture, SpriteBatch spriteBatch, Action changeScreen, Vector2 location, SpriteFont font, string text, Color color)
        {
            textSize size;
            size.X = font.MeasureString(text).X;
            size.Y = font.MeasureString(text).Y;

            int width = texture.Width / columns;
            int height = texture.Height / rows;
            int row = (currentFrame / columns);
            int column = currentFrame % columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);
            Rectangle area = new Rectangle((int)location.X, (int)location.Y, width, height);

            Vector2 textLocation = new Vector2(location.X + (width / 2) - (size.X / 2), location.Y + (height / 2) - (size.Y / 2));

            MouseState curMouse = Mouse.GetState();

            if (area.Contains(curMouse.Position) &&
                curMouse != preMouse && curMouse.LeftButton == ButtonState.Pressed)
            {
                changeScreen();
                currentFrame = 2;
            }
            else if (area.Contains(curMouse.Position) &&
                curMouse == preMouse && curMouse.LeftButton == ButtonState.Pressed)
            {
                currentFrame = 2;
            }
            else if (area.Contains(curMouse.Position) && curMouse.LeftButton == ButtonState.Released)
            {
                currentFrame = 1;
            }
            else { currentFrame = 0; }

            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);

            spriteBatch.DrawString(font, text, textLocation, color);
            preMouse = curMouse;
        }
    }
}
