using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Optic_Coma
{
    /// <summary>
    /// Contains dictionary associating a spritesheet's coordinates with a name
    /// </summary>
    public struct SpriteSheetProperties
    {
        public Dictionary<string, Vector2> StateDictionary;
        public Dictionary<Vector2, string> StateDictionaryInverse;
    }
    public class SpriteSheet
    {
        private Texture2D _texture;
        private Vector2 _currentCoords;
        private SpriteSheetProperties _properties;
        /// <summary>
        /// Constructor for a spritesheet
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="loc"></param>
        /// <param name="props"></param>
        public SpriteSheet(Texture2D tex, Vector2 loc, SpriteSheetProperties props)
        {
            _texture = tex;
            _currentCoords = loc;
            _properties = props;
        }
        /// <summary>
        /// Sets the texture for the given object
        /// </summary>
        public void SetTexture(Texture2D t)
        {
            _texture = t;
        }
        /// <summary>
        /// Gets the texture of the given object
        /// </summary>
        /// <returns>Texture2D</returns>
        public Texture2D GetTexture()
        {
            return _texture;
        }
        /// <summary>
        /// Sets the state of the spritesheet
        /// </summary>
        /// <param name="State Name"></param>
        public void SetState(string stateName)
        {
            _properties.StateDictionary.TryGetValue(stateName, out _currentCoords);
        }
        /// <summary>
        /// Gets the current state of the spritesheet
        /// </summary>
        /// <returns></returns>
        public string GetState(SpriteSheetProperties props)
        {
            string s;
            _properties.StateDictionaryInverse.TryGetValue(_currentCoords, out s);
            return s;
        }
        /// <summary>
        /// Takes in a frame number instead of a string in order to make animations easier to deal with
        /// </summary>
        /// <param name="Frame"></param>
        public void SetState(ushort frame)
        {
            _properties.StateDictionary.TryGetValue(frame.ToString(), out _currentCoords);
        }
        /// <summary>
        /// Returns the texture that the spritesheet is currently focused on
        /// </summary>
        /// <param name="Texture Size"></param>
        /// <returns>Texture2D</returns>
        public Texture2D CurrentTexture(Vector2 size)
        {
            return Crop(_texture, new Rectangle((int)_currentCoords.X, (int)_currentCoords.Y, (int)size.X, (int)size.Y));
        }
        /// <summary>
        /// Crops a spritesheet in order to focus on a specific texture
        /// </summary>
        /// <param name="image"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public Texture2D Crop(Texture2D image, Rectangle source)
        {
            var graphics = image.GraphicsDevice;
            var returnable = new RenderTarget2D(graphics, source.Width, source.Height);
            var sb = new SpriteBatch(graphics);

            graphics.SetRenderTarget(returnable); // draw to image
            graphics.Clear(new Color(0, 0, 0, 0));

            sb.Begin();
            sb.Draw(image, Vector2.Zero, source, Color.White);
            sb.End();

            graphics.SetRenderTarget(null); // set back to main window

            return returnable;
        }
    }
}
