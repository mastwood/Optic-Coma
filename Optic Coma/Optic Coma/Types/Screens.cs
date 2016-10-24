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
        protected ContentManager content;
        [XmlIgnore]
        public Type Type;

        public BaseScreen()
        {
            Type = this.GetType();
        }

        public virtual void LoadContent()
        {
            content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
        }
        public virtual void UnloadContent()
        {
            content.Unload();
        }
        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
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
        static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
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
    }
    class LevelScreen : BaseScreen
    {
        public FrameCounter frameCounter = new FrameCounter();
        public Vector2 LevelSize;
        float getDistToClosestEnemy(List<Enemy> enemies, Vector2 source)
        {
            double lowDist, curDist;
            lowDist = -1;
            foreach (Enemy enemy in enemies)
            {
                curDist = Math.Sqrt(
                    Math.Pow((double)(Math.Abs(source.X - enemy.currentPosition.X)), 2) +
                    Math.Pow((double)(Math.Abs(source.Y - enemy.currentPosition.Y)), 2)
                );
                if( lowDist == -1 || curDist < lowDist)
                {
                    lowDist = curDist;
                }
            }
            return (float)lowDist;
        }
        public override void LoadContent()
        {
            base.LoadContent();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime) //gametime is a tick
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
    class MenuScreen : BaseScreen
    {
        Button btnEnterGame;
        SpriteFont buttonFont;
        Texture2D enterButtonTexture;
        Vector2 enterButtonPos;
        Texture2D titleGraphic;
        Texture2D bg;
        public override void LoadContent()
        {
            base.LoadContent();
            titleGraphic = content.Load<Texture2D>("ocbigSheet");

            btnEnterGame = new Button();
            enterButtonTexture = content.Load<Texture2D>("buttonSheet");
            buttonFont = content.Load<SpriteFont>("buttonFont");

            enterButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - enterButtonTexture.Width / 2,
                                         ScreenManager.Instance.Dimensions.Y / 2 - enterButtonTexture.Height / 8);

            bg = content.Load<Texture2D>("starsbg");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
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
                ScreenManager.Instance.BGLayer
            );
            spriteBatch.Draw
            (
                titleGraphic,
                null,
                new Rectangle((int)Entity.centerScreen.X - titleGraphic.Width / 2, 0, titleGraphic.Width, titleGraphic.Height/2),
                new Rectangle(0, 0, titleGraphic.Width, titleGraphic.Height/2),
                null,
                0,
                null,
                Color.White,
                SpriteEffects.None,
                ScreenManager.Instance.BGLayer-0.01f
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
    class Level1Screen : LevelScreen
    {
        public bool Equals(Level1Screen level)
        {
            return true;
        }
        
        Type type;
        public Level1Screen()
        {
            type = GetType();
        }

        #region fields

        Spotlight testLight;

        Texture2D loadingScreen;
        BackgroundWorker loader = new BackgroundWorker();
        volatile bool hasLoaded; //volatile means that the variable can be used in multiple threads at once

        Vector2 mouseLoc;

        public bool IsPaused = false;

        Texture2D debugColRect;
        List<Vector2> movementTiles;
        
        int screenWidth = (int)ScreenManager.Instance.Dimensions.X;
        int screenHeight = (int)ScreenManager.Instance.Dimensions.Y;

        float deltaTime;

        Random random = new Random();

        Player player;

        List<Enemy> enemies = new List<Enemy>();
        List<Entity> nonPlayerEntities = new List<Entity>();

        TileSystem tileSystem;

        Texture2D lightTexture;
        Texture2D flashLightTexture;
        Texture2D playerTexture;
        Texture2D enemyTexture;
        Texture2D floorTexture;
        
        Vector2 TileOffsetLocation;
        Vector2 playerPos;
        Vector2 enemyPos;
        string playerPath = "player";
        string enemyPath = "enemy";
        string flashPath = "flashlight";

        Texture2D buttonSheet;
        private Button pauseButton;
        Vector2 pauseButtonPos;
        SpriteFont buttonFont;

        private Button btnUnpause;
        Vector2 unpauseButtonPos;

        private Button btnExit;
        Vector2 exitButtonPos;
        Texture2D bg;
        private Button btnFullscreen;
        Vector2 fullButtonPos;

        SoundEffect music;
        SoundEffectInstance musicInstance;
        public float musicVolume = 0.02f;
        #endregion

        //this method is called after everything loads
        protected void Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            loader.DoWork -= LoadAsync;
            loader.RunWorkerCompleted -= Complete;
            loader = null;
            hasLoaded = true;
        }
        //this method is called by a background thread while the loading screen is displayed
        protected void LoadAsync(object sender, DoWorkEventArgs e)
        {
            IsPaused = false;
            testLight = new Spotlight()
            {
                Position = Entity.centerScreen,
                Radius = 900,
                Enabled = true,
                CastsShadows = true,
                Scale = new Vector2(900, 900),
                Color = Color.White,
                Intensity = 2,
                ShadowType = ShadowType.Occluded,
            };
            Foundation.lightingEngine.Lights.Add(testLight);

            LevelSize = new Vector2(ScreenManager.Instance.Dimensions.X * 2, ScreenManager.Instance.Dimensions.Y * 2);

            #region tiles
            movementTiles = new List<Vector2>();
            for (int i = 10; i < 30; i++)
            {
                for (int j = 3; j < 5; j++)
                {
                    movementTiles.Add(new Vector2(i, j));
                }
            }
            for (int i = 5; i < 30; i++)
            {
                for (int j = 5; j < 10; j++)
                {
                    movementTiles.Add(new Vector2(i, j));
                }
            }
            debugColRect = content.Load<Texture2D>("rectbox");
            floorTexture = content.Load<Texture2D>("floorSheet");
            tileSystem = new TileSystem(floorTexture, 4, 4, 1, LevelSize, movementTiles);
            #endregion

            music = content.Load<SoundEffect>("samplemusic");
            musicInstance = music.CreateInstance();
            musicInstance.IsLooped = true;
            musicInstance.Volume = musicVolume;
            //musicInstance.Play();

            bg = content.Load<Texture2D>("starsbg");

            #region buttons
            buttonSheet = content.Load<Texture2D>("buttonSheet");
            pauseButton = new Button();
            pauseButtonPos = Vector2.Zero;
            buttonFont = content.Load<SpriteFont>("buttonFont");

            btnUnpause = new Button();
            unpauseButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - buttonSheet.Width / 2,
                                           ScreenManager.Instance.Dimensions.Y / 2);
            btnExit = new Button();
            exitButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - buttonSheet.Width / 2,
                                        ScreenManager.Instance.Dimensions.Y / 2 - 128);
            btnFullscreen = new Button();
            fullButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - buttonSheet.Width / 2,
                                        ScreenManager.Instance.Dimensions.Y / 2 + 64 - 128);
            #endregion
            #region entities
            lightTexture = content.Load<Texture2D>("light");
            flashLightTexture = content.Load<Texture2D>(flashPath);
            playerTexture = content.Load<Texture2D>(playerPath);
            enemyTexture = content.Load<Texture2D>(enemyPath);

            playerPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - playerTexture.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 2 - playerTexture.Height / 8);

            enemyPos = new Vector2(ScreenManager.Instance.Dimensions.X / 4 - playerTexture.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 4 - playerTexture.Height / 8);

            player = new Player(playerTexture, playerPos, flashLightTexture, lightTexture);

            enemies.Add(new Enemy(enemyTexture, enemyPos));
            enemies.Add(new Enemy(enemyTexture, new Vector2(ScreenManager.Instance.Dimensions.X - enemyPos.X, ScreenManager.Instance.Dimensions.X - enemyPos.Y)));
            foreach (var enemy in enemies) nonPlayerEntities.Add(enemy);
            #endregion
            return;
        }
        
        public override void LoadContent()
        {
            base.LoadContent();
            loadingScreen = content.Load<Texture2D>("loadingScreen");
            hasLoaded = false;

            //"loader" is a "BackgroundWorker", meaning it opens up a thread and performs a method while the program can continue running
            loader.DoWork += new DoWorkEventHandler(LoadAsync); //makes the action to perform "LoadAsync()"
            loader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Complete); //makes the action to perform when done "Complete()"
            loader.RunWorkerAsync(); //runs the worker then continues the game loop (right now just the loading screen until everything is done)
            
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        KeyboardState prevState;
        public override void Update(GameTime gameTime) //gametime is a tick
        {
            if (hasLoaded)
            {
                deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                frameCounter.Update(deltaTime);

                if (!IsPaused)
                {
                    Rectangle playerArea = new Rectangle((int)playerPos.X, (int)playerPos.Y, 48, 48);

                    int spawnLocationIndicator = random.Next(0, 3);

                    MouseState curMouse = Mouse.GetState();

                    mouseLoc = new Vector2(curMouse.X, curMouse.Y);
                    mouseLoc.X = curMouse.X;
                    mouseLoc.Y = curMouse.Y;

                    player.facingDirection = mouseLoc - player.currentPosition;

                    // using radians
                    // measure clockwise from left
                    #region player movement
                    player.flashAngle = (float)(Math.Atan2(player.facingDirection.Y, player.facingDirection.X)) + (float)Math.PI;

                    if ((player.flashAngle > 0 && player.flashAngle <= Math.PI / 4) || (player.flashAngle > Math.PI * 7 / 4 && player.flashAngle <= 2 * Math.PI))
                    {
                        player.playerAngle = (float)Math.PI; //Right
                    }
                    else if (player.flashAngle > Math.PI / 4 && player.flashAngle <= Math.PI * 3 / 4)
                    {
                        player.playerAngle = -(float)Math.PI / 2; //Down
                    }
                    else if (player.flashAngle > Math.PI * 3 / 4 && player.flashAngle <= Math.PI * 5 / 4)
                    {
                        player.playerAngle = 0f; //Left
                    }
                    else if (player.flashAngle > Math.PI * 5 / 4 && player.flashAngle <= Math.PI * 7 / 4)
                    {
                        player.playerAngle = (float)Math.PI / 2; //Up
                    }
                    KeyboardState keyState = Keyboard.GetState();
                    if (keyState.IsKeyDown(Keys.R) && keyState != prevState)
                    {
                        Foundation.lightingEngine.Debug = !Foundation.lightingEngine.Debug;
                    }
                    prevState = keyState;
                    if (keyState.IsKeyDown(Keys.W))
                    {
                        if (TileOffsetLocation.Y + (4.25 * Entity.walkMult((float)Math.PI / 2, player.flashAngle, 1, false)) <= 368)
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.currentPosition.Y += (float)(4.25 * Entity.walkMult((float)Math.PI / 2, player.flashAngle, 1, false));
                                nonPlayer.hull.Position = nonPlayer.currentPosition;
                            }
                            TileOffsetLocation.Y += (float)(4.25 * Entity.walkMult((float)Math.PI / 2, player.flashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.A))
                    {
                        if (TileOffsetLocation.X + (4.25 * Entity.walkMult(0, player.flashAngle, 1, false)) <= 478)
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.currentPosition.X += (float)(4.25 * Entity.walkMult(0, player.flashAngle, 1, false));
                                nonPlayer.hull.Position = nonPlayer.currentPosition;
                            }
                            TileOffsetLocation.X += (float)(4.25 * Entity.walkMult(0, player.flashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.S))
                    {
                        if (TileOffsetLocation.Y - (4.25 * Entity.walkMult(3 * (float)Math.PI / 2, player.flashAngle, 1, false)) >= -1160)
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.currentPosition.Y -= (float)(4.25 * Entity.walkMult(3 * (float)Math.PI / 2, player.flashAngle, 1, false));
                                nonPlayer.hull.Position = nonPlayer.currentPosition;
                            }
                            TileOffsetLocation.Y -= (float)(4.25 * Entity.walkMult(3 * (float)Math.PI / 2, player.flashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.D))
                    {
                        if (TileOffsetLocation.X - (4.25 * Entity.walkMult((float)Math.PI, player.flashAngle, 1, false)) >= -1500)
                        {
                            foreach (var nonPlayer in nonPlayerEntities)
                            {
                                nonPlayer.currentPosition.X -= (float)(4.25 * Entity.walkMult((float)Math.PI, player.flashAngle, 1, false));
                                nonPlayer.hull.Position = nonPlayer.currentPosition;
                            }
                            TileOffsetLocation.X -= (float)(4.25 * Entity.walkMult((float)Math.PI, player.flashAngle, 1, false));
                        }
                    }
                    #endregion
                    foreach (Enemy enemy in nonPlayerEntities)
                    {
                        enemy.Update();
                    }
                    testLight.Rotation = (float)Math.PI + player.flashAngle;
                    Foundation.lightingEngine.Hulls.Clear();
                    foreach(Entity e in nonPlayerEntities)
                    {
                        Foundation.lightingEngine.Hulls.Add(e.hull);
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (hasLoaded)
            {
                var fps = string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond);

                #region when not paused
                if (!IsPaused)
                {
                    spriteBatch.End();

                    Foundation.lightingEngine.BeginDraw();
                    spriteBatch.Begin(SpriteSortMode.BackToFront);
                    spriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, ScreenManager.Instance.BGLayer);
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    player.Draw(spriteBatch, buttonFont);
                    tileSystem.Draw(spriteBatch, TileOffsetLocation, LevelSize);

                    spriteBatch.End();
                    Foundation.lightingEngine.Draw(gameTime);
                    spriteBatch.Begin(SpriteSortMode.BackToFront);
                    spriteBatch.DrawString(buttonFont, "Position: " + TileOffsetLocation.X + "," + TileOffsetLocation.Y, new Vector2(1, 84), Color.White);
                    spriteBatch.DrawString(buttonFont, fps, new Vector2(1, 65), Color.White);
                    spriteBatch.DrawString(buttonFont, "Lighting Debug Enabled?: " + Foundation.lightingEngine.Debug, new Vector2(1, 103), Color.White);
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
                spriteBatch.Draw(loadingScreen, new Rectangle(0, 0, 1024, 800), Color.White);
            }
        }
    }
}
