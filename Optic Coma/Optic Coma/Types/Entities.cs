using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Optic_Coma
{
    public class Player
    {
        static float flashAngle = 0f;
        float playerAngle = 0f;
        Vector2 facingDirection;
        public Texture2D Texture { get; set; }
        Vector2 initPosition;

        public Vector2 currentPosition;

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

            facingDirection = mouseLoc - currentPosition;

            // using radians
            // measure clockwise from left
            flashAngle = (float)(Math.Atan2(facingDirection.Y, facingDirection.X)) + (float)Math.PI;

            if ((flashAngle > 0 && flashAngle <= Math.PI / 4) || (flashAngle > Math.PI * 7 / 4 && flashAngle <= 2 * Math.PI))
            {
                playerAngle = (float)Math.PI; //Right
            }
            else if (flashAngle > Math.PI / 4 && flashAngle <= Math.PI * 3 / 4)
            {
                playerAngle = -(float)Math.PI / 2; //Down
            }
            else if (flashAngle > Math.PI * 3 / 4 && flashAngle <= Math.PI * 5 / 4)
            {
                playerAngle = 0f; //Left
            }
            else if (flashAngle > Math.PI * 5 / 4 && flashAngle <= Math.PI * 7 / 4)
            {
                playerAngle = (float)Math.PI / 2; //Up
            }

            //Alright, now we calculate walkMult.
            //First up - Are the player and flashlights facing the same direction?
            if(
                (playerAngle == 0 * (Math.PI / 2)) && (flashAngle >= 7 * (Math.PI / 4) || flashAngle < 1 * (Math.PI / 4))
              )
            {

            }
        }
        

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.W))
                currentPosition.Y -= (4 * walkMult((float)Math.PI / 2));
            if (keyState.IsKeyDown(Keys.A))
                currentPosition.X -= (4 * walkMult(0));
            if (keyState.IsKeyDown(Keys.S))
                currentPosition.Y += (4 * walkMult(3 * (float)Math.PI / 2));
            if (keyState.IsKeyDown(Keys.D))
                currentPosition.X += (4 * walkMult((float)Math.PI));      
            spriteBatch.DrawString(font, "baseAngle: " + playerAngle, new Vector2(700, 100), Color.White);
            spriteBatch.DrawString(font, "flashAngle: " + flashAngle, new Vector2(700, 120), Color.White);
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
        public static float walkMult(float dir)
        {
            //First we check if the flash is roughly pointing the same way we are going.
            if (
                ((7 * Math.PI / 4 < dir || dir <= 1 * Math.PI / 4) && (7 * Math.PI / 4 < flashAngle || flashAngle <= 1 * Math.PI / 4)) ||//Both westward?
                ((1 * Math.PI / 4 < dir && dir <= 3 * Math.PI / 4) && (1 * Math.PI / 4 < flashAngle && flashAngle <= 3 * Math.PI / 4)) ||//Both northward?
                ((3 * Math.PI / 4 < dir && dir <= 5 * Math.PI / 4) && (3 * Math.PI / 4 < flashAngle && flashAngle <= 5 * Math.PI / 4)) ||//Both eastward?
                ((5 * Math.PI / 4 < dir && dir <= 7 * Math.PI / 4) && (5 * Math.PI / 4 < flashAngle && flashAngle <= 7 * Math.PI / 4))   //Both southward?
              )
            {
                return 1;
            }
            else if //Then we check if the person is directly backpedalling.
              (
                ((7 * Math.PI / 4 < dir || dir <= 1 * Math.PI / 4) && (3 * Math.PI / 4 < flashAngle && flashAngle <= 5 * Math.PI / 4)) ||//Backpedaling west?
                ((1 * Math.PI / 4 < dir && dir <= 3 * Math.PI / 4) && (5 * Math.PI / 4 < flashAngle && flashAngle <= 7 * Math.PI / 4)) ||//Backpedaling north?
                ((3 * Math.PI / 4 < dir && dir <= 5 * Math.PI / 4) && (7 * Math.PI / 4 < flashAngle || flashAngle <= 1 * Math.PI / 4)) ||//Backpedaling east?
                ((5 * Math.PI / 4 < dir && dir <= 7 * Math.PI / 4) && (1 * Math.PI / 4 < flashAngle && flashAngle <= 3 * Math.PI / 4))   //Backpedaling south?
              )
            {
                return (0.5F);
            }
            else //Must be sidestepping, then.
            {
                return (0.75F);
            }
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
