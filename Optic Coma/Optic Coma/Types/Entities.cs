using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Optic_Coma
{
    public class Player
    {
        enum Direction
        {
            E = 0,
            NE = 45,
            N = 90,
            NW = 135,
            W = 180,
            SW = 225,
            S = 270,
            SE = 315,
        }
        float flashAngle = 0f;
        float playerAngle = 0f;
        Vector2 direction;
        public Texture2D Texture { get; set; }
        Vector2 initPosition;
        Vector2 currentPosition;
        Vector2 mouseLoc;
        Texture2D flashLightTexture;

        public Player(Texture2D texture, Vector2 initPos, Texture2D flashlightTexture)
        {
            initPosition = initPos;
            currentPosition = initPos;
            Texture = texture;
            flashLightTexture = flashlightTexture;
        }

        public void Update()
        {
            MouseState curMouse = Mouse.GetState();

            mouseLoc = new Vector2(curMouse.X, curMouse.Y);
            mouseLoc.X = curMouse.X;
            mouseLoc.Y = curMouse.Y;

            direction = mouseLoc - currentPosition;

            //using radians
            flashAngle = (float)(Math.Atan2(direction.Y, direction.X));

            if ((flashAngle < 2 * Math.PI && flashAngle > 7 * Math.PI / 4) 
                ||(flashAngle >= 0 && flashAngle <= Math.PI / 4))
            {
                playerAngle = 0;
            }
            else if (flashAngle > Math.PI / 4 && flashAngle <= 3 * Math.PI  / 4)
            {
                playerAngle = (float)Math.PI / 2;
            }
            else if (flashAngle > 3 * Math.PI / 4 && flashAngle <= 5 * Math.PI / 4)
            {
                playerAngle = (float)Math.PI;
            }
            else if (flashAngle >  5 * Math.PI / 4 && flashAngle <= 7 * Math.PI / 4)
            {
                playerAngle = (float)Math.PI * 2;
            }

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            KeyboardState keyState = Keyboard.GetState();
            /*
            if (keyState.IsKeyDown(Keys.Space) &&
                (currentPosition.X < mouseLoc.X - 3f || currentPosition.X > mouseLoc.X + 3f) &&
                (currentPosition.Y < mouseLoc.Y - 3f || currentPosition.Y > mouseLoc.Y + 3f) &&
                flashAngle != 0 && flashAngle != Math.PI / 2 && flashAngle != -1 * (Math.PI / 2) && flashAngle != Math.PI)
            {
                currentPosition.X += (float)Math.Cos(flashAngle - Math.PI / 2) * 3;
                currentPosition.Y += (float)Math.Sin(flashAngle - Math.PI / 2) * 3;
            }
            else if(keyState.IsKeyDown(Keys.Space) &&
                (currentPosition.X < mouseLoc.X - 3f || currentPosition.X > mouseLoc.X + 3f) &&
                (currentPosition.Y < mouseLoc.Y - 3f || currentPosition.Y > mouseLoc.Y + 3f) && (
                flashAngle != 0 || flashAngle != Math.PI / 2 || flashAngle != (3 * Math.PI / 2) || flashAngle != Math.PI))
            {
                currentPosition.X += (Math.Sign(direction.X)) * 3;
                currentPosition.Y += (Math.Sign(direction.Y)) * 3;
            }
            */
            if (keyState.IsKeyDown(Keys.A))
                currentPosition.X -= 4;
            if (keyState.IsKeyDown(Keys.D))
                currentPosition.X += 4;
            if (keyState.IsKeyDown(Keys.W))
                currentPosition.Y -= 4;
            if (keyState.IsKeyDown(Keys.S))
                currentPosition.Y += 4;

            spriteBatch.Draw
            (
                Texture,
                new Rectangle
                (
                    (int)currentPosition.X,
                    (int)currentPosition.Y,
                    Texture.Width,
                    Texture.Height
                ),
                null,
                Color.White,
                playerAngle,
                new Vector2
                (
                    Texture.Width / 2,
                    Texture.Height / 2
                ),
                SpriteEffects.None,
                ScreenManager.Instance.EntityLayer
            );
            spriteBatch.Draw
            (
                flashLightTexture,
                new Rectangle
                (
                    (int)currentPosition.X,
                    (int)currentPosition.Y,
                    flashLightTexture.Width,
                    flashLightTexture.Height
                ),
                null,
                Color.White,
                flashAngle,
                new Vector2
                (
                    flashLightTexture.Width / 2,
                    flashLightTexture.Height / 2
                ),
                SpriteEffects.None,
                ScreenManager.Instance.FlashlightLayer
            );
        }
    }
    class Enemy
    {
        public float angle = 0;
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
            spriteBatch.Draw
            (
                Texture,
                new Rectangle
                (
                    (int)CurrentPosition.X,
                    (int)CurrentPosition.Y,
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
                SpriteEffects.None,
                ScreenManager.Instance.EntityLayer
            );
        }
    }
}
