using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Optic_Coma
{
    /// <summary>
    /// Interface for any object that is managed by the HUD Composite
    /// </summary>
    public interface IHUDObject
    {
        void Draw(SpriteBatch spriteBatch);
        void Update(GameTime gameTime);
        void LoadContent();
    }
    /// <summary>
    /// Composite object which contains every element in the HUD as well as their subelements etc.
    /// See http://www.dofactory.com/net/composite-design-pattern for an explanation
    /// </summary>
    public class HUDComposite : IHUDObject, IEnumerable<IHUDObject>
    {
        private List<IHUDObject> _children = new List<IHUDObject>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        IEnumerator<IHUDObject> IEnumerable<IHUDObject>.GetEnumerator()
        {
            return ((IEnumerable<IHUDObject>)_children).GetEnumerator();
        }

        public void AddChild(IHUDObject c)
        {
            _children.Add(c);
        }
        public void RemoveChild(IHUDObject c)
        {
            _children.Remove(c);
        }
        public IHUDObject GetChild(int index)
        {
            return _children.ElementAt(index);
        }
        /// <summary>
        /// Returns a windows "enumerator" which allows you to perform operations on everything in the HUD
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (IHUDObject obj in _children)
            {
                yield return obj; //yield stops the foreach from breaking until every object has been returned
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(IHUDObject o in _children)
            {
                o.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach(IHUDObject o in _children)
            {
                o.Update(gameTime);
            }
        }

        public void LoadContent()
        {
            foreach(IHUDObject o in _children)
            {
                o.LoadContent();
            }
        }
    }

    /// <summary>
    /// A pause button
    /// </summary>
    public class HUDPauseButton : IHUDObject
    {
        private SpriteSheetProperties _properties;
        private SpriteSheet _sheet;
        private Vector2 _buttonSize;
        private Vector2 _location;
        private Rectangle _bounds;
        private bool _greyed = false;
        private MouseState _prevMouseState;
        private SpriteFont _font;
        /// <summary>
        /// Initializes the button with a texture atlas to use
        /// </summary>
        /// <param name="s"></param>
        public HUDPauseButton()
        {
            
        }
        /// <summary>
        /// Draws the button onto the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw
            (
                _sheet.CurrentTexture(_buttonSize),                                                             //Texture
                null,                                                                                           //Location
                _bounds,                                                                                        //Destination
                null,                                                                                           //SourceRect
                null,                                                                                           //Rotation Origin
                0,                                                                                              //Rotation Angle
                null,                                                                                           //Scale
                Color.White,                                                                                    //Tint
                SpriteEffects.None,                                                                             //Effects
                (float)RenderLayer.HUD                                                                          //Layer
            );
            spriteBatch.DrawString
            (
                _font,
                "Pause",
                new Vector2(_location.X - (_font.MeasureString("Pause").X / 2),
                            _location.Y - (_font.MeasureString("Pause").Y / 2)),
                Color.Black
            );
        }

        
        /// <summary>
        /// Updates the state of the button
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();
            KeyboardState currentKeyBoardState = Keyboard.GetState();
            Vector2 mousePos = new Vector2(currentMouseState.X, currentMouseState.Y);

            if (_bounds.Contains(mousePos) && !_greyed)
            {
                if (currentMouseState == _prevMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    _sheet.SetState("Held");
                }
                else if (currentMouseState != _prevMouseState)
                {
                    _sheet.SetState("Hover");
                }
            }
            else if (!_bounds.Contains(mousePos) && !_greyed)
            {
                _sheet.SetState("Idle");
            }
            else _sheet.SetState("Greyed");

            _prevMouseState = currentMouseState;
        }
        public void SetLocation(Vector2 loc)
        {
            _location = loc;
        }
        public void SetTexture(Texture2D tex)
        {
            _sheet.SetTexture(tex);
        }

        public void LoadContent()
        {
            _location = Vector2.Zero;
            _bounds = new Rectangle((int)_location.X, (int)_location.Y, (int)_buttonSize.X, (int)_buttonSize.Y);
            _buttonSize = new Vector2(128, 64); //TODO: Detect this automatically
            _properties = new SpriteSheetProperties();
            _properties.StateDictionary = new Dictionary<string, Vector2>();
            _properties.StateDictionaryInverse = new Dictionary<Vector2, string>();

            _properties.StateDictionary.Add("Idle", Vector2.Zero);
            _properties.StateDictionary.Add("Hover", new Vector2(0, _buttonSize.Y));
            _properties.StateDictionary.Add("Held", new Vector2(0, 2 * _buttonSize.Y));
            _properties.StateDictionary.Add("Greyed", new Vector2(0, 3 * _buttonSize.Y));

            _properties.StateDictionaryInverse.Add(Vector2.Zero, "Idle");
            _properties.StateDictionaryInverse.Add(new Vector2(0, _buttonSize.Y), "Hover");
            _properties.StateDictionaryInverse.Add(new Vector2(0, 2 * _buttonSize.Y), "Held");
            _properties.StateDictionaryInverse.Add(new Vector2(0, 3 * _buttonSize.Y), "Greyed");


            bool n = false, m = false;
            try
            {
                string line;

                StreamReader file = new StreamReader("mod_overloads.txt");
                while ((line = file.ReadLine()) != null)
                {
                    if (line == "pauseButtonTexture")
                    {
                        n = true;
                    }
                    if (line == "buttonFont")
                    {
                        m = true;
                    }
                    if (n && m)
                    {
                        _sheet = new SpriteSheet
                        (
                            Foundation.StaticContent.Load<Texture2D>("Modded\\Textures\\buttonTexture"),
                            Vector2.Zero,
                            _properties
                        );
                        _font = Foundation.StaticContent.Load<SpriteFont>("Modded\\Fonts\\buttonFont");
                        _sheet.SetState("Idle");
                        return;
                    }
                    else if (n == true && m == false)
                    {
                        _sheet = new SpriteSheet
                        (
                            Foundation.StaticContent.Load<Texture2D>("Modded\\Textures\\buttonTexture"),
                            Vector2.Zero,
                            _properties
                        );
                        _font = Foundation.StaticContent.Load<SpriteFont>("Fonts\\buttonFont");
                        _sheet.SetState("Idle");
                        return;
                    }
                    else if (n == false && m == true)
                    {
                        _sheet = new SpriteSheet
                            (
                                Foundation.StaticContent.Load<Texture2D>("Textures\\buttonTexture"),
                                Vector2.Zero,
                                _properties
                            );
                        _font = Foundation.StaticContent.Load<SpriteFont>("Modded\\Fonts\\buttonFont");
                        _sheet.SetState("Idle");
                        return;
                    }
                    else if (m == false && n == false)
                    {
                        _sheet = new SpriteSheet
                            (
                                Foundation.StaticContent.Load<Texture2D>("Textures\\buttonTexture"),
                                Vector2.Zero,
                                _properties
                            );
                        _font = Foundation.StaticContent.Load<SpriteFont>("Fonts\\buttonFont");
                        _sheet.SetState("Idle");
                        return;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                _sheet = new SpriteSheet
                (
                    Foundation.StaticContent.Load<Texture2D>("Textures\\buttonTexture"),
                    Vector2.Zero,
                    _properties
                );
                _font = Foundation.StaticContent.Load<SpriteFont>("Fonts\\buttonFont");
                _sheet.SetState("Idle");
                return;
            }
            
        }
    }
}
