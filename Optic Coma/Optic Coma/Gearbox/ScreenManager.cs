using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Serialization;
using Penumbra;
using System.Collections.Generic;

namespace OpticComa_Main
{
    
    public class ScreenManager
    { 
        private KeyboardState _oldState;

        public Vector2 Dimensions { private set; get; }

        public ContentManager Content { private set; get; }

        public BaseScreen CurrentScreen { set; get; }

        public List<Level> LevelsLoaded;

        public ScreenManager()
        {
            //Size of screen where Dimensions.X and Dimensions.Y are variables that can be used but must be converted to integers to be used
            Dimensions = new Vector2(1024, 800);

            //Changes the screen to the splash screen upon start up
            CurrentScreen = new MenuScreen();
        }
        public void LoadContent(ContentManager content, PenumbraComponent lightingEngine)
        {
            Content = new ContentManager(content.ServiceProvider, "Content");
            //instantiate content, then tell it where the content is stored as the 2nd parameter, as well as the pipeline to load the content, called "ServiceProvider"

            //LevelsLoaded = LevelReadWriter.Read(new string[] { "level1.xml" });

            if (CurrentScreen is MenuScreen)
            {
                CurrentScreen.LoadContent();
            }
            else if (CurrentScreen is Level1Screen)
            {
                CurrentScreen.LoadContent();
            }
            //Load whatever content is on the current screen into the window
        }

        public void UnloadContent()
        {
            CurrentScreen.UnloadContent();
        }
        public void Update(GameTime gameTime, PenumbraComponent lightingEngine)
        {
            KeyboardState newState = Keyboard.GetState();

            if (_oldState.IsKeyUp(Keys.Q) && newState.IsKeyDown(Keys.Q))
            {
                // this will only be called when the key is first pressed
                MenuKey_OnPress();
            }

            _oldState = newState;

            if (CurrentScreen is Level1Screen)
                CurrentScreen.Update(gameTime);
            else if (CurrentScreen is MenuScreen)
                CurrentScreen.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gT, PenumbraComponent lightingEngine)
        {
            if (CurrentScreen is Level1Screen)
            {
                CurrentScreen.Draw(spriteBatch, gT);
            }
            else if (CurrentScreen is MenuScreen)
                CurrentScreen.Draw(spriteBatch, gT);
        }

        public void MenuKey_OnPress()
        {
            if (CurrentScreen is MenuScreen)
            {
                CurrentScreen.UnloadContent();
                CurrentScreen = new Level1Screen();
                CurrentScreen.LoadContent();
            }
            else if (CurrentScreen is Level1Screen)
            {
                
                CurrentScreen.UnloadContent();
                CurrentScreen = new MenuScreen();
                CurrentScreen.LoadContent();
            }
        }
        public void PauseKey_OnPress()
        {
            if (CurrentScreen is Level1Screen)
            {
                Level1Screen level1Screen = (Level1Screen)CurrentScreen;
                if (!level1Screen.IsPaused)
                {
                    SaveFileSerializer.Save(Foundation.SaveReaderWriter, level1Screen.DataToSave);
                    level1Screen.IsPaused = true;
                }
                else
                    level1Screen.IsPaused = false;
            }

        }
        public void ChangeScreenMode()
        {
            if (Foundation.IsFullScreen)
            {
                Foundation.IsFullScreen = false;
                Foundation.GlobalGraphicsDeviceManager.ApplyChanges();
            }
            else
            {
                Foundation.IsFullScreen = true;
                Foundation.GlobalGraphicsDeviceManager.ApplyChanges();
            }
        }
        public Foundation Foundation;
        public void ExitKey_OnPress()
        {
            
        }
    }
}

