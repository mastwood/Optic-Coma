using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Serialization;
namespace Optic_Coma
{
    public class ScreenManager
    {
        private KeyboardState oldState;
        ///Create a new instance of screen manager and called it, incidentally, "instance".
        ///This instance can not be changed or redefined by other classes, but may be used
        private static ScreenManager instance;

        public Vector2 Dimensions { private set; get; }
        //Private set public get means that it can only be changed in this class, but other classes can recive the information stored

        public ContentManager Content { private set; get; }

        XmlManager<BaseScreen> xmlBaseScreenManager;

        public BaseScreen currentScreen { set; get; }

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

            xmlBaseScreenManager = new XmlManager<BaseScreen>();
            xmlBaseScreenManager.Type = currentScreen.Type;
            if (currentScreen is InGameScreen)
                currentScreen = xmlBaseScreenManager.Load("Load/InGameScreen.xml");

        }
        public void LoadContent(ContentManager Content)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
            //instantiate content, then tell it where the content is stored as the 2nd parameter, as well as the pipeline to load the content, called "ServiceProvider"
            currentScreen.LoadContent();
            //Load whatever content is on the current screen into the window
        }
        public void UnloadContent()
        {
            currentScreen.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();

            if (oldState.IsKeyUp(Keys.Q) && newState.IsKeyDown(Keys.Q))
            {
                // this will only be called when the key is first pressed
                MenuKey_OnPress();
            }

            oldState = newState;

            currentScreen.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
        }

        public void MenuKey_OnPress()
        {
            if (currentScreen is MenuScreen)
            {
                currentScreen.UnloadContent();
                currentScreen = new InGameScreen();
                currentScreen.LoadContent();
            }
            else if (currentScreen is InGameScreen)
            {
                currentScreen.UnloadContent();
                currentScreen = new MenuScreen();
                currentScreen.LoadContent();
            }
        }
        public void PauseKey_OnPress()
        {
            if (currentScreen is InGameScreen)
            {
                currentScreen.UnloadContent();
                currentScreen = new PauseScreen();
                currentScreen.LoadContent();
            }
            else if (currentScreen is PauseScreen)
            {
                currentScreen.UnloadContent();
                currentScreen = new InGameScreen();
                currentScreen.LoadContent();
            }
        }
        public void ChangeScreenMode()
        {
            if (Foundation.isFullScreen)
            {
                Foundation.isFullScreen = false;
            }
            else
            {
                Foundation.isFullScreen = true;
            }
            Foundation.graphics.ApplyChanges();
        }
        public Foundation foundation;
        public void ExitKey_OnPress()
        {
            
        }
    }
}

