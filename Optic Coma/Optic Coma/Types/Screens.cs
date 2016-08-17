using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Xml.Serialization;
using Krypton;

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
    }

    class MenuScreen : BaseScreen
    {
        Button btnEnterGame;
        SpriteFont buttonFont;
        Texture2D enterButtonTexture;
        Vector2 enterButtonPos;

        public override void LoadContent()
        {
            base.LoadContent();

            btnEnterGame = new Button();
            enterButtonTexture = content.Load<Texture2D>("buttonSheet");
            buttonFont = content.Load<SpriteFont>("buttonFont");

            enterButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - enterButtonTexture.Width / 2,
                                         ScreenManager.Instance.Dimensions.Y / 2 - enterButtonTexture.Height / 8);
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
    class InGameScreen : BaseScreen
    {

        int enemySpawnIncrement = 750;
        int timeSinceLastEnemy = 0;

        int screenWidth = (int)ScreenManager.Instance.Dimensions.X;
        int screenHeight = (int)ScreenManager.Instance.Dimensions.Y;

        Random random = new Random();

        uint playerScore = 0;
        SpriteFont scoreDisplay;

        Vector2 enemyInitPosition;

        Vector2 left;
        Vector2 middle;
        Vector2 right;

        Player player;

        List<Enemy> enemies = new List<Enemy>();

        Texture2D enemyTexture;
        string EnemyPath = "enemy";

        Texture2D pointerTriangle; //read: playerBox, i just havent changed the names yet
        Vector2 pointerPosition;
        string PointerPath = "player";

        Texture2D buttonSheet;
        private Button pauseButton;
        Vector2 pauseButtonPos;
        SpriteFont buttonFont;

        SoundEffect music;
        SoundEffectInstance musicInstance;
        public float musicVolume = 0.2f;



        public override void LoadContent()
        {

            left = new Vector2(screenWidth - 200, 0);
            middle = new Vector2(screenWidth / 2 - 16, 0);
            right = new Vector2(200 - 32, 0);
            
            enemies.Capacity = 0;
            base.LoadContent();

            music = content.Load<SoundEffect>("samplemusic");
            musicInstance = music.CreateInstance();
            musicInstance.IsLooped = true;
            musicInstance.Volume = musicVolume;

            buttonSheet = content.Load<Texture2D>("buttonSheet");
            pauseButton = new Button();
            pauseButtonPos = Vector2.Zero;
            buttonFont = content.Load<SpriteFont>("buttonFont");

            scoreDisplay = content.Load<SpriteFont>("ingamescreen_title");

            enemyTexture = content.Load<Texture2D>(EnemyPath);


            pointerTriangle = content.Load<Texture2D>(PointerPath);
            pointerPosition = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - pointerTriangle.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 2 - pointerTriangle.Height / 8);

            player = new Player(pointerTriangle);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime) //gametime is a tick
        {
            Color[] playerColor = new Color[64 * 64];
            pointerTriangle.GetData(playerColor);
            Color[] enemyColor = new Color[32 * 32];
            enemyTexture.GetData(enemyColor);
            Rectangle playerArea = new Rectangle((int)pointerPosition.X, (int)pointerPosition.Y, 48, 48);
            Rectangle enemyArea = new Rectangle();

            int spawnLocationIndicator = random.Next(0, 3);

            timeSinceLastEnemy += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastEnemy > enemySpawnIncrement)
            {
                try
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
                catch (Exception e)
                {
                    /*LogWriter.Write
                        (
                        e.Message, 
                        e.StackTrace, 
                        "ERROR 001: See InGameScreen.cs Update()...\r"
                  + " \t Possible errors are: \r" 
                  + " \t Too many enemies,\r"
                  + " \t acceleration too high,\r"
                  + " \t stupid mistake by Michael\r"        
                        );
                    //stupid format for readability
                    */
                    throw;
                }
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
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.A))
                pointerPosition.X -= 4;
            if (keyState.IsKeyDown(Keys.D))
                pointerPosition.X += 4;
            if (keyState.IsKeyDown(Keys.W))
                pointerPosition.Y -= 4;
            if (keyState.IsKeyDown(Keys.S))
                pointerPosition.Y += 4;

            player.Update(pointerPosition);
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(scoreDisplay, "" + playerScore, new Vector2(10, 650), Color.Black);

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
            player.Draw(spriteBatch, pointerPosition, pointerPosition);
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
    class PauseScreen : BaseScreen
    {
        SpriteFont buttonFont;
        Texture2D buttonSheet;

        private Button btnUnpause;
        Vector2 pauseButtonPos;

        private Button btnExit;
        Vector2 exitButtonPos;

        private Button btnFullscreen;
        Vector2 fullButtonPos;

        public override void LoadContent()
        {

            base.LoadContent();

            buttonFont = content.Load<SpriteFont>("buttonFont");
            buttonSheet = content.Load<Texture2D>("buttonSheet");

            btnUnpause = new Button();
            pauseButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - buttonSheet.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 2);
            btnExit = new Button();
            exitButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - buttonSheet.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 2 - 128);
            btnFullscreen = new Button();
            fullButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - buttonSheet.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 2 + 64 - 128);

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
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
                pauseButtonPos,
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
    }
}
