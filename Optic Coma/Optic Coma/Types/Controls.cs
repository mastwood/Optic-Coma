using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Collections.Generic;
using OpticComa_Types;

namespace OpticComa_Main
{
    public class Button
    {        
        private int rows = 4;
        private int columns = 1;
        private int currentFrame;
        private int totalFrames;
        private MouseState preMouse;

        //This method is called when you make a new button - ala Button button = new Button();
        public Button()
        {
            currentFrame = 0;
            totalFrames = rows * columns;
        }

        //This isn't a method - it's a simplified class - a data structure.
        public struct TextSize
        {
            public float X { get; set; }
            public float Y { get; set; }
        }

        //The Template
        public void Draw(Texture2D texture, SpriteBatch spriteBatch, Action action, Vector2 location, SpriteFont font, string text, Color color)
        {
            TextSize size = new TextSize();
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
                (float)LayerDepth.HUD / 10f
            );

            spriteBatch.DrawString(font, text, textLocation, color, 0f, Vector2.Zero, 1, SpriteEffects.None, (float)LayerDepth.HUD / 10f);
            preMouse = curMouse;
        }
    }
    /// <summary>
    /// Needs to be redone, wow
    /// </summary>
    class DropDown
    {
        public struct DropDownOptions
        {
            public Action[] OptionAction { get; set; }
            public Rectangle[] OptionLocation { get; set; }
        }

        private string[] _contents;
        private MouseState _mouseState;
        private MouseState _prevState;
        private bool _opened = false;
        private int _rows;
        private List<string> _contentsList;
        private Rectangle _sourceRectangleA;
        private Rectangle _sourceRectangleB;
        private Rectangle _destRectangle;
        private Rectangle _mainRect;
        private Texture2D _texture;
        private DropDownOptions _ddO;

        public DropDown(string[] Contents, Texture2D Texture)
        {
            _contentsList = new List<string>();
            _texture = Texture;
            _contents = Contents;
            _opened = false;
            _rows = _contents.Length;
            foreach (string s in _contents)
            {
                _contentsList.Add(s);
            }
            SortByLength(_contentsList);
            _contents = _contentsList.ToArray();
            _ddO.OptionAction = new Action[_rows];
            _ddO.OptionLocation = new Rectangle[_rows];
        }
        private static IEnumerable<string> SortByLength(IEnumerable<string> e)
        {
            var sorted = from s in e
                         orderby s.Length descending
                         select s;
            return sorted;
        }
        public void Update(GameTime gameTime)
        {
            _mouseState = Mouse.GetState();
            if (_mouseState != _prevState 
             && _mouseState.LeftButton == ButtonState.Pressed 
             && _mainRect.Contains(_mouseState.Position))
            {
                if (_opened)
                    _opened = false;
                else if (!_opened)
                    _opened = true;

                _prevState = _mouseState;
            }
            if (_opened)
            {
                for (int i = 0; i < _rows; i++)
                {
                    if (_mouseState != _prevState 
                        && _mouseState.LeftButton == ButtonState.Pressed 
                        && _ddO.OptionLocation[i].Contains(_mouseState.Position))
                    {
                        _ddO.OptionAction[i]();
                    }
                }
            }
            
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 location, Color color)
        {
            if (_opened)
            {
                _sourceRectangleA = new Rectangle(0, 0, _texture.Width, _texture.Height / 2);
                _destRectangle = new Rectangle((int)location.X, (int)location.Y, _texture.Width, _texture.Height / 2);
                _mainRect = _destRectangle;
                spriteBatch.Draw(_texture, _destRectangle, _sourceRectangleA, Color.White);
                _sourceRectangleB = new Rectangle(
                        (int)location.X, (int)location.Y + _texture.Height / 2, _texture.Width, _texture.Height / 2);
                for (int i = 1; i <= _rows; i++)
                {
                    _destRectangle = new Rectangle((int)location.X, (int)location.Y * i * _texture.Height,_texture.Width,_texture.Height/2);
                    spriteBatch.Draw(_texture, _destRectangle, _sourceRectangleB, Color.White);
                    spriteBatch.DrawString(font, _contents[i], new Vector2(_destRectangle.Left - (font.MeasureString(_contents[i]).X / 2), _destRectangle.Top - (font.MeasureString(_contents[i]).Y / 2)), color);
                    _ddO.OptionLocation[i] = _destRectangle;
                }
            }
            else
            {
                _sourceRectangleA = new Rectangle(0, 0, _texture.Width, _texture.Height / 2);
                _destRectangle = new Rectangle((int)location.X, (int)location.Y, _texture.Width, _texture.Height / 2);
                spriteBatch.Draw(_texture, _destRectangle, _sourceRectangleA, Color.White);
            }
        }
    }
}
