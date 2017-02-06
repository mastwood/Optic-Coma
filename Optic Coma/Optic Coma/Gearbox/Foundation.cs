using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;
namespace Optic_Coma
{
    [Serializable]
    public struct SaveData
    {
        [XmlElement("Location_X")]
        public static float LocationX { get; set; }

        [XmlElement("Location_Y")]
        public static float LocationY { get; set; }

        [XmlElement("RecentSavePoint")]
        public static float RecentSavePoint { get; set; }
    }
    
    public class Foundation : Game
    {
        public static XmlSerializer SaveReaderWriter = new XmlSerializer(typeof(float[]));

        public static PenumbraComponent LightingEngine;

        public static GraphicsDeviceManager Graphics;
        private SpriteBatch _spriteBatch;

        public static int ScreenWidth;
        public static int ScreenHeight;
        
        public static bool IsFullScreen;

        public string InstallDirectory;
        public Foundation()
        {
            //x = new XmlSerializer(typeof(GameState));
            LightingEngine = new PenumbraComponent(this);

            IsMouseVisible = true;
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            InstallDirectory = Content.RootDirectory;
            Components.Add(LightingEngine);
            LightingEngine.AmbientColor = new Color(30,0,0,255);
            LightingEngine.Debug = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            float[] i = new float[3];

            ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Graphics.PreferredBackBufferWidth = (int)ScreenManager.Instance.Dimensions.X;
            Graphics.PreferredBackBufferHeight = (int)ScreenManager.Instance.Dimensions.Y;
            Graphics.IsFullScreen = false;

            Graphics.ApplyChanges();
            //Send the size to the graphics manager

            if (SaveFileSerializer.Load(SaveReaderWriter) != null)
            {
                i = SaveFileSerializer.Load(SaveReaderWriter);
                SaveData.LocationX = i[0]; SaveData.LocationY = i[1]; SaveData.RecentSavePoint = i[2];
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //Instantiate spriteBatch
            ScreenManager.Instance.LoadContent(Content, LightingEngine);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            ScreenManager.Instance.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (IsFullScreen)
            {
                Graphics.PreferredBackBufferWidth = ScreenWidth;
                Graphics.PreferredBackBufferHeight = ScreenHeight;
                Graphics.IsFullScreen = true;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = (int)ScreenManager.Instance.Dimensions.X;
                Graphics.PreferredBackBufferHeight = (int)ScreenManager.Instance.Dimensions.Y;
                Graphics.IsFullScreen = false;
            }
            ScreenManager.Instance.Update(gameTime, LightingEngine);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.BackToFront);
            ScreenManager.Instance.Draw(_spriteBatch, gameTime, LightingEngine);
            _spriteBatch.End();
        }
    }
    public class FrameCounter
    {
        public FrameCounter()
        {
        }

        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        public const int MaximumSamples = 100;

        private Queue<float> _sampleBuffer = new Queue<float>();

        public bool Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MaximumSamples)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
            return true;
        }
    }  
    public class TextureCutter
    {
        public static Texture2D Cut(Texture2D image, Vector2 sourceLocation, int individualSize)
        {
            Color[] imageData = new Color[image.Width * image.Height];
            image.GetData<Color>(imageData);
            Rectangle sourceRectangle = new Rectangle((int)(sourceLocation.X * individualSize), (int)(sourceLocation.Y * individualSize), individualSize, individualSize);
            Color[] imagePiece = GetImageData(imageData, image.Width, sourceRectangle);
            Texture2D returnable = new Texture2D(Foundation.Graphics.GraphicsDevice, individualSize, individualSize);
            returnable.SetData<Color>(imagePiece);
            return returnable;
        }
        private static Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle)
        {
            Color[] color = new Color[rectangle.Width * rectangle.Height];
            for (int x = 0; x < rectangle.Width; x++)
                for (int y = 0; y < rectangle.Height; y++)
                    color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
            return color;
        }
    }
}
