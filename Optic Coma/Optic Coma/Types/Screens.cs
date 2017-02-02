using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Xml.Serialization;
using System.Threading;
using System.ComponentModel;
using Penumbra;

namespace Optic_Coma
{
    public class BaseScreen
    {
        public static ContentManager BaseScreenContent;
        [XmlIgnore]
        public Type Type;

        public BaseScreen()
        {
            Type = this.GetType();
        }

        public virtual void LoadContent()
        {
            BaseScreenContent = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
        }
        public virtual void LoadContent(LevelHandler l)
        {
            BaseScreenContent = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
        }
        public virtual void UnloadContent()
        {
            BaseScreenContent.Unload();
        }
        public static Rectangle RectangularHitbox(Orientation o, Rectangle r)
        {
            // rotates hitbox alongside players sprite
            if (o == Orientation.Horiz)
            {
                return new Rectangle(r.Y, r.X, r.Height, r.Width);
            }
            else
            {
                return new Rectangle(r.X, r.Y, r.Width, r.Height);
            }
        }
        /*
        private static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }

                }
            }
            return false;
        }
        */
    }

    public class LevelScreen : BaseScreen
    {
        public List<Vector2> WalkableTiles = new List<Vector2>();

        private double lowDist, curDist;
        public FrameCounter FrameCounter = new FrameCounter();
        public Vector2 LevelSize;
        public LevelHandler Handler;

        public static bool NotOutOfBounds(List<Vector2> walkableTiles, List<Triangle> nonWalkableTriangles, Vector2 location, Rectangle playerHitBox)
        {
            List<Rectangle> levelArea = new List<Rectangle>();

            foreach (Vector2 v in walkableTiles)
            {
                levelArea.Add(new Rectangle((int)(v.X * 32 + location.X), (int)(v.Y * 32 + location.Y), 32, 32));
            }
            foreach (Rectangle z in levelArea)
            {
                if (z.Contains(Entity.CenterScreen))
                {
                    return true;
                }               
            }
            if (nonWalkableTriangles != null) {
                foreach (Triangle t in nonWalkableTriangles)
                {
                    if (t.ContainsCornersOf(playerHitBox))
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        
        public float GetDistToClosestEnemy(List<Enemy> enemies, Vector2 source)
        {
            lowDist = -1;
            foreach (Enemy enemy in enemies)
            {
                curDist = Math.Sqrt
                (
                    Math.Pow((Math.Abs(source.X - enemy.CurrentPosition.X)), 2) +
                    Math.Pow((Math.Abs(source.Y - enemy.CurrentPosition.Y)), 2)
                );
                if( lowDist == -1 || curDist < lowDist)
                {
                    lowDist = curDist;
                }
            }
            return (float)lowDist;
        }
        public double LogisticForLight(float x)
        {
            double xD;
            xD = Convert.ToDouble(x);
            return (350 / (1 + Math.Exp(-0.02d*(xD - 200d))));
        }
        public Texture2D buttonSheet;
        public Button pauseButton;
        public Vector2 pauseButtonPos;
        public SpriteFont buttonFont;

        public Button btnUnpause;
        public Vector2 unpauseButtonPos;

        public Button btnExit;
        public Vector2 exitButtonPos;
        public Texture2D bg;
        public Button btnFullscreen;
        public Vector2 fullButtonPos;
        Action<object, DoWorkEventArgs> method;

        public LevelScreen(Level l)
        {
            ;
        }

        public void SetMethod(Action<object, DoWorkEventArgs> m)
        {
            method = m;
        }
        public override void LoadContent(LevelHandler l)
        {
            base.LoadContent();
            Handler = new LevelHandler(method);
            Handler.BeginLoad();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public virtual void Update(GameTime gameTime) //gametime is a tick
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        public void LoadDefaultButtons()
        {
            buttonSheet = BaseScreenContent.Load<Texture2D>("buttonSheet");
            pauseButton = new Button();
            pauseButtonPos = Vector2.Zero;
            buttonFont = BaseScreenContent.Load<SpriteFont>("buttonFont");

            btnUnpause = new Button();
            unpauseButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - buttonSheet.Width / 2,
                                           ScreenManager.Instance.Dimensions.Y / 2);
            btnExit = new Button();
            exitButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - buttonSheet.Width / 2,
                                        ScreenManager.Instance.Dimensions.Y / 2 - 128);
            btnFullscreen = new Button();
            fullButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - buttonSheet.Width / 2,
                                        ScreenManager.Instance.Dimensions.Y / 2 + 64 - 128);
        }
    }

    internal class MenuScreen : BaseScreen
    {
        private Button btnEnterGame;
        private SpriteFont buttonFont;
        private Texture2D enterButtonTexture;
        private Vector2 enterButtonPos;
        private Texture2D titleGraphic;
        private Texture2D bg;
        public override void LoadContent()
        {
            base.LoadContent();
            titleGraphic = BaseScreenContent.Load<Texture2D>("ocbigSheet");

            btnEnterGame = new Button();
            enterButtonTexture = BaseScreenContent.Load<Texture2D>("buttonSheet");
            buttonFont = BaseScreenContent.Load<SpriteFont>("buttonFont");

            enterButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - enterButtonTexture.Width / 2,
                                         ScreenManager.Instance.Dimensions.Y / 2 - enterButtonTexture.Height / 8);

            bg = BaseScreenContent.Load<Texture2D>("starsbg");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw
            (
                bg,
                null,
                new Rectangle(0,0,(int)ScreenManager.Instance.Dimensions.X, (int)ScreenManager.Instance.Dimensions.Y),
                null, 
                null,
                0,
                null,
                Color.White,
                SpriteEffects.None,
                ScreenManager.Instance.BgLayer
            );
            spriteBatch.Draw
            (
                titleGraphic,
                null,
                new Rectangle((int)Entity.CenterScreen.X - titleGraphic.Width / 2, 0, titleGraphic.Width, titleGraphic.Height/2),
                new Rectangle(0, 0, titleGraphic.Width, titleGraphic.Height/2),
                null,
                0,
                null,
                Color.White,
                SpriteEffects.None,
                ScreenManager.Instance.BgLayer-0.01f
            );
            //We're making our button! Woo!
            btnEnterGame.Draw
            (
                enterButtonTexture,
                spriteBatch,
                ScreenManager.Instance.MenuKey_OnPress,
                enterButtonPos,
                buttonFont,
                "Enter Game",
                Color.Black
            );
        }
    }

    internal class Level1Screen : LevelScreen
    {
        public bool Equals(Level1Screen level)
        {
            return true;
        }

        #region fields

        private Spotlight testLight;

        private Texture2D loadingScreen;
        private BackgroundWorker loader = new BackgroundWorker();
        private volatile bool hasLoaded; //volatile means that the variable can be used in multiple threads at once

        private Vector2 mouseLoc;

        public bool IsPaused = false;
        private Texture2D debugColRect;

        private int screenWidth = (int)ScreenManager.Instance.Dimensions.X;
        private int screenHeight = (int)ScreenManager.Instance.Dimensions.Y;

        private float deltaTime;

        private Random random = new Random();

        private Player player;

        private List<Enemy> enemies = new List<Enemy>();
        private List<Entity> nonPlayerEntities = new List<Entity>();

        private TileSystem walkableTileRenderer;

        private Texture2D lightTexture;
        private Texture2D flashLightTexture;
        private Texture2D playerTexture;
        private Texture2D enemyTexture;
        private Texture2D floorTexture;

        private Vector2 tileOffsetLocation;
        private Vector2 playerPos;
        private Vector2 enemyPos;
        private string playerPath = "player";
        private string enemyPath = "enemy";
        private string flashPath = "flashlight";



        private SoundEffect music;
        private SoundEffectInstance musicInstance;
        public float MusicVolume = 0.02f;
        #endregion

        public float[] DataToSave = new float[3];

        //TILE SIZE IS 32x32
        //SCREEN SIZE IS 1024x800
        public List<Vector2> TileSetup()
        {
            List<Vector2> t = new List<Vector2>();
            for(int x = 11; x <= 20; x++)
            {
                for(int y = 8; y <= 16; y++)
                {
                    t.Add(new Vector2(x, y));
                }
            }
            return t;
        }

        //this method is called by a background thread while the loading screen is displayed
        protected void Loader(object sender, DoWorkEventArgs e)
        {
            IsPaused = false;

            WalkableTiles = TileSetup();

            testLight = new Spotlight()
            {
                Position = Entity.CenterScreen,
                Enabled = true,
                CastsShadows = true,
                Scale = new Vector2(900, 900),
                Color = Color.White,
                Intensity = 2,
                ShadowType = ShadowType.Occluded,
            };
            Foundation.LightingEngine.Lights.Add(testLight);

            LevelSize = new Vector2(ScreenManager.Instance.Dimensions.X * 2, ScreenManager.Instance.Dimensions.Y * 2);

            debugColRect = BaseScreenContent.Load<Texture2D>("rectbox");
            floorTexture = BaseScreenContent.Load<Texture2D>("floorSheet");
            walkableTileRenderer = new TileSystem(floorTexture, 4, 4, 1, LevelSize, WalkableTiles);

            music = BaseScreenContent.Load<SoundEffect>("samplemusic");
            musicInstance = music.CreateInstance();
            musicInstance.IsLooped = true;
            musicInstance.Volume = MusicVolume;
            //musicInstance.Play();

            bg = BaseScreenContent.Load<Texture2D>("starsbg");

            #region buttons
            base.LoadDefaultButtons();
            #endregion
            #region entities
            lightTexture = BaseScreenContent.Load<Texture2D>("light");
            flashLightTexture = BaseScreenContent.Load<Texture2D>(flashPath);
            playerTexture = BaseScreenContent.Load<Texture2D>(playerPath);
            enemyTexture = BaseScreenContent.Load<Texture2D>(enemyPath);

            playerPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - playerTexture.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 2 - playerTexture.Height / 8);

            enemyPos = new Vector2(ScreenManager.Instance.Dimensions.X / 4 - playerTexture.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 4 - playerTexture.Height / 8);

            player = new Player(playerTexture, playerPos, flashLightTexture, lightTexture);

            enemies.Add(new Enemy(enemyTexture, enemyPos));
            enemies.Add(new Enemy(enemyTexture, new Vector2(ScreenManager.Instance.Dimensions.X - enemyPos.X, ScreenManager.Instance.Dimensions.X - enemyPos.Y)));
            foreach (var enemy in enemies) nonPlayerEntities.Add(enemy);
            #endregion

            tileOffsetLocation = new Vector2(SaveData.LocationX, SaveData.LocationY);

            return;
        }
        
        public override void LoadContent()
        {
            SetMethod(Loader);
           
            base.LoadContent();
            loadingScreen = BaseScreenContent.Load<Texture2D>("loadingScreen");

            hasLoaded = false;
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        private KeyboardState prevState;

        public Level1Screen(Level l) : base(l)
        {
        }

        public override void Update(GameTime gameTime) //gametime is a tick
        {
            hasLoaded = Handler.LoadingSuccess();
            if ( hasLoaded)
            {
                deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                FrameCounter.Update( deltaTime);
                #region  not paused
                if (!IsPaused)
                {
                    Rectangle playerArea = new Rectangle((int) playerPos.X, (int) playerPos.Y, 48, 48);

                    int spawnLocationIndicator = random.Next(0, 3);

                    MouseState curMouse = Mouse.GetState();

                    mouseLoc = new Vector2(curMouse.X, curMouse.Y);
                    mouseLoc.X = curMouse.X;
                    mouseLoc.Y = curMouse.Y;

                    player.FacingDirection = mouseLoc - player.CurrentPosition;

                    // using radians
                    // measure clockwise from left
                    #region player rotation
                    player.FlashAngle = (float)(Math.Atan2( player.FacingDirection.Y, player.FacingDirection.X)) + (float)Math.PI;

                    if (( player.FlashAngle > 0 && player.FlashAngle <= Math.PI / 4) || ( player.FlashAngle > Math.PI * 7 / 4 && player.FlashAngle <= 2 * Math.PI))
                    {
                        player.PlayerAngle = (float)Math.PI; //Right
                    }
                    else if ( player.FlashAngle > Math.PI / 4 && player.FlashAngle <= Math.PI * 3 / 4)
                    {
                        player.PlayerAngle = -(float)Math.PI / 2; //Down
                    }
                    else if ( player.FlashAngle > Math.PI * 3 / 4 && player.FlashAngle <= Math.PI * 5 / 4)
                    {
                        player.PlayerAngle = 0f; //Left
                    }
                    else if ( player.FlashAngle > Math.PI * 5 / 4 && player.FlashAngle <= Math.PI * 7 / 4)
                    {
                        player.PlayerAngle = (float)Math.PI / 2; //Up
                    }
                    KeyboardState keyState = Keyboard.GetState();
                    if (keyState.IsKeyDown(Keys.R) && keyState != prevState)
                    {
                        Foundation.LightingEngine.Debug = !Foundation.LightingEngine.Debug;
                    }
                    #endregion
                    prevState = keyState;
                    #region collision and movement
                    if (keyState.IsKeyDown(Keys.W))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2( tileOffsetLocation.X, tileOffsetLocation.Y + (float)(4.25 * Entity.WalkMult((float)Math.PI / 2, player.FlashAngle, 1, false))), player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.Y += (float)(4.25 * Entity.WalkMult((float)Math.PI / 2, player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            tileOffsetLocation.Y += (float)(4.25 * Entity.WalkMult((float)Math.PI / 2, player.FlashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.A))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2( tileOffsetLocation.X + (float)(4.25 * Entity.WalkMult(0, player.FlashAngle, 1, false)), tileOffsetLocation.Y), player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.X += (float)(4.25 * Entity.WalkMult(0, player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            tileOffsetLocation.X += (float)(4.25 * Entity.WalkMult(0, player.FlashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.S))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2( tileOffsetLocation.X, tileOffsetLocation.Y - (float)(4.25 * Entity.WalkMult(3 * (float)Math.PI / 2, player.FlashAngle, 1, false))), player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.Y -= (float)(4.25 * Entity.WalkMult(3 * (float)Math.PI / 2, player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            tileOffsetLocation.Y -= (float)(4.25 * Entity.WalkMult(3 * (float)Math.PI / 2, player.FlashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.D))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2( tileOffsetLocation.X - (float)(4.25 * Entity.WalkMult((float)Math.PI, player.FlashAngle, 1, false)), tileOffsetLocation.Y), player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.X -= (float)(4.25 * Entity.WalkMult((float)Math.PI, player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            tileOffsetLocation.X -= (float)(4.25 * Entity.WalkMult((float)Math.PI, player.FlashAngle, 1, false));
                        }
                    }
                    #endregion

                    foreach (Enemy enemy in nonPlayerEntities)
                    {
                        enemy.Update();
                    }
                    float dist = GetDistToClosestEnemy( enemies, playerPos);
                    float colorVal;
                    if (dist <= 370f && dist >= 100)
                    {
                        colorVal = (float)LogisticForLight(dist);
                        testLight.Color = new Color(1f, colorVal / 350, colorVal / 350, 1f);
                    }
                    else if (dist < 100f)
                    {
                        colorVal = (float)LogisticForLight(dist);
                        testLight.Color = new Color(1f, colorVal / 350, colorVal / 350, colorVal / 100);
                    }
                    else
                    {
                        testLight.Color = Color.White;
                    }
                    testLight.Rotation = (float)Math.PI + player.FlashAngle;
                    Foundation.LightingEngine.Hulls.Clear();
                    foreach(Entity e in nonPlayerEntities)
                    {
                        Foundation.LightingEngine.Hulls.Add(e.Hull);
                    }
                    DataToSave[0] = tileOffsetLocation.X; DataToSave[1] = tileOffsetLocation.Y; DataToSave[2] = 0;
                }
                #endregion
                #region paused
                if (IsPaused)
                {
                    //?
                }
                #endregion
            }

            base.Update(gameTime);
        }
  
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if ( hasLoaded)
            {
                var fps = string.Format("FPS: {0}", FrameCounter.AverageFramesPerSecond);

                #region when not paused
                if (!IsPaused)
                {
                    spriteBatch.End();

                    Foundation.LightingEngine.BeginDraw();
                    spriteBatch.Begin(SpriteSortMode.BackToFront);
                    spriteBatch.Draw( bg, new Rectangle(0, 0, screenWidth, screenHeight), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, ScreenManager.Instance.BgLayer);
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    player.Draw(spriteBatch, buttonFont);
                    walkableTileRenderer.Draw(spriteBatch, tileOffsetLocation, LevelSize);

                    spriteBatch.End();
                    Foundation.LightingEngine.Draw(gameTime);

                    spriteBatch.Begin(SpriteSortMode.BackToFront);
                    spriteBatch.DrawString( buttonFont, "Position: " + tileOffsetLocation.X + "," + tileOffsetLocation.Y, new Vector2(1, 84), Color.White);
                    spriteBatch.DrawString( buttonFont, fps, new Vector2(1, 65), Color.White);
                    spriteBatch.DrawString( buttonFont, "Lighting Debug Enabled?: " + Foundation.LightingEngine.Debug, new Vector2(1, 103), Color.White);
                    spriteBatch.DrawString( buttonFont, "Distance to Closest Enemy: " + GetDistToClosestEnemy( enemies, Entity.CenterScreen), new Vector2(1, 123), Color.White);
                    pauseButton.Draw
                    (
                        buttonSheet,
                        spriteBatch,
                        ScreenManager.Instance.PauseKey_OnPress,
                        pauseButtonPos,
                        buttonFont,
                        "Pause Game",
                        Color.Black
                    );
                }
                #endregion
                #region when paused (contains options menu)
                else
                {
                    btnExit.Draw
                    (
                        buttonSheet,
                        spriteBatch,
                        ScreenManager.Instance.ExitKey_OnPress,
                        exitButtonPos,
                        buttonFont,
                        "Exit Game",
                        Color.Black
                    );
                    btnUnpause.Draw
                    (
                        buttonSheet,
                        spriteBatch,
                        ScreenManager.Instance.PauseKey_OnPress,
                        unpauseButtonPos,
                        buttonFont,
                        "Un-Pause Game",
                        Color.Black
                    );
                    btnFullscreen.Draw
                    (
                        buttonSheet,
                        spriteBatch,
                        ScreenManager.Instance.ChangeScreenMode,
                        fullButtonPos,
                        buttonFont,
                        "   Toggle \nFullscreen",
                        Color.Black
                    );
                }
                #endregion
            }
            else
            {
                spriteBatch.Draw( loadingScreen, new Rectangle(0, 0, 1024, 800), Color.White);
            }
        }
    }
}
