using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Optic_Coma
{
    public class Player
    {
        float angle = 0f;

        public Texture2D Texture { get; set; }

        public Player(Texture2D texture)
        {
            Texture = texture;
        }

        public void Update(Vector2 pointerPosition)
        {

            MouseState curMouse = Mouse.GetState();
            Vector2 mouseLoc = new Vector2(curMouse.X, curMouse.Y);
            Vector2 direction = mouseLoc - pointerPosition;

            //THIS IS IN RADIANS!!!!!!!!!!!!!!!!!!!!!!!!!!
            //angle = ((float)(Math.Atan2(direction.Y, direction.X)) + ((float)Math.PI / 2));
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 defaultLocation,
                         Vector2 pointerPosition)
        {
            spriteBatch.Draw
            (
                Texture,
                new Rectangle
                (
                    (int)pointerPosition.X,
                    (int)pointerPosition.Y,
                    Texture.Width,
                    Texture.Height
                ),
                null,
                Color.White,
                angle,
                new Vector2
                (
                    Texture.Width / 2,
                    Texture.Height / 2
                ),
                SpriteEffects.None, 0
            );
        }
    }
    class Enemy
    {
        public Texture2D Texture { get; set; }
        public Vector2 CurrentPosition;
        public Vector2 InitPosition;
        Random random = new Random();
        int speed;
        public int acceleration = 0;

        public Enemy(Texture2D texture, Vector2 initPosition)
        {
            Texture = texture;
            InitPosition = initPosition;
            CurrentPosition = InitPosition;
            speed = 2 + acceleration;
        }

        public void Update()
        {

            CurrentPosition.Y += speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, CurrentPosition);
        }

    }

}
