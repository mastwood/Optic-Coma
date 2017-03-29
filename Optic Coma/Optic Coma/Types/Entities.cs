using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using System.Xml.Serialization;
using OpticComa_Types;

namespace OpticComa_Main
{

    public abstract class Entity
    {
        /// <summary>
        /// Position of entity
        /// </summary>
        public Vector2 CurrentPosition;
        /// <summary>
        /// Global magic number
        /// </summary>
        public static Vector2 CenterScreen = new Vector2(Foundation.GlobalScreenManager.Dimensions.X / 2, Foundation.GlobalScreenManager.Dimensions.Y / 2);
        /// <summary>
        /// Direction entity is facing
        /// </summary>
        public float Angle;
        /// <summary>
        /// Polygon for shape of shadow
        /// </summary>
        public Hull ShadowHull;

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
        /// Calculates speed at which entities will move in certain situations
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Angle"></param>
        /// <param name="Sloweffect"></param>
        /// <param name="Exponentiation"></param>
        /// <returns></returns>
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

    /// <summary>
    /// Class encapsulating the player character -> controls all movement and drawing logic
    /// </summary>
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
                (float)LayerDepth.Player / 10f
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
                (float)LayerDepth.Flashlight / 10f
            );
        }  
    }

    
    public class NPC : Entity
    {
        public NPCProperties Properties;

        public NPC()
        {
        }

    }
    /// <summary>
    /// Enemy object -> this class controls the enemys actions in game
    /// </summary>
    public class Enemy : Entity
    {
        // Why is this static?
        private static Random random;

        // These are so that we can easily export and import the important properties of the enemy
        public EnemyProperties Properties { get; set; } //The get-set part makes this a property and not a field

        /*
               Properties that end with {get;set;} are called AutoProperties
                These are used so that you dont get funky things like accidentally modifying the wrong value
                Also, non-auto properties are good for hiding stuff
                    For example: an index that starts at 1, maybe you put 0 for convention. A property setter could
                    have logic that automatically changes this to 1 so that you dont have to do debugging.

            danswain on stackexchange says:
            
                Object orientated programming principles say that, the internal workings of a class
                should be hidden from the outside world. If you expose a field you're in essence 
                exposing the internal implementation of the class. Therefore we wrap fields with 
                Properties (or methods in Java's case) to give us the ability to change the implementation
                without breaking code depending on us. Seeing as we can put logic in the Property also
                allows us to perform validation logic etc if we need it. C# 3 has the possibly
                confusing notion of autoproperties. This allows us to simply define the Property 
                and the C#3 compiler will generate the private field for us.
        */

        // Properties
        public float EnemyAngle { get; set; }
        private int speed { get; set; }
        private int direction { get; set; }
        private float velocityModifier { get; set; }

        // Legacy code
        private int acceleration = 0;
        public int Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }
        public bool Spawned = true;

        /// <summary>
        /// Constructs a new enemy
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="initPosition"></param>
        /// <param name="eType"></param>
        public Enemy(Texture2D texture, Vector2 initPosition, EnemyType eType)
        {
            Properties = new EnemyProperties();
            random = new Random();
            Properties.Texture = texture;
            Properties.Mode = eType;
            CurrentPosition = initPosition;
            speed = 4 + acceleration;
            velocityModifier = 0;
            EnemyAngle = 0f;
            ShadowHull = Hull.CreateRectangle(CurrentPosition, 
                new Vector2(texture.Width, texture.Height), Angle, new Vector2(texture.Width/2, texture.Height/2));
            ShadowHull.Enabled = true;
            Foundation.LightingEngine.Hulls.Add(ShadowHull);
        }
        public Enemy(EnemyProperties p, Vector2 initPosition)
        {
            Spawned = false;
            CurrentPosition = initPosition;
            Properties = p;
            speed = 4 + acceleration;
            velocityModifier = 0;
            ShadowHull = Hull.CreateRectangle(CurrentPosition, 
                new Vector2(p.Texture.Width, p.Texture.Height), Angle, new Vector2(p.Texture.Width / 2, p.Texture.Height / 2));
            ShadowHull.Enabled = true;
            Foundation.LightingEngine.Hulls.Add(ShadowHull);
        }
        /// <summary>
        /// Deprecated method - not used atm
        /// </summary>
        public void Initialize()
        {
            //other stuff?
        }
        public override void Update()
        {
            if (Spawned)
            {
                Angle = EnemyAngle;
                if (Properties.Mode == EnemyType.Jiggler)
                {
                    //Atan2(int Opposite, int Adjacent)
                    EnemyAngle = (float)(Math.Atan2(CenterScreen.Y - CurrentPosition.Y,
                                CenterScreen.X - CurrentPosition.X)) + (float)Math.PI;
                    //moveAmp += 0.001f;
                    velocityModifier = 4; //We can toy around with this later.
                    direction = random.Next(0, 4);
                }
                else if (Properties.Mode == EnemyType.Wavey)
                {
                    velocityModifier += 0.01f;
                    if (velocityModifier >= Math.PI * 4)
                        velocityModifier = 0;
                    if (Math.Sin(velocityModifier) < 0)
                    {
                        EnemyAngle = (float)(Math.Atan2(CenterScreen.Y - CurrentPosition.Y,
                                CenterScreen.X - CurrentPosition.X)) + (float)Math.PI;
                    }
                }
                UpdateHull(ref Foundation.LightingEngine);
            }
        }
        /// <summary>
        /// Updates shadow geometry in presence of light
        /// </summary>
        public void UpdateHull(ref PenumbraComponent lightingEngine)
        {
            lightingEngine.Hulls.Remove(ShadowHull);
            ShadowHull.Rotation = Angle;
            ShadowHull.Position = ShadowHull.Origin;
            lightingEngine.Hulls.Add(ShadowHull);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Spawned)
            {
                switch (Properties.Mode)
                {
                    case EnemyType.Jiggler:
                        direction = random.Next(0, 4);
                        switch (direction)
                        {
                            case 0:
                                CurrentPosition.Y -= (4 * WalkMult((float)Math.PI / 2, EnemyAngle, velocityModifier, false)); break;
                            case 1:
                                CurrentPosition.X -= (4 * WalkMult(0, EnemyAngle, velocityModifier, false)); break;
                            case 2:
                                CurrentPosition.Y += (4 * WalkMult(3 * (float)Math.PI / 2, EnemyAngle, velocityModifier, false)); break;
                            default:
                                CurrentPosition.X += (4 * WalkMult((float)Math.PI, EnemyAngle, velocityModifier, false)); break;
                        }
                        break;
                    case EnemyType.Wavey:
                        CurrentPosition.X -= (float)Math.Cos(EnemyAngle) * ((float)Math.Sin(velocityModifier) + 1);
                        CurrentPosition.Y -= (float)Math.Sin(EnemyAngle) * ((float)Math.Sin(velocityModifier) + 1); break;
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
                    (float)LayerDepth.Enemy / 10f
                );
            }
        } //Draw
    } //class
} //namespace

