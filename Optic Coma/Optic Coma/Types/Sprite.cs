using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Serialization;

namespace Optic_Coma
{
    public class SpriteSheet
    {
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 location)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, int frame)
        {

        }
        public virtual void Update()
        {

        }
        public virtual void Update(GameTime gameTime)
        {

        }
    }

    public enum EntitySpriteMode
    {
        Idle,
        Walking,
        Running,
        Hit
    }

    public class EntitySprite : SpriteSheet
    {
        public EntitySpriteMode Mode;
        public Vector2 TileSize { get; set; }
        public Vector2 TotalSize { get; set; }
        public Texture2D SpriteSheet;

        [XmlIgnore]
        public int CurrentColumn = 0;
        [XmlIgnore]
        public int CurrentRow = 0;
        [XmlIgnore]
        public Rectangle SourceRectangle;

        public EntitySprite()
        {
            Mode = EntitySpriteMode.Idle;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            SourceRectangle = new Rectangle(
                (int)TileSize.X * CurrentColumn,
                (int)TileSize.Y * CurrentRow,
                (int)TileSize.X,
                (int)TileSize.Y);
            Rectangle DestinationRectangle = new Rectangle(
                (int)location.X,
                (int)location.Y,
                (int)TileSize.X,
                (int)TileSize.Y);
            spriteBatch.Draw(SpriteSheet, DestinationRectangle, SourceRectangle, Color.White, 0, new Vector2(0,0), SpriteEffects.None, Layer)

            base.Draw(spriteBatch);
        }
        public override void Update()
        {
            base.Update();
        }
    }

    /// <summary>
    /// A sprite which continuously cycles through a spritesheet
    /// </summary>
    public class AnimatedSprite : SpriteSheet
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int _currentFrame;
        private int _totalFrames;

        public AnimatedSprite(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            _currentFrame = 0;
            _totalFrames = Rows * Columns;
        }

        public override void Update()
        {
            _currentFrame++;
            if (_currentFrame == _totalFrames)
                _currentFrame = 0;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)_currentFrame / (float)Columns);
            int column = _currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
        }

    }
}
