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
            BaseScreenContent = new ContentManager(Foundation.GlobalScreenManager.Content.ServiceProvider, "Content");
        }
        public virtual void LoadContent(LevelHandler l)
        {
            BaseScreenContent = new ContentManager(Foundation.GlobalScreenManager.Content.ServiceProvider, "Content");
        }
        public virtual void UnloadContent()
        {
            BaseScreenContent.Unload();
        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }public virtual void Update(GameTime gameTime)
        {

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

            enterButtonPos = new Vector2(Foundation.GlobalScreenManager.Dimensions.X / 2 - enterButtonTexture.Width / 2,
                                         Foundation.GlobalScreenManager.Dimensions.Y / 2 - enterButtonTexture.Height / 8);

            bg = BaseScreenContent.Load<Texture2D>("starsbg");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw
            (
                bg,
                null,
                new Rectangle(0, 0, (int)Foundation.GlobalScreenManager.Dimensions.X, (int)Foundation.GlobalScreenManager.Dimensions.Y),
                null,
                null,
                0,
                null,
                Color.White,
                SpriteEffects.None,
                (float)LayerDepth.BGImage / 10f
            );
            spriteBatch.Draw
            (
                titleGraphic,
                null,
                new Rectangle((int)Entity.CenterScreen.X - titleGraphic.Width / 2, 0, titleGraphic.Width, titleGraphic.Height / 2),
                new Rectangle(0, 0, titleGraphic.Width, titleGraphic.Height / 2),
                null,
                0,
                null,
                Color.White,
                SpriteEffects.None,
                (float)LayerDepth.BGImage / 10f
            );
            
            //We're making our button! Woo!
            btnEnterGame.Draw
            (
                enterButtonTexture,
                spriteBatch,
                Foundation.GlobalScreenManager.MenuKey_OnPress,
                enterButtonPos,
                buttonFont,
                "Enter Game",
                Color.Black
            );
        }
    }
    public class LevelScreen : BaseScreen
    {
        private float deltaTime;
        public volatile bool hasLoaded = false; //volatile means that the variable can be used in multiple threads at once

        public List<Vector2> WalkableTiles = new List<Vector2>();
        public Vector2 TileOffsetLocation;

        private double lowDist, curDist;
        public FrameCounter FrameCounter = new FrameCounter();
        public Vector2 LevelSize;
        public LevelHandler Handler;

        public Texture2D buttonSheet;
        public Button pauseButton;
        public Vector2 pauseButtonPos;
        public SpriteFont font;

        public Button btnUnpause;
        public Vector2 unpauseButtonPos;

        public Button btnExit;
        public Vector2 exitButtonPos;

        public Button btnFullscreen;
        public Vector2 fullButtonPos;

        public Player player;
        public Texture2D playerTexture;
        public string playerPath = "player";
        public Vector2 playerPos;
        public Texture2D lightTexture;
        public Texture2D flashLightTexture;
        public string flashPath = "flashlight";
        public OldTileSystem TileRenderer;
        public List<Enemy> enemies = new List<Enemy>();
        public Texture2D backgroundTexture;

        public Action<object, DoWorkEventArgs> LoaderMethod;

        public int screenWidth = (int)Foundation.GlobalScreenManager.Dimensions.X;
        public int screenHeight = (int)Foundation.GlobalScreenManager.Dimensions.Y;

        public bool inherited;
        public Level InheritedLevel;

        /// <summary>
        /// Checks if player will be out of bounds in the next game tick
        /// </summary>
        /// <param name="walkableTiles"></param>
        /// <param name="nonWalkableTriangles"></param>
        /// <param name="location"></param>
        /// <param name="playerHitBox"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Gets distance between player and nearest enemy
        /// </summary>
        /// <param name="enemies"></param>
        /// <param name="source"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculates magnitude of light as distance increases or decreases
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double LogisticForLight(float x)
        {
            double xD;
            xD = Convert.ToDouble(x);
            return (350 / (1 + Math.Exp(-0.02d*(xD - 200d))));
        }
        
        public LevelScreen()
        {
            inherited = false;
        }
        public LevelScreen(Level l)
        {
            inherited = true;
        }

        public override void LoadContent()
        {
            font = BaseScreenContent.Load<SpriteFont>("buttonFont");
            playerTexture = BaseScreenContent.Load<Texture2D>(playerPath);
            playerPos = new Vector2(Foundation.GlobalScreenManager.Dimensions.X / 2 - playerTexture.Width / 2,
                                     Foundation.GlobalScreenManager.Dimensions.Y / 2 - playerTexture.Height / 8);
            lightTexture = BaseScreenContent.Load<Texture2D>("light");
            flashLightTexture = BaseScreenContent.Load<Texture2D>(flashPath);
            player = new Player(playerTexture, playerPos, flashLightTexture);

            if (inherited == false) //Level1Screen
            {
                backgroundTexture = BaseScreenContent.Load<Texture2D>("starsbg");
                Handler = new LevelHandler(LoaderMethod, hasLoaded);
                Handler.worker.RunWorkerAsync();
            }
            else if (inherited == true) //Level loaded from xml (LevelEditor)
            {
                InheritedLevel.LoadContent();
                Handler = new LevelHandler(InheritedLevel.ALoader, hasLoaded);
            }
            base.LoadContent();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime) //gametime is a tick
        {
            if(hasLoaded)
            { 
                if (inherited)
                {
                    InheritedLevel.Update(gameTime);
                }

                deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                FrameCounter.Update(deltaTime);
            }
            else if(inherited && !hasLoaded) //FOR LEVEL EDITOR
            {
                InheritedLevel.Update(gameTime);
            }
            hasLoaded = Handler.loaded;
        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (hasLoaded)
            {
                
                spriteBatch.End();

                Foundation.LightingEngine.BeginDraw();
                spriteBatch.Begin(SpriteSortMode.BackToFront);
                spriteBatch.Draw
                (
                    backgroundTexture,
                    new Rectangle(0, 0, screenWidth, screenHeight),
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    (float)LayerDepth.BGImage / 10f
                );
                foreach (Enemy enemy in enemies)
                {
                    enemy.Draw(spriteBatch);
                }
                player.Draw(spriteBatch);
                TileRenderer.Draw(spriteBatch, TileOffsetLocation, LevelSize);

                spriteBatch.End();
                Foundation.LightingEngine.Draw(gameTime);

                spriteBatch.Begin(SpriteSortMode.BackToFront);
                if (inherited)
                {
                    InheritedLevel.Draw(spriteBatch, gameTime);
                }
                #region HUD
                var fps = string.Format("FPS: {0}", FrameCounter.AverageFramesPerSecond);
                spriteBatch.DrawString
                (
                    font, 
                    "Position: " + TileOffsetLocation.X + "," + TileOffsetLocation.Y,
                    new Vector2(1, 84),
                    Color.White,
                    0,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    (float)LayerDepth.HUD / 10f
                );
                spriteBatch.DrawString
                (
                    font,
                    "FPS: " + fps,
                    new Vector2(1, 65),
                    Color.White,
                    0,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    (float)LayerDepth.HUD / 10f);
                spriteBatch.DrawString(
                    font,
                    "Lighting Debug Enabled?: " + Foundation.LightingEngine.Debug,
                    new Vector2(1, 103),
                    Color.White,
                    0,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    (float)LayerDepth.HUD / 10f);
                spriteBatch.DrawString(
                    font,
                    "Distance to Closest Enemy: " + GetDistToClosestEnemy(enemies, Entity.CenterScreen),
                    new Vector2(1, 123),
                    Color.White, 
                    0,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    (float)LayerDepth.HUD / 10f);
                pauseButton.Draw
                (
                    buttonSheet,
                    spriteBatch,
                    Foundation.GlobalScreenManager.PauseKey_OnPress,
                    pauseButtonPos,
                    font,
                    "Pause Game",
                    Color.Black
                );
                #endregion
            }
            else if (inherited && !hasLoaded)
            {
                InheritedLevel.Draw(spriteBatch, gameTime);
            }
        }

        public void LoadDefaultButtons()
        {
            buttonSheet = BaseScreenContent.Load<Texture2D>("buttonSheet");
            pauseButton = new Button();
            pauseButtonPos = Vector2.Zero;

            btnUnpause = new Button();
            unpauseButtonPos = new Vector2(Foundation.GlobalScreenManager.Dimensions.X / 2 - buttonSheet.Width / 2,
                                           Foundation.GlobalScreenManager.Dimensions.Y / 2);
            btnExit = new Button();
            exitButtonPos = new Vector2(Foundation.GlobalScreenManager.Dimensions.X / 2 - buttonSheet.Width / 2,
                                        Foundation.GlobalScreenManager.Dimensions.Y / 2 - 128);
            btnFullscreen = new Button();
            fullButtonPos = new Vector2(Foundation.GlobalScreenManager.Dimensions.X / 2 - buttonSheet.Width / 2,
                                        Foundation.GlobalScreenManager.Dimensions.Y / 2 + 64 - 128);
        }
    }
    internal class Level1Screen : LevelScreen
    {
        public bool Equals(Level1Screen level)
        {
            return true;
        }

        #region fields
        private Texture2D loadingScreen;
        
        private Vector2 mouseLoc;

        public bool IsPaused = false;
        private Texture2D debugColRect;

        private Random random = new Random();

        private List<Entity> nonPlayerEntities = new List<Entity>();

        private Texture2D enemyTexture;
        private Texture2D floorTexture;

        private Vector2 enemyPos;

        private string enemyPath = "enemy";

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
            for(int x = 1; x <= 40; x++)
            {
                for(int y = 8; y <= 40; y++)
                {
                    t.Add(new Vector2(x, y));
                }
            }
            return t;
        }
        public Level1Screen()
        {
            LoaderMethod = Loader;
        }
        //this method is called by a background thread while the loading screen is displayed
        public void Loader(object sender, DoWorkEventArgs e)
        {
            IsPaused = false;
            enemies = new List<Enemy>();
            WalkableTiles = TileSetup();
            LevelSize = new Vector2(Foundation.GlobalScreenManager.Dimensions.X * 2, Foundation.GlobalScreenManager.Dimensions.Y * 2);
            debugColRect = BaseScreenContent.Load<Texture2D>("rectbox");
            floorTexture = BaseScreenContent.Load<Texture2D>("floorSheet");
            TileRenderer = new OldTileSystem(floorTexture, 4, 4, 1, LevelSize, WalkableTiles);
            music = BaseScreenContent.Load<SoundEffect>("samplemusic");
            musicInstance = music.CreateInstance();
            musicInstance.IsLooped = true;
            musicInstance.Volume = MusicVolume;
            //musicInstance.Play();

            #region buttons
            LoadDefaultButtons();
            #endregion
            #region entities
            enemyTexture = Foundation.GlobalScreenManager.Content.Load<Texture2D>(enemyPath);
            enemyPos = new Vector2(Foundation.GlobalScreenManager.Dimensions.X / 4 - playerTexture.Width / 2,
                                     Foundation.GlobalScreenManager.Dimensions.Y / 4 - playerTexture.Height / 8);
            enemies.Add(new Enemy(enemyTexture, enemyPos, EnemyType.Jiggler));
            enemies.Add(new Enemy(enemyTexture, new Vector2(Foundation.GlobalScreenManager.Dimensions.X - enemyPos.X, Foundation.GlobalScreenManager.Dimensions.X - enemyPos.Y), EnemyType.Wavey));
            foreach (var enemy in enemies)
            {
                nonPlayerEntities.Add(enemy);               
            }
            #endregion

            TileOffsetLocation = new Vector2(SaveData.LocationX, SaveData.LocationY);

            hasLoaded = true;

            return;
        }
        
        public override void LoadContent()
        {
            base.LoadContent();
            loadingScreen = BaseScreenContent.Load<Texture2D>("loadingScreen");
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        private KeyboardState prevState;

        public override void Update(GameTime gameTime) //gametime is a tick
        {
            if(!hasLoaded)
                base.Update(gameTime);
            if (hasLoaded)
            {
                
                #region  not paused
                if (!IsPaused)
                {
                    Rectangle playerArea = new Rectangle((int) playerPos.X, (int) playerPos.Y, 48, 48);

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
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2( TileOffsetLocation.X, TileOffsetLocation.Y + (float)(4.25 * Entity.WalkMult((float)Math.PI / 2, player.FlashAngle, 1, false))), player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.Y += (float)(4.25 * Entity.WalkMult((float)Math.PI / 2, player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            TileOffsetLocation.Y += (float)(4.25 * Entity.WalkMult((float)Math.PI / 2, player.FlashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.A))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2( TileOffsetLocation.X + (float)(4.25 * Entity.WalkMult(0, player.FlashAngle, 1, false)), TileOffsetLocation.Y), player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.X += (float)(4.25 * Entity.WalkMult(0, player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            TileOffsetLocation.X += (float)(4.25 * Entity.WalkMult(0, player.FlashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.S))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2( TileOffsetLocation.X, TileOffsetLocation.Y - (float)(4.25 * Entity.WalkMult(3 * (float)Math.PI / 2, player.FlashAngle, 1, false))), player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.Y -= (float)(4.25 * Entity.WalkMult(3 * (float)Math.PI / 2, player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            TileOffsetLocation.Y -= (float)(4.25 * Entity.WalkMult(3 * (float)Math.PI / 2, player.FlashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.D))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2( TileOffsetLocation.X - (float)(4.25 * Entity.WalkMult((float)Math.PI, player.FlashAngle, 1, false)), TileOffsetLocation.Y), player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.X -= (float)(4.25 * Entity.WalkMult((float)Math.PI, player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            TileOffsetLocation.X -= (float)(4.25 * Entity.WalkMult((float)Math.PI, player.FlashAngle, 1, false));
                        }
                    }
                    #endregion
                    
                    foreach (Enemy enemy in nonPlayerEntities)
                    {
                        enemy.Update();
                    }
                    float dist = GetDistToClosestEnemy(enemies, playerPos);
                    float colorVal;
                    if (dist <= 370f && dist >= 100)
                    {
                        colorVal = (float)LogisticForLight(dist);
                        player.FlashLight.Color = new Color(1f, colorVal / 350, colorVal / 350, 1f);
                    }
                    else if (dist < 100f)
                    {
                        colorVal = (float)LogisticForLight(dist);
                        player.FlashLight.Color = new Color(1f, colorVal / 350, colorVal / 350, colorVal / 100);
                    }
                    else
                    {
                        player.FlashLight.Color = Color.White;
                    }
                    player.FlashLight.Rotation = (float)Math.PI + player.FlashAngle;
                    Foundation.LightingEngine.Hulls.Clear();
                    foreach(Enemy enemy in nonPlayerEntities)
                    {
                        enemy.UpdateHull(ref Foundation.LightingEngine);
                    }

                    DataToSave[0] = TileOffsetLocation.X; DataToSave[1] = TileOffsetLocation.Y; DataToSave[2] = 0;
                }
                #endregion
                #region paused
                if (IsPaused)
                {
                    //?
                }
                #endregion
            }
        }
  
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            if (hasLoaded)
            {
                #region when not paused
                if (!IsPaused)
                {
                }
                #endregion
                #region when paused (contains options menu)
                else
                {
                    btnExit.Draw
                    (
                        buttonSheet,
                        spriteBatch,
                        Foundation.GlobalScreenManager.ExitKey_OnPress,
                        exitButtonPos,
                        font,
                        "Exit Game",
                        Color.Black
                    );
                    btnUnpause.Draw
                    (
                        buttonSheet,
                        spriteBatch,
                        Foundation.GlobalScreenManager.PauseKey_OnPress,
                        unpauseButtonPos,
                        font,
                        "Un-Pause Game",
                        Color.Black
                    );
                    btnFullscreen.Draw
                    (
                        buttonSheet,
                        spriteBatch,
                        Foundation.GlobalScreenManager.ChangeScreenMode,
                        fullButtonPos,
                        font,
                        "   Toggle \nFullscreen",
                        Color.Black
                    );
                }
                #endregion
            }
            else
            {
                spriteBatch.Draw( loadingScreen, new Rectangle(0, 0, 1024, 800), Color.White);
                spriteBatch.DrawString(base.font, "" + Handler.PercentProgress, new Vector2(0, 0), Color.Black);
            }
        }
    }
}
