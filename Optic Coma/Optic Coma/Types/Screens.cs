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
        public Vector2 LevelSize;
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
        sTitleFlicker titleFlicker;
        Texture2D bg;
        public override void LoadContent()
        {
            base.LoadContent();
            titleGraphic = content.Load<Texture2D>("ocbigSheet");

            titleFlicker = new sTitleFlicker(titleGraphic, 2, 1);

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
            titleFlicker.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            #region background image scaling for screen sizes
            if (Foundation.isFullScreen) {
                if ((Foundation.ScreenWidth - bg.Width) > (Foundation.ScreenHeight - bg.Height)) {
                    spriteBatch.Draw
                    (
                        bg,
                        new Rectangle(0,0, Foundation.ScreenWidth, Foundation.ScreenHeight * Foundation.ScreenHeight / bg.Height), //scale horizontally
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        ScreenManager.Instance.BGLayer
                    );
                }
                else if ((Foundation.ScreenWidth - bg.Width) < (Foundation.ScreenHeight - bg.Height))
                {
                    spriteBatch.Draw
                    (
                        bg,
                        new Rectangle(0,0, Foundation.ScreenWidth * Foundation.ScreenWidth / bg.Width, Foundation.ScreenHeight), //scale vertically
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        ScreenManager.Instance.BGLayer
                    );
                }
                else
                {
                    spriteBatch.Draw
                    (
                        bg,
                        new Rectangle(0,0, Foundation.ScreenWidth, Foundation.ScreenHeight),
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        ScreenManager.Instance.BGLayer
                    );
                }
            }
            else
            {
                spriteBatch.Draw
                (
                    bg,
                    new Rectangle(0, 0, bg.Width, bg.Height),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    ScreenManager.Instance.BGLayer
                );
            }
            #endregion

            spriteBatch.Draw
            (
                bg,
                Vector2.Zero,
                null,
                Color.White,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                ScreenManager.Instance.BGLayer
            );
            titleFlicker.Draw(spriteBatch, new Vector2((ScreenManager.Instance.Dimensions.X - 822) / 2, 20));
            //We're making our button! Woo!
            btnEnterGame.Draw
            (
                //We already defined this thing.
                enterButtonTexture,
                //Spritebatch is love, spritebatch is life.
                spriteBatch,
                //Our event! It passes a method located in ScreenManager.cs. Note the instance - it's a singleton, and remember
                //don't be a simpleton, use instance for singletons!
                ScreenManager.Instance.MenuKey_OnPress,
                //Here's where we want to put our button.
                enterButtonPos,
                //Golly gee, I hope it's Rock Salt.
                buttonFont,
                //What do we want it to say?
                "Enter Game",
                //What color is the text?
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
        Texture2D loadingScreen;
        BackgroundWorker loader = new BackgroundWorker();
        volatile bool hasLoaded; //volatile means that the variable can be used in multiple threads at once

        Vector2 mouseLoc;

        public bool IsPaused = false;

        Effect l1, l2;
        Lighting lightEngine;
        PointLight testLight;
        List<Light> lCollection;
        Texture2D floortextestC;
        Texture2D floortextestN;
        Texture2D floortextestB;

        int screenWidth = (int)ScreenManager.Instance.Dimensions.X;
        int screenHeight = (int)ScreenManager.Instance.Dimensions.Y;

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

            Thread.Sleep(2000); //for testing the loading screen

            LevelSize = new Vector2(ScreenManager.Instance.Dimensions.X * 2, ScreenManager.Instance.Dimensions.Y * 2);

            
            

            #region Lighting
            floortextestC = content.Load<Texture2D>("floortextest_COLOR");
            l1 = content.Load<Effect>("LightingShadow");
            l2 = content.Load<Effect>("LightingCombined");
            lightEngine = new Lighting(Foundation.graphics.GraphicsDevice, l1, l2);
            testLight = new PointLight()
            {
                IsEnabled = true,
                Color = new Vector4(100.0f, 100.0f, 100.0f, 255.0f),
                Power = 1,
                LightDecay = 100,
                Position = new Vector3(300, 300, 20)
            };
            lCollection = new List<Light>();
            lCollection.Add(testLight);
            floortextestN = content.Load<Texture2D>("floortextest_NRM");
            floortextestB = content.Load<Texture2D>("floortextest_SPEC");
            #endregion

            tileSystem = new TileSystem(4, 4, 1);
            floorTexture = content.Load<Texture2D>("floorSheet");
            music = content.Load<SoundEffect>("samplemusic");
            musicInstance = music.CreateInstance();
            musicInstance.IsLooped = true;
            musicInstance.Volume = musicVolume;
            //musicInstance.Play();

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
        public override void Update(GameTime gameTime) //gametime is a tick
        {
            if (hasLoaded)
            {
                if (!IsPaused)
                {
                    Color[] playerColor = new Color[64 * 64];
                    //playerTexture.GetData(playerColor);
                    //enemyTexture.GetData(enemyColor);
                    Rectangle playerArea = new Rectangle((int)playerPos.X, (int)playerPos.Y, 48, 48);

                    int spawnLocationIndicator = random.Next(0, 3);

                    #region commented
                    /*
                    timeSinceLastEnemy += gameTime.ElapsedGameTime.Milliseconds;
                    if (timeSinceLastEnemy > enemySpawnIncrement)
                    {

                        if (spawnLocationIndicator == 0)
                        {
                            enemies.Capacity += 1;
                            enemyInitPosition = left;
                            Enemy enemy = new Enemy(enemyTexture, enemyInitPosition);
                            enemies.Add(enemy);
                        }
                        else if (spawnLocationIndicator == 1)
                        {
                            enemies.Capacity += 1;
                            enemyInitPosition = middle;
                            Enemy enemy = new Enemy(enemyTexture, enemyInitPosition);
                            enemies.Add(enemy);
                        }
                        else if (spawnLocationIndicator == 2)
                        {
                            enemies.Capacity += 1;
                            enemyInitPosition = right;
                            Enemy enemy = new Enemy(enemyTexture, enemyInitPosition);
                            enemies.Add(enemy);
                        }
                        foreach (Enemy enemy in enemies)
                        {
                            if (timeSinceLastEnemy > enemySpawnIncrement * 10)
                                enemy.acceleration += 1;
                        }
                        timeSinceLastEnemy -= enemySpawnIncrement;  
                    }

                    for (int i = 0; i < enemies.Count; i++)
                    {
                        (enemies.ToArray())[i].Update();
                        enemyArea = new Rectangle((int)(enemies.ToArray())[i].CurrentPosition.X + 16, (int)(enemies.ToArray())[i].CurrentPosition.Y + 32, 32, 32);

                        bool intersecting = IntersectPixels(enemyArea, enemyColor, playerArea, playerColor);
                        bool intersectingV2 = enemyArea.Intersects(playerArea);

                        if ((enemies.ToArray())[i].CurrentPosition.Y >= screenHeight)
                        {
                            if (playerScore != 0)
                                playerScore--;
                            enemies.Remove((enemies.ToArray())[i]);
                            enemies.Capacity--;
                        }
                        if (intersecting)
                        {
                            playerScore++;
                            enemies.Remove((enemies.ToArray())[i]);
                            enemies.Capacity--;
                        }
                    }


                    */
                    #endregion

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

                    if (keyState.IsKeyDown(Keys.W))
                    {
                        foreach (var nonPlayer in nonPlayerEntities)
                        {
                            nonPlayer.currentPosition.Y += (4 * Entity.walkMult((float)Math.PI / 2, player.flashAngle, 1, false));
                        }
                        TileOffsetLocation.Y += (4 * Entity.walkMult((float)Math.PI / 2, player.flashAngle, 1, false));
                    }
                    if (keyState.IsKeyDown(Keys.A))
                    {
                        foreach (var nonPlayer in nonPlayerEntities)
                        {
                            nonPlayer.currentPosition.X += (4 * Entity.walkMult(0, player.flashAngle, 1, false));
                        }
                        TileOffsetLocation.X += (4 * Entity.walkMult(0, player.flashAngle, 1, false));
                    }
                    if (keyState.IsKeyDown(Keys.S))
                    {
                        foreach (var nonPlayer in nonPlayerEntities)
                        {
                            nonPlayer.currentPosition.Y -= (4 * Entity.walkMult(3 * (float)Math.PI / 2, player.flashAngle, 1, false));
                        }
                        TileOffsetLocation.Y -= (4 * Entity.walkMult(3 * (float)Math.PI / 2, player.flashAngle, 1, false));
                    }
                    if (keyState.IsKeyDown(Keys.D))
                    {
                        foreach (var nonPlayer in nonPlayerEntities)
                        {
                            nonPlayer.currentPosition.X -= (4 * Entity.walkMult((float)Math.PI, player.flashAngle, 1, false));
                        }
                        TileOffsetLocation.X -= (4 * Entity.walkMult((float)Math.PI, player.flashAngle, 1, false));
                    }
                    #endregion
                    foreach (Enemy enemy in nonPlayerEntities)
                    {
                        enemy.Update();
                    }
                    
                }
            }
            base.Update(gameTime);
        }
        public void DrawScene(SpriteBatch spriteBatch, Texture2D t)
        {
            spriteBatch.End();
            spriteBatch.Begin();
            spriteBatch.Draw(
                t,
                Vector2.Zero,
                Color.White);
            spriteBatch.End();
            spriteBatch.Begin();
        }
        public void DrawNormals(SpriteBatch spriteBatch, Texture2D t)
        {
            spriteBatch.End();
            spriteBatch.Begin();
            spriteBatch.Draw(
                t,
                Vector2.Zero,
                Color.White);
            spriteBatch.End();
            spriteBatch.Begin();
        }
        public void DrawBumps(SpriteBatch spriteBatch, Texture2D t)
        {
            spriteBatch.End();
            spriteBatch.Begin();
            spriteBatch.Draw(
                t,
                Vector2.Zero,
                Color.White);
            spriteBatch.End();
            spriteBatch.Begin();
        }
        GraphicsDevice GDevice = Foundation.graphics.GraphicsDevice;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hasLoaded)
            {
                #region when not paused
                if (!IsPaused)
                {
                    GDevice.SetRenderTarget(Lighting._colorMapRenderTarget);

                    // Clear all render targets
                    GDevice.Clear(Color.Transparent);

                    DrawScene(spriteBatch, floortextestC);

                    GDevice.SetRenderTarget(null);
                    GDevice.SetRenderTarget(Lighting._normalMapRenderTarget);

                    // Clear all render targets
                    GDevice.Clear(Color.Transparent);

                    DrawNormals(spriteBatch, floortextestN);

                    // Deactive the render targets to resolve them
                    GDevice.SetRenderTarget(null);

                    lightEngine.GenerateShadowMap(lCollection);

                    // Finally draw the combined Maps onto the screen
                    lightEngine.DrawCombinedMaps(spriteBatch);

                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    player.Draw(spriteBatch, buttonFont);
                    tileSystem.Draw(floorTexture, spriteBatch, TileOffsetLocation, LevelSize);

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
