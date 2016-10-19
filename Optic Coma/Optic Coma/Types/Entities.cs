﻿using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Optic_Coma
{
    public abstract class Entity
    {
        public Vector2 currentPosition;
        public Entity()
        {

        }
        public virtual void Update()
        {

        }
        // This is called overloading, it is where you have 2 versions of the same method
        // spriteBatch.Draw follows this property, where there are multiple versions of draw
        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
        public static float walkMult(float dir, float angle, float amp, bool useExp)
        {
            //dir, in this method, is equal to the angle in radians the character is moving.
            //angle is the "best" angle - the one that results in fastest movement.
            //amp is in regards to how powerful the slowing effect is.
            //First we check if the flash is roughly pointing the same way we are going.
            if (
                ((7 * Math.PI / 4 < dir || dir <= 1 * Math.PI / 4) && (7 * Math.PI / 4 < angle || angle <= 1 * Math.PI / 4)) ||//Both westward?
                ((1 * Math.PI / 4 < dir && dir <= 3 * Math.PI / 4) && (1 * Math.PI / 4 < angle && angle <= 3 * Math.PI / 4)) ||//Both northward?
                ((3 * Math.PI / 4 < dir && dir <= 5 * Math.PI / 4) && (3 * Math.PI / 4 < angle && angle <= 5 * Math.PI / 4)) ||//Both eastward?
                ((5 * Math.PI / 4 < dir && dir <= 7 * Math.PI / 4) && (5 * Math.PI / 4 < angle && angle <= 7 * Math.PI / 4))   //Both southward?
              )
            {
                if (useExp)
                    return (float)Math.Pow(1, amp);
                else
                    return 1 * amp;

            }
            else if //Then we check if the person is directly backpedalling.
              (
                ((7 * Math.PI / 4 < dir || dir <= 1 * Math.PI / 4) && (3 * Math.PI / 4 < angle && angle <= 5 * Math.PI / 4)) ||//Backpedaling west?
                ((1 * Math.PI / 4 < dir && dir <= 3 * Math.PI / 4) && (5 * Math.PI / 4 < angle && angle <= 7 * Math.PI / 4)) ||//Backpedaling north?
                ((3 * Math.PI / 4 < dir && dir <= 5 * Math.PI / 4) && (7 * Math.PI / 4 < angle || angle <= 1 * Math.PI / 4)) ||//Backpedaling east?
                ((5 * Math.PI / 4 < dir && dir <= 7 * Math.PI / 4) && (1 * Math.PI / 4 < angle && angle <= 3 * Math.PI / 4))   //Backpedaling south?
              )
            {
                if (useExp)
                    return (float)Math.Pow(0.5f, amp);
                else
                    return 0.5f * amp;
            }
            else //Must be sidestepping, then.
            {
                if (useExp)
                    return (float)Math.Pow(0.75f, amp);
                else
                    return 0.75f * amp;
            }
        }
    }


    public class Player : Entity
    {
        public float flashAngle = 0f;
        public float playerAngle = 0f;
        public Vector2 facingDirection;
        public Texture2D Texture { get; set; }
        public Vector2 initPosition;
        public Texture2D LightTexture;
        public static Vector2 currentPosToSend;

        Texture2D flashLightTexture;

        public Player(Texture2D texture, Vector2 initPos, Texture2D flashlightTexture, Texture2D lightTexture)
        {
            LightTexture = lightTexture;
            initPosition = initPos;
            currentPosition = initPos;
            Texture = texture;
            flashLightTexture = flashlightTexture;
        }

        public override void Update()
        {
            currentPosToSend = currentPosition;

        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            
            //spriteBatch.DrawString(font, "enemyAngle: " + Enemy.enemyAngle, new Vector2(700, 100), Color.White);
            //spriteBatch.DrawString(font, "moveAmp: " + Enemy.moveAmp, new Vector2(700, 120), Color.White);
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
            spriteBatch.Draw(
                LightTexture,
                new Rectangle
                (
                    (int)currentPosition.X,
                    (int)currentPosition.Y - LightTexture.Height / 2,
                    LightTexture.Width,
                    LightTexture.Height
                ),
                null,
                Color.White,
                (float)Math.PI + flashAngle,
                new Vector2
                (
                    0,
                    (float)Math.Sin(flashAngle + (float)Math.PI) * LightTexture.Height
                ),
                SpriteEffects.None,
                ScreenManager.Instance.FlashlightLayer
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
                (float)Math.PI + flashAngle,
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
    class Enemy : Entity
    {
        float enemyAngle = 0f;
        public Texture2D Texture { get; set; }

        static Random random;
        int speed;
        int dir;
        float moveAmp;
        public int acceleration = 0;

        public Enemy(Texture2D texture, Vector2 initPosition)
        {
            random = new Random();
            Texture = texture;
            currentPosition = initPosition;
            speed = 2 + acceleration;
            moveAmp = -1;
        }

        public override void Update()
        {
            enemyAngle = (float)(Math.Atan2(Player.currentPosToSend.Y - currentPosition.Y, Player.currentPosToSend.X - currentPosition.X)) + (float)Math.PI;
            //moveAmp += 0.001f;
            moveAmp = 2; //We can toy around with this later.
            dir = random.Next(0, 4);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            dir = random.Next(0, 4);
            if (dir == 0)
                currentPosition.Y -= (4 * walkMult((float)Math.PI / 2, enemyAngle, moveAmp, false));
            else if (dir == 1)
                currentPosition.X -= (4 * walkMult(0, enemyAngle, moveAmp, false));
            else if (dir == 2)
                currentPosition.Y += (4 * walkMult(3 * (float)Math.PI / 2, enemyAngle, moveAmp, false));
            else
                currentPosition.X += (4 * walkMult((float)Math.PI, enemyAngle, moveAmp, false));
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
                enemyAngle,
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
