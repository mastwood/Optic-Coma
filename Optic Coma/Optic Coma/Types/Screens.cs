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
        protected ContentManager Content;
        [XmlIgnore]
        public Type Type;

        public BaseScreen()
        {
            Type = this.GetType();
        }

        public virtual void LoadContent()
        {
            Content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
        }
        public virtual void UnloadContent()
        {
            Content.Unload();
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
    }

    internal class LevelScreen : BaseScreen
    {
        public List<Vector2> WalkableTiles = new List<Vector2>();

        private double _lowDist, _curDist;
        public FrameCounter FrameCounter = new FrameCounter();
        public Vector2 LevelSize;

        public static bool NotOutOfBounds(List<Vector2> walkableTiles, List<Triangle> nonWalkableTriangles, Vector2 location, Rectangle playerHitBox)
        {
            List<Rectangle> levelArea = new List<Rectangle>();
            bool b = false;
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
            _lowDist = -1;
            foreach (Enemy enemy in enemies)
            {
                _curDist = Math.Sqrt
                (
                    Math.Pow((Math.Abs(source.X - enemy.CurrentPosition.X)), 2) +
                    Math.Pow((Math.Abs(source.Y - enemy.CurrentPosition.Y)), 2)
                );
                if( _lowDist == -1 || _curDist < _lowDist)
                {
                    _lowDist = _curDist;
                }
            }
            return (float)_lowDist;
        }
        public double LogisticForLight(float x)
        {
            double xD;
            xD = Convert.ToDouble(x);
            return (350 / (1 + Math.Exp(-0.02d*(xD - 200d))));
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

    internal class MenuScreen : BaseScreen
    {
        private Button _btnEnterGame;
        private SpriteFont _buttonFont;
        private Texture2D _enterButtonTexture;
        private Vector2 _enterButtonPos;
        private Texture2D _titleGraphic;
        private Texture2D _bg;
        public override void LoadContent()
        {
            base.LoadContent();
            _titleGraphic = Content.Load<Texture2D>("ocbigSheet");

            _btnEnterGame = new Button();
            _enterButtonTexture = Content.Load<Texture2D>("buttonSheet");
            _buttonFont = Content.Load<SpriteFont>("buttonFont");

            _enterButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - _enterButtonTexture.Width / 2,
                                         ScreenManager.Instance.Dimensions.Y / 2 - _enterButtonTexture.Height / 8);

            _bg = Content.Load<Texture2D>("starsbg");
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
                _bg,
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
                _titleGraphic,
                null,
                new Rectangle((int)Entity.CenterScreen.X - _titleGraphic.Width / 2, 0, _titleGraphic.Width, _titleGraphic.Height/2),
                new Rectangle(0, 0, _titleGraphic.Width, _titleGraphic.Height/2),
                null,
                0,
                null,
                Color.White,
                SpriteEffects.None,
                ScreenManager.Instance.BgLayer-0.01f
            );
            //We're making our button! Woo!
            _btnEnterGame.Draw
            (
                _enterButtonTexture,
                spriteBatch,
                ScreenManager.Instance.MenuKey_OnPress,
                _enterButtonPos,
                _buttonFont,
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

        private Type _type;
        public Level1Screen()
        {
            _type = GetType();
        }

        #region fields

        private Spotlight _testLight;

        private Texture2D _loadingScreen;
        private BackgroundWorker _loader = new BackgroundWorker();
        private volatile bool _hasLoaded; //volatile means that the variable can be used in multiple threads at once

        private Vector2 _mouseLoc;

        public bool IsPaused = false;
        private bool _notOutOfBounds;

        private Texture2D _debugColRect;

        private int _screenWidth = (int)ScreenManager.Instance.Dimensions.X;
        private int _screenHeight = (int)ScreenManager.Instance.Dimensions.Y;

        private float _deltaTime;

        private Random _random = new Random();

        private Player _player;

        private List<Enemy> _enemies = new List<Enemy>();
        private List<Entity> _nonPlayerEntities = new List<Entity>();

        private TileSystem _walkableTileRenderer;

        private Texture2D _lightTexture;
        private Texture2D _flashLightTexture;
        private Texture2D _playerTexture;
        private Texture2D _enemyTexture;
        private Texture2D _floorTexture;

        private Vector2 _tileOffsetLocation;
        private Vector2 _playerPos;
        private Vector2 _enemyPos;
        private string _playerPath = "player";
        private string _enemyPath = "enemy";
        private string _flashPath = "flashlight";

        private Texture2D _buttonSheet;
        private Button _pauseButton;
        private Vector2 _pauseButtonPos;
        private SpriteFont _buttonFont;

        private Button _btnUnpause;
        private Vector2 _unpauseButtonPos;

        private Button _btnExit;
        private Vector2 _exitButtonPos;
        private Texture2D _bg;
        private Button _btnFullscreen;
        private Vector2 _fullButtonPos;

        private SoundEffect _music;
        private SoundEffectInstance _musicInstance;
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

        //this method is called after everything loads
        protected void Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _loader.DoWork -= LoadAsync;
            _loader.RunWorkerCompleted -= Complete;
            _loader = null;
            _hasLoaded = true;
        }
        //this method is called by a background thread while the loading screen is displayed
        protected void LoadAsync(object sender, DoWorkEventArgs e)
        {
            IsPaused = false;

            WalkableTiles = TileSetup();

            _testLight = new Spotlight()
            {
                Position = Entity.CenterScreen,
                Enabled = true,
                CastsShadows = true,
                Scale = new Vector2(900, 900),
                Color = Color.White,
                Intensity = 2,
                ShadowType = ShadowType.Occluded,
            };
            Foundation.LightingEngine.Lights.Add(_testLight);

            LevelSize = new Vector2(ScreenManager.Instance.Dimensions.X * 2, ScreenManager.Instance.Dimensions.Y * 2);

            _debugColRect = Content.Load<Texture2D>("rectbox");
            _floorTexture = Content.Load<Texture2D>("floorSheet");
            _walkableTileRenderer = new TileSystem(_floorTexture, 4, 4, 1, LevelSize, WalkableTiles);

            _music = Content.Load<SoundEffect>("samplemusic");
            _musicInstance = _music.CreateInstance();
            _musicInstance.IsLooped = true;
            _musicInstance.Volume = MusicVolume;
            //musicInstance.Play();

            _bg = Content.Load<Texture2D>("starsbg");

            #region buttons
            _buttonSheet = Content.Load<Texture2D>("buttonSheet");
            _pauseButton = new Button();
            _pauseButtonPos = Vector2.Zero;
            _buttonFont = Content.Load<SpriteFont>("buttonFont");

            _btnUnpause = new Button();
            _unpauseButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - _buttonSheet.Width / 2,
                                           ScreenManager.Instance.Dimensions.Y / 2);
            _btnExit = new Button();
            _exitButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - _buttonSheet.Width / 2,
                                        ScreenManager.Instance.Dimensions.Y / 2 - 128);
            _btnFullscreen = new Button();
            _fullButtonPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - _buttonSheet.Width / 2,
                                        ScreenManager.Instance.Dimensions.Y / 2 + 64 - 128);
            #endregion
            #region entities
            _lightTexture = Content.Load<Texture2D>("light");
            _flashLightTexture = Content.Load<Texture2D>(_flashPath);
            _playerTexture = Content.Load<Texture2D>(_playerPath);
            _enemyTexture = Content.Load<Texture2D>(_enemyPath);

            _playerPos = new Vector2(ScreenManager.Instance.Dimensions.X / 2 - _playerTexture.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 2 - _playerTexture.Height / 8);

            _enemyPos = new Vector2(ScreenManager.Instance.Dimensions.X / 4 - _playerTexture.Width / 2,
                                     ScreenManager.Instance.Dimensions.Y / 4 - _playerTexture.Height / 8);

            _player = new Player(_playerTexture, _playerPos, _flashLightTexture, _lightTexture);

            _enemies.Add(new Enemy(_enemyTexture, _enemyPos));
            _enemies.Add(new Enemy(_enemyTexture, new Vector2(ScreenManager.Instance.Dimensions.X - _enemyPos.X, ScreenManager.Instance.Dimensions.X - _enemyPos.Y)));
            foreach (var enemy in _enemies) _nonPlayerEntities.Add(enemy);
            #endregion

            _tileOffsetLocation = new Vector2(SaveData.LocationX, SaveData.LocationY);

            return;
        }
        
        public override void LoadContent()
        {
            base.LoadContent();
            _loadingScreen = Content.Load<Texture2D>("loadingScreen");
            _hasLoaded = false;

            //"loader" is a "BackgroundWorker", meaning it opens up a thread and performs a method while the program can continue running
            _loader.DoWork += new DoWorkEventHandler(LoadAsync); //makes the action to perform "LoadAsync()"
            _loader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Complete); //makes the action to perform when done "Complete()"
            _loader.RunWorkerAsync(); //runs the worker then continues the game loop (right now just the loading screen until everything is done)
            
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        private KeyboardState _prevState;
        public override void Update(GameTime gameTime) //gametime is a tick
        {
            if (_hasLoaded)
            {
                _deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                FrameCounter.Update(_deltaTime);
                #region  not paused
                if (!IsPaused)
                {
                    Rectangle playerArea = new Rectangle((int)_playerPos.X, (int)_playerPos.Y, 48, 48);

                    int spawnLocationIndicator = _random.Next(0, 3);

                    MouseState curMouse = Mouse.GetState();

                    _mouseLoc = new Vector2(curMouse.X, curMouse.Y);
                    _mouseLoc.X = curMouse.X;
                    _mouseLoc.Y = curMouse.Y;

                    _player.FacingDirection = _mouseLoc - _player.CurrentPosition;

                    // using radians
                    // measure clockwise from left
                    #region player rotation
                    _player.FlashAngle = (float)(Math.Atan2(_player.FacingDirection.Y, _player.FacingDirection.X)) + (float)Math.PI;

                    if ((_player.FlashAngle > 0 && _player.FlashAngle <= Math.PI / 4) || (_player.FlashAngle > Math.PI * 7 / 4 && _player.FlashAngle <= 2 * Math.PI))
                    {
                        _player.PlayerAngle = (float)Math.PI; //Right
                    }
                    else if (_player.FlashAngle > Math.PI / 4 && _player.FlashAngle <= Math.PI * 3 / 4)
                    {
                        _player.PlayerAngle = -(float)Math.PI / 2; //Down
                    }
                    else if (_player.FlashAngle > Math.PI * 3 / 4 && _player.FlashAngle <= Math.PI * 5 / 4)
                    {
                        _player.PlayerAngle = 0f; //Left
                    }
                    else if (_player.FlashAngle > Math.PI * 5 / 4 && _player.FlashAngle <= Math.PI * 7 / 4)
                    {
                        _player.PlayerAngle = (float)Math.PI / 2; //Up
                    }
                    KeyboardState keyState = Keyboard.GetState();
                    if (keyState.IsKeyDown(Keys.R) && keyState != _prevState)
                    {
                        Foundation.LightingEngine.Debug = !Foundation.LightingEngine.Debug;
                    }
                    #endregion
                    _prevState = keyState;
                    #region collision and movement
                    if (keyState.IsKeyDown(Keys.W))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2(_tileOffsetLocation.X, _tileOffsetLocation.Y + (float)(4.25 * Entity.WalkMult((float)Math.PI / 2, _player.FlashAngle, 1, false))), _player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in _nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.Y += (float)(4.25 * Entity.WalkMult((float)Math.PI / 2, _player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            _tileOffsetLocation.Y += (float)(4.25 * Entity.WalkMult((float)Math.PI / 2, _player.FlashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.A))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2(_tileOffsetLocation.X + (float)(4.25 * Entity.WalkMult(0, _player.FlashAngle, 1, false)), _tileOffsetLocation.Y), _player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in _nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.X += (float)(4.25 * Entity.WalkMult(0, _player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            _tileOffsetLocation.X += (float)(4.25 * Entity.WalkMult(0, _player.FlashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.S))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2(_tileOffsetLocation.X, _tileOffsetLocation.Y - (float)(4.25 * Entity.WalkMult(3 * (float)Math.PI / 2, _player.FlashAngle, 1, false))), _player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in _nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.Y -= (float)(4.25 * Entity.WalkMult(3 * (float)Math.PI / 2, _player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            _tileOffsetLocation.Y -= (float)(4.25 * Entity.WalkMult(3 * (float)Math.PI / 2, _player.FlashAngle, 1, false));
                        }
                    }
                    if (keyState.IsKeyDown(Keys.D))
                    {
                        if (NotOutOfBounds(WalkableTiles, null, new Vector2(_tileOffsetLocation.X - (float)(4.25 * Entity.WalkMult((float)Math.PI, _player.FlashAngle, 1, false)), _tileOffsetLocation.Y), _player.Texture.Bounds))
                        {
                            foreach (var nonPlayer in _nonPlayerEntities)
                            {
                                nonPlayer.CurrentPosition.X -= (float)(4.25 * Entity.WalkMult((float)Math.PI, _player.FlashAngle, 1, false));
                                nonPlayer.Hull.Position = nonPlayer.CurrentPosition;
                            }
                            _tileOffsetLocation.X -= (float)(4.25 * Entity.WalkMult((float)Math.PI, _player.FlashAngle, 1, false));
                        }
                    }
                    #endregion

                    foreach (Enemy enemy in _nonPlayerEntities)
                    {
                        enemy.Update();
                    }
                    float dist = GetDistToClosestEnemy(_enemies, _playerPos);
                    float colorVal;
                    if (dist <= 370f && dist >= 100)
                    {
                        colorVal = (float)LogisticForLight(dist);
                        _testLight.Color = new Color(1f, colorVal / 350, colorVal / 350, 1f);
                    }
                    else if (dist < 100f)
                    {
                        colorVal = (float)LogisticForLight(dist);
                        _testLight.Color = new Color(1f, colorVal / 350, colorVal / 350, colorVal / 100);
                    }
                    else
                    {
                        _testLight.Color = Color.White;
                    }
                    _testLight.Rotation = (float)Math.PI + _player.FlashAngle;
                    Foundation.LightingEngine.Hulls.Clear();
                    foreach(Entity e in _nonPlayerEntities)
                    {
                        Foundation.LightingEngine.Hulls.Add(e.Hull);
                    }
                    DataToSave[0] = _tileOffsetLocation.X; DataToSave[1] = _tileOffsetLocation.Y; DataToSave[2] = 0;
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
  
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_hasLoaded)
            {
                var fps = string.Format("FPS: {0}", FrameCounter.AverageFramesPerSecond);

                #region when not paused
                if (!IsPaused)
                {
                    spriteBatch.End();

                    Foundation.LightingEngine.BeginDraw();
                    spriteBatch.Begin(SpriteSortMode.BackToFront);
                    spriteBatch.Draw(_bg, new Rectangle(0, 0, _screenWidth, _screenHeight), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, ScreenManager.Instance.BgLayer);
                    foreach (Enemy enemy in _enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    _player.Draw(spriteBatch, _buttonFont);
                    _walkableTileRenderer.Draw(spriteBatch, _tileOffsetLocation, LevelSize);

                    spriteBatch.End();
                    Foundation.LightingEngine.Draw(gameTime);

                    spriteBatch.Begin(SpriteSortMode.BackToFront);
                    spriteBatch.DrawString(_buttonFont, "Position: " + _tileOffsetLocation.X + "," + _tileOffsetLocation.Y, new Vector2(1, 84), Color.White);
                    spriteBatch.DrawString(_buttonFont, fps, new Vector2(1, 65), Color.White);
                    spriteBatch.DrawString(_buttonFont, "Lighting Debug Enabled?: " + Foundation.LightingEngine.Debug, new Vector2(1, 103), Color.White);
                    spriteBatch.DrawString(_buttonFont, "Distance to Closest Enemy: " + GetDistToClosestEnemy(_enemies, Entity.CenterScreen), new Vector2(1, 123), Color.White);
                    _pauseButton.Draw
                    (
                        _buttonSheet,
                        spriteBatch,
                        ScreenManager.Instance.PauseKey_OnPress,
                        _pauseButtonPos,
                        _buttonFont,
                        "Pause Game",
                        Color.Black
                    );
                }
                #endregion
                #region when paused (contains options menu)
                else
                {
                    _btnExit.Draw
                    (
                        _buttonSheet,
                        spriteBatch,
                        ScreenManager.Instance.ExitKey_OnPress,
                        _exitButtonPos,
                        _buttonFont,
                        "Exit Game",
                        Color.Black
                    );
                    _btnUnpause.Draw
                    (
                        _buttonSheet,
                        spriteBatch,
                        ScreenManager.Instance.PauseKey_OnPress,
                        _unpauseButtonPos,
                        _buttonFont,
                        "Un-Pause Game",
                        Color.Black
                    );
                    _btnFullscreen.Draw
                    (
                        _buttonSheet,
                        spriteBatch,
                        ScreenManager.Instance.ChangeScreenMode,
                        _fullButtonPos,
                        _buttonFont,
                        "   Toggle \nFullscreen",
                        Color.Black
                    );
                }
                #endregion
            }
            else
            {
                spriteBatch.Draw(_loadingScreen, new Rectangle(0, 0, 1024, 800), Color.White);
            }
        }
    }
}
