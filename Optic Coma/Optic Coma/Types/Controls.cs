using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Collections.Generic;

namespace Optic_Coma
{
    class Button
    {        
        int rows = 4;
        int columns = 1;
        private int currentFrame;
        private int totalFrames;
        MouseState preMouse;

        //This method is called when you make a new button - ala Button button = new Button();
        public Button()
        {
            currentFrame = 0;
            totalFrames = rows * columns;
        }

        //This isn't a method - it's a simplified class - a data structure.
        public struct textSize
        {
            public float X;
            public float Y;
        }

        //The Template
        public void Draw(Texture2D texture, SpriteBatch spriteBatch, Action action, Vector2 location, SpriteFont font, string text, Color color)
        {
            textSize size;
            size.X = font.MeasureString(text).X;
            size.Y = font.MeasureString(text).Y;

            int width = texture.Width / columns;
            int height = texture.Height / rows;

            //Magic code
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
                action();
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
            else
            {
                currentFrame = 0;
            }

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
                ScreenManager.Instance.ButtonLayer
            );

            spriteBatch.DrawString(font, text, textLocation, color, 0f, Vector2.Zero, 1, SpriteEffects.None, ScreenManager.Instance.ButtonTextLayer);
            preMouse = curMouse;
        }
    }
    class DropDown
    {
        public struct DropDownOptions
        {
            public Action[] optionAction;
            public Rectangle[] optionLocation;
        }

        string[] contents;
        MouseState mouseState;
        MouseState prevState;
        bool opened = false;
        int rows;
        List<string> contentsList;
        Rectangle sourceRectangleA;
        Rectangle sourceRectangleB;
        Rectangle destRectangle;
        Rectangle mainRect;
        Texture2D texture;
        DropDownOptions ddO;
        public DropDown(string[] Contents, Texture2D Texture)
        {
            contentsList = new List<string>();
            texture = Texture;
            contents = Contents;
            opened = false;
            rows = contents.Length;
            foreach (string s in contents)
            {
                contentsList.Add(s);
            }
            SortByLength(contentsList);
            contents = contentsList.ToArray();
            ddO.optionAction = new Action[rows];
            ddO.optionLocation = new Rectangle[rows];
        }
        static IEnumerable<string> SortByLength(IEnumerable<string> e)
        {
            var sorted = from s in e
                         orderby s.Length descending
                         select s;
            return sorted;
        }
        public void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            if (mouseState != prevState 
             && mouseState.LeftButton == ButtonState.Pressed 
             && mainRect.Contains(mouseState.Position))
            {
                if (opened)
                    opened = false;
                else if (!opened)
                    opened = true;

                prevState = mouseState;
            }
            if (opened)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (mouseState != prevState 
                        && mouseState.LeftButton == ButtonState.Pressed 
                        && ddO.optionLocation[i].Contains(mouseState.Position))
                    {
                        ddO.optionAction[i]();
                    }
                }
            }
            
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 location, Color color)
        {
            if (opened)
            {
                sourceRectangleA = new Rectangle(0, 0, texture.Width, texture.Height / 2);
                destRectangle = new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height / 2);
                mainRect = destRectangle;
                spriteBatch.Draw(texture, destRectangle, sourceRectangleA, Color.White);
                sourceRectangleB = new Rectangle(
                        (int)location.X, (int)location.Y + texture.Height / 2, texture.Width, texture.Height / 2);
                for (int i = 1; i <= rows; i++)
                {
                    destRectangle = new Rectangle((int)location.X, (int)location.Y * i * texture.Height,texture.Width,texture.Height/2);
                    spriteBatch.Draw(texture, destRectangle, sourceRectangleB, Color.White);
                    spriteBatch.DrawString(font, contents[i], new Vector2(destRectangle.Left - (font.MeasureString(contents[i]).X / 2), destRectangle.Top - (font.MeasureString(contents[i]).Y / 2)), color);
                    ddO.optionLocation[i] = destRectangle;
                }
            }
            else
            {
                sourceRectangleA = new Rectangle(0, 0, texture.Width, texture.Height / 2);
                destRectangle = new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height / 2);
                spriteBatch.Draw(texture, destRectangle, sourceRectangleA, Color.White);
            }
        }
    }
}
