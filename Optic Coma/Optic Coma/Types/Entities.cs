using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;

namespace Optic_Coma
{
    public abstract class Entity
    {
        public Vector2 CurrentPosition;
        public static Vector2 CenterScreen = new Vector2(ScreenManager.Instance.Dimensions.X / 2, ScreenManager.Instance.Dimensions.Y / 2);
        public float Angle;
        public Hull Hull;

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
        public static float WalkMult(float dir, float angle, float amp, bool useExp)
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
        public float FlashAngle = 0f;
        public float PlayerAngle = 0f;
        public Vector2 FacingDirection;
        public Texture2D Texture { get; set; }
        
        public Texture2D LightTexture;
        public Spotlight FlashLight;
        private Texture2D _flashLightTexture;

        public Player(Texture2D texture, Vector2 initPos, Texture2D flashlightTexture, Texture2D lightTexture)
        {
            LightTexture = lightTexture;
            CurrentPosition = initPos;
            Texture = texture;
            _flashLightTexture = flashlightTexture;
            FlashLight = new Spotlight()
            {
                Position = CenterScreen,
                Enabled = true,
                CastsShadows = true,
                Scale = new Vector2(900, 900),
                Color = Color.White,
                Intensity = 2,
                ShadowType = ShadowType.Occluded,
            };
            Foundation.LightingEngine.Lights.Add(FlashLight);
        }

        public override void Update()
        {
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
                    (int)CenterScreen.X,
                    (int)CenterScreen.Y,
                    Texture.Width,
                    Texture.Height
                ),
                null,
                Color.White,
                PlayerAngle,
                new Vector2
                (
                    Texture.Width / 2,
                    Texture.Height / 2
                ),
                SpriteEffects.None,
                ScreenManager.Instance.EntityLayer
            );
            /*
            spriteBatch.Draw(
                LightTexture,
                new Rectangle
                (
                    (int)centerScreen.X,
                    (int)centerScreen.Y ,
                    LightTexture.Width,
                    LightTexture.Height
                ),
                null,
                Color.White,
                (float)Math.PI + flashAngle,
                new Vector2
                (
                    0,
                    LightTexture.Height / 2
                ),
                SpriteEffects.None,
                ScreenManager.Instance.FlashlightLayer
            );
            */
            spriteBatch.Draw
            (
                _flashLightTexture,
                new Rectangle
                (
                    (int)CenterScreen.X,
                    (int)CenterScreen.Y,
                    _flashLightTexture.Width,
                    _flashLightTexture.Height
                ),
                null,
                Color.White,
                (float)Math.PI + FlashAngle,
                new Vector2
                (
                    _flashLightTexture.Width / 2,
                    _flashLightTexture.Height / 2
                ),
                SpriteEffects.None,
                ScreenManager.Instance.FlashlightLayer
            );
        }  
    }

    public enum EnemyType
    {
        Jiggler,
        Wavey
    }

    public class Enemy : Entity
    {
        public EnemyType EnemyMode;
        ///     Example use:
        ///     if(EnemyMode == EnemyType.Wavey)
        ///     {
        ///          DOSTUFF()
        ///     }

        public float EnemyAngle = 0f;
        public Texture2D Texture { get; set; }
        private static Random _random;
        private int _speed;
        private int _dir;
        private float _moveAmp;
        public int Acceleration = 0;
        public bool Spawned = true;

        public Enemy(Texture2D texture, Vector2 initPosition)
        {
            _random = new Random();
            Texture = texture;
            CurrentPosition = initPosition;
            _speed = 4 + Acceleration;
            _moveAmp = -1;
            Hull = Hull.CreateRectangle(CurrentPosition, new Vector2(texture.Width, texture.Height), Angle, new Vector2(texture.Width/2, texture.Height/2));
            Hull.Origin = new Vector2(CurrentPosition.X / 2, CurrentPosition.Y / 2);
            
            Hull.Enabled = true;
        }
        public void Initialize()
        {
            Spawned = true;
            //other stuff?
        }
        public override void Update()
        {
            if (Spawned)
            {
                EnemyAngle = (float)(Math.Atan2(CenterScreen.Y - CurrentPosition.Y,
                    CenterScreen.X - CurrentPosition.X)) + (float)Math.PI;
                //moveAmp += 0.001f;
                _moveAmp = 4; //We can toy around with this later.
                _dir = _random.Next(0, 4);
                Angle = EnemyAngle;
            }
        }
        public void UpdateHull()
        {
            if (Spawned)
            {
                Hull.Rotation = Angle;
                Hull.Position = Hull.Origin;
                Foundation.LightingEngine.Hulls.Add(Hull);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Spawned)
            {
                _dir = _random.Next(0, 4);
                if (_dir == 0)
                    CurrentPosition.Y -= (4 * WalkMult((float)Math.PI / 2, EnemyAngle, _moveAmp, false));
                else if (_dir == 1)
                    CurrentPosition.X -= (4 * WalkMult(0, EnemyAngle, _moveAmp, false));
                else if (_dir == 2)
                    CurrentPosition.Y += (4 * WalkMult(3 * (float)Math.PI / 2, EnemyAngle, _moveAmp, false));
                else
                    CurrentPosition.X += (4 * WalkMult((float)Math.PI, EnemyAngle, _moveAmp, false));

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
                    EnemyAngle,
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
}
