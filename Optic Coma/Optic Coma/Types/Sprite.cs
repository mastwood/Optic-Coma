using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Serialization;
using OpticComa_Types;

namespace OpticComa_Main
{
    public class SpriteSheet
    {
        public virtual void Draw(SpriteBatch spriteBatch, float rotation)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, int frame)
        {

        }
        public virtual void Update()
        {

        }
        public virtual void Update(Vector2 location)
        {

        }
    }

    

    public class EntitySprite : SpriteSheet
    {
        public EntitySpriteMode Mode { get; set; }
        public Vector2 TileSize { get; set; }
        public Vector2 TotalSize { get; set; }
        public Texture2D SpriteSheetTex;

        [XmlIgnore]
        protected int CurrentColumn = 0;
        [XmlIgnore]
        protected int CurrentRow = 0;
        [XmlIgnore]
        protected Rectangle SourceRectangle;
        [XmlIgnore]
        protected Rectangle DestinationRectangle;

        protected Vector2 Origin;

        protected int rowCounter = 0;

        public EntitySprite()
        {
            Mode = EntitySpriteMode.Idle;
            Origin = new Vector2(TileSize.X / 2, TileSize.Y / 2);
        }
        public override void Draw(SpriteBatch spriteBatch, float rotation)
        {
        }
        public override void Update(Vector2 location)
        {
            SourceRectangle = new Rectangle(
                (int)TileSize.X * CurrentColumn,
                (int)TileSize.Y * CurrentRow,
                (int)TileSize.X,
                (int)TileSize.Y);
            DestinationRectangle = new Rectangle(
                (int)location.X,
                (int)location.Y,
                (int)TileSize.X,
                (int)TileSize.Y);

            if (Mode == EntitySpriteMode.Idle)
            {
                CurrentColumn = 0;
            }
            else if (Mode == EntitySpriteMode.Walking)
            {
                CurrentColumn = 1;
            }
            else if (Mode == EntitySpriteMode.Running)
            {
                CurrentColumn = 2;
            }
            else if (Mode == EntitySpriteMode.Hit)
            {
                CurrentColumn = 3;
            }
            if (rowCounter == 4)
            {
                if (CurrentRow > TotalSize.Y / TileSize.Y)
                    CurrentRow = 0;
                else
                    CurrentRow += 1;

                rowCounter = 0;
            }
            rowCounter++;
        }
    }
    public class EnemySprite : EntitySprite
    {
        public override void Draw(SpriteBatch spriteBatch, float rotation)
        {
            spriteBatch.Draw(SpriteSheetTex, DestinationRectangle, SourceRectangle, Color.White, rotation, Vector2.Zero, SpriteEffects.None, (float)LayerDepth.Enemy / 10f);
        }
        public override void Update(Vector2 location)
        {
            base.Update(location);
        }
    }
    public class PlayerSprite : EntitySprite
    {
        public override void Draw(SpriteBatch spriteBatch, float rotation)
        {
            spriteBatch.Draw(SpriteSheetTex, DestinationRectangle, SourceRectangle, Color.White, rotation, Vector2.Zero, SpriteEffects.None, (float)LayerDepth.Player / 10f);
        }
        public override void Update(Vector2 location)
        {
            base.Update(location);
        }
    }
    public class NPCSprite : EntitySprite
    {
        public override void Draw(SpriteBatch spriteBatch, float rotation)
        {
            spriteBatch.Draw(SpriteSheetTex, DestinationRectangle, SourceRectangle, Color.White, rotation, Vector2.Zero, SpriteEffects.None, (float)LayerDepth.Enemy / 10f);
        }
        public override void Update(Vector2 location)
        {
            base.Update(location);
        }
    }
}
