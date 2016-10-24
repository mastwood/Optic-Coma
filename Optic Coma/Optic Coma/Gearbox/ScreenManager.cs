using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Penumbra;

namespace Optic_Coma
{
    public class ScreenManager
    {
        public float TileLayer = 0.9f, ButtonLayer = 0.1f, ButtonTextLayer = 0.05f, BGLayer = 1f, MGLayer = 0.5f, EntityLayer = 0.6f, FlashlightLayer = 0.61f, FGLayer = 0.4f; 

        private KeyboardState oldState;
        ///Create a new instance of screen manager and called it, incidentally, "instance".
        ///This instance can not be changed or redefined by other classes, but may be used
        private static ScreenManager instance;

        public Vector2 Dimensions { private set; get; }
        //Private set public get means that it can only be changed in this class, but other classes can recive the information stored

        public ContentManager Content { private set; get; }


        public BaseScreen currentScreen { set; get; }

        //Oh boy it's a singleton
        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ScreenManager();
                return instance;
            }
        }

        private ScreenManager()
        //If you remember from the main class, the constructor gives the screenmanager class its "default" settings on startup
        {
            //Size of screen where Dimensions.X and Dimensions.Y are variables that can be used but must be converted to integers to be used
            Dimensions = new Vector2(1024, 800);

            //Changes the screen to the splash screen upon start up
            currentScreen = new MenuScreen();

        }
        public void LoadContent(ContentManager Content, PenumbraComponent lightingEngine)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
            //instantiate content, then tell it where the content is stored as the 2nd parameter, as well as the pipeline to load the content, called "ServiceProvider"
            if(currentScreen is MenuScreen)
                currentScreen.LoadContent();
            else if(currentScreen is Level1Screen)
                currentScreen.LoadContent();
            //Load whatever content is on the current screen into the window
        }
        public void UnloadContent()
        {
            currentScreen.UnloadContent();
        }
        public void Update(GameTime gameTime, PenumbraComponent lightingEngine)
        {
            KeyboardState newState = Keyboard.GetState();

            if (oldState.IsKeyUp(Keys.Q) && newState.IsKeyDown(Keys.Q))
            {
                // this will only be called when the key is first pressed
                MenuKey_OnPress();
            }

            oldState = newState;

            if (currentScreen is Level1Screen)
                currentScreen.Update(gameTime);
            else if (currentScreen is MenuScreen)
                currentScreen.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gT, PenumbraComponent lightingEngine)
        {
            if (currentScreen is Level1Screen)
            {
                currentScreen.Draw(spriteBatch, gT);
            }
            else if (currentScreen is MenuScreen)
                currentScreen.Draw(spriteBatch);
        }

        public void MenuKey_OnPress()
        {
            if (currentScreen is MenuScreen)
            {
                currentScreen.UnloadContent();
                currentScreen = new Level1Screen();
                currentScreen.LoadContent();
            }
            else if (currentScreen is Level1Screen)
            {
                
                currentScreen.UnloadContent();
                currentScreen = new MenuScreen();
                currentScreen.LoadContent();
            }
        }
        public void PauseKey_OnPress()
        {
            if (currentScreen is Level1Screen)
            {
                Level1Screen level1Screen = (Level1Screen)currentScreen;
                if (!level1Screen.IsPaused)
                    level1Screen.IsPaused = true;
                else
                    level1Screen.IsPaused = false;
            }

        }
        public void ChangeScreenMode()
        {
            if (Foundation.isFullScreen)
            {
                Foundation.isFullScreen = false;
                Foundation.graphics.ApplyChanges();
            }
            else
            {
                Foundation.isFullScreen = true;
                Foundation.graphics.ApplyChanges();
            }
        }
        public Foundation foundation;
        public void ExitKey_OnPress()
        {
            
        }
    }
}

