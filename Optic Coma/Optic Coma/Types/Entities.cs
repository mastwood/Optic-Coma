using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Optic_Coma
{
    public class Player
    {
        float flashAngle = 0f;
        float playerAngle = 0f;
        Vector2 direction;
        public Texture2D Texture { get; set; }
        Vector2 initPosition;

        public Vector2 currentPosition;

        Vector2 mouseLoc;
        Texture2D flashLightTexture;
        float flashRadians = 0f;

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
            //Clockwise from left
            flashAngle = (float)(Math.Atan2(direction.Y, direction.X));
            flashRadians = flashAngle + (float)Math.PI;
            if ((flashRadians > 0 && flashRadians <= Math.PI / 4) || (flashRadians > Math.PI * 7 / 4 && flashRadians <= 2 * Math.PI))
            {
                playerAngle = (float)Math.PI;
            } else if (flashRadians > Math.PI / 4 && flashRadians <= Math.PI * 3 / 4)
            {
                playerAngle = -(float)Math.PI / 2;
            } else if (flashRadians > Math.PI * 3 / 4 && flashRadians <= Math.PI * 5 / 4)
            {
                playerAngle = 0f;
            } else if (flashRadians > Math.PI * 5 / 4 && flashRadians <= Math.PI * 7 / 4)
            {
                playerAngle = (float)Math.PI / 2;
            }

        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            KeyboardState keyState = Keyboard.GetState();
            
            if (keyState.IsKeyDown(Keys.A))
                currentPosition.X -= 4;
            if (keyState.IsKeyDown(Keys.D))
                currentPosition.X += 4;
            if (keyState.IsKeyDown(Keys.W))
                currentPosition.Y -= 4;
            if (keyState.IsKeyDown(Keys.S))
                currentPosition.Y += 4;
            spriteBatch.DrawString(font, "baseAngle: " + (flashAngle * (180 / Math.PI)), new Vector2(700, 100), Color.White);
            spriteBatch.DrawString(font, "flashAngle: " + (flashRadians * (180 / Math.PI)), new Vector2(700, 120), Color.White);
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
