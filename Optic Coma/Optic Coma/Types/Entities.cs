using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using System.Xml.Serialization;
namespace Optic_Coma
{
    public enum EnemyType
    {
        Jiggler,
        Wavey
    }
    public enum NPCMode
    {
        ScriptedMovement,
        Stationary,
        Dead
    }


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
        /// <summary>
        /// Allows for serialization of arrays -- Enemyspawner
        /// </summary>
        /// <returns></returns>
        public static XmlSerializer EnemySpawner_CreateOverrider()
        {
            // Creating XmlAttributeOverrides and XmlAttributes objects.
            XmlAttributeOverrides xOver = new XmlAttributeOverrides();
            XmlAttributes xAttrs = new XmlAttributes();

            // Add an override for the XmlArray.    
            XmlArrayAttribute xArray = new XmlArrayAttribute("EnemyWave");
            xAttrs.XmlArray = xArray;
            xOver.Add(typeof(EnemySpawnerProperties), "EnemyWaveArray", xAttrs);

            // Create the XmlSerializer and return it.
            return new XmlSerializer(typeof(EnemySpawnerProperties), xOver);
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
        
        public Spotlight FlashLight;
        private Texture2D flashLightTexture;

        public Player(Texture2D texture, Vector2 initPos, Texture2D flashlightTexture)
        {
            if (flashLightTexture != null)
            {
                CurrentPosition = initPos;
                Texture = texture;
                flashLightTexture = flashlightTexture;
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
            else if(flashLightTexture == null)
            {
                CurrentPosition = initPos;
                Texture = TextureCutter.Cut(texture, new Vector2(0, 0), 32);
                flashLightTexture = TextureCutter.Cut(texture, new Vector2(1, 0), 32);
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
        }
        public override void Update()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
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
                (float)LayerDepth.Player
            );
            spriteBatch.Draw
            (
                flashLightTexture,
                new Rectangle
                (
                    (int)CenterScreen.X,
                    (int)CenterScreen.Y,
                    flashLightTexture.Width,
                    flashLightTexture.Height
                ),
                null,
                Color.White,
                (float)Math.PI + FlashAngle,
                new Vector2
                (
                    flashLightTexture.Width / 2,
                    flashLightTexture.Height / 2
                ),
                SpriteEffects.None,
                (float)LayerDepth.Flashlight
            );
        }  
    }

    /// <summary>
    /// Contains the properties of an enemy that will be loaded or saved alongside a level
    /// </summary>
    public class EnemyProperties
    {
        public EnemyType Mode;
        public Texture2D Texture;
    }
    public class NPCProperties
    {
        public NPCMode InitMode;
        public Vector2 InitLocation;
        public Texture2D Texture;
    }
    public class NPC : Entity
    {
        public NPCProperties Properties;

        public NPC()
        {
        }

    }
    public class Enemy : Entity
    {
        public EnemyProperties Properties;
        public float EnemyAngle = 0f;
        private static Random random;
        private int speed;
        private int dir;
        private float moveAmp;
        public int Acceleration = 0;
        public bool Spawned = true;

        public Enemy(Texture2D texture, Vector2 initPosition, EnemyType eType)
        {
            random = new Random();
            Properties.Texture = texture;
            Properties.Mode = eType;
            CurrentPosition = initPosition;
            speed = 4 + Acceleration;
            moveAmp = 0;
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
                
                Angle = EnemyAngle;
                if (Properties.Mode == EnemyType.Jiggler)
                {
                    EnemyAngle = (float)(Math.Atan2(CenterScreen.Y - CurrentPosition.Y,
                             CenterScreen.X - CurrentPosition.X)) + (float)Math.PI;
                    //moveAmp += 0.001f;
                    moveAmp = 4; //We can toy around with this later.
                    dir = random.Next(0, 4);
                }
                else if (Properties.Mode == EnemyType.Wavey)
                {
                    moveAmp += 0.01f;
                    if (moveAmp >= Math.PI * 4)
                        moveAmp = 0;
                    if (Math.Sin(moveAmp) < 0)
                    {
                        EnemyAngle = (float)(Math.Atan2(CenterScreen.Y - CurrentPosition.Y,
                             CenterScreen.X - CurrentPosition.X)) + (float)Math.PI;
                    }
                }
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
                switch (Properties.Mode)
                {
                    case EnemyType.Jiggler:
                        dir = random.Next(0, 4);
                        switch (dir)
                        {
                            case 0:
                                CurrentPosition.Y -= (4 * WalkMult((float)Math.PI / 2, EnemyAngle, moveAmp, false));break;
                            case 1:
                                CurrentPosition.X -= (4 * WalkMult(0, EnemyAngle, moveAmp, false));break;
                            case 2:
                                CurrentPosition.Y += (4 * WalkMult(3 * (float)Math.PI / 2, EnemyAngle, moveAmp, false));break;
                            default:
                                CurrentPosition.X += (4 * WalkMult((float)Math.PI, EnemyAngle, moveAmp, false));break;
                        }
                        break;
                    case EnemyType.Wavey:
                        CurrentPosition.X -= (float)Math.Cos(EnemyAngle) * ((float)Math.Sin(moveAmp) + 1);
                        CurrentPosition.Y -= (float)Math.Sin(EnemyAngle) * ((float)Math.Sin(moveAmp) + 1);break;
                } 
                spriteBatch.Draw
                (
                    Properties.Texture,
                    new Rectangle
                    (
                        (int)CurrentPosition.X,
                        (int)CurrentPosition.Y,
                        Properties.Texture.Width,
                        Properties.Texture.Height
                    ),
                    null,
                    Color.White,
                    EnemyAngle,
                    new Vector2
                    (
                        Properties.Texture.Width / 2,
                        Properties.Texture.Height / 2
                    ),
                    SpriteEffects.None,
                    (float)LayerDepth.Enemy
                );
            } //If(Spawned)
        } //Draw
    } //class
} //namespace

