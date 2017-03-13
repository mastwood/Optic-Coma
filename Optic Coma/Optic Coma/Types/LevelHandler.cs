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
using System.IO;
using Penumbra;

namespace Optic_Coma
{
    using WorkerAction = Action<object, DoWorkEventArgs>; //rename this to prevent typing cus lazy

    public class EnemySpawnerProperties
    {
        [XmlArray] public List<EnemyProperties>[] EnemyWaveArray;
        public int NumWaves;
    }
    public class EnemySpawner
    {
        public EnemySpawnerProperties Properties;

        EnemySpawner(List<Enemy>[] e) //queue is used so that enemies can be loaded in first-to-last and then taken out first-to-last
        {

        }

        [XmlIgnore] List<Enemy> currentlySpawning;
        [XmlIgnore] int WaveNum;
        public void SpawnNextEnemies()
        {

            foreach (Enemy e in currentlySpawning)
            {
                if (!e.Spawned)
                    e.Initialize();
            }
        }
    } //TODO: Implement this into level editor

    public class TexLoc //for easy compilation of this data
    {
        public Vector2 Location;
        public Vector2 TextureMapPos;
        public string TexturePath;
        public Texture2D Texture;
        Vector2 TexMapLoc;
        public TexLoc(string tex, Vector2 loc, Vector2 maploc)
        {
            TexturePath = tex;
            Location = loc;
            TexMapLoc = maploc;
            Texture = BaseScreen.BaseScreenContent.Load<Texture2D>(TexturePath);
        }
    }

    
    public class Level
    {
        string Name;
        int Index;

        public Player Player;
        public WorkerAction ALoader;
        public TileSystem tileSystem;
        public Texture2D spriteSheet;
        public Texture2D backgroundImage;

        public List<PointLight> ambientLights; //deal with lighting later

        public bool HasLoaded;
        LevelHandler Handler;
        public Level(LevelSerializable LS)
        {

        }
        public void LoadContent()
        {
            Handler = new LevelHandler(ALoader, HasLoaded);
            Handler.BeginLoad();
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime)
        {
            HasLoaded = Handler.loaded;   
            if (HasLoaded) 
                Player.Update();
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(HasLoaded)
                Player.Draw(spriteBatch);
        }
    }

    [Serializable] public class LevelSerializable
    {
        public List<string> DependantTextures;
        public List<EnemySpawnerProperties> EnemySpawners;
        
    }

    public class LevelReadWriter
    {
        public LevelReadWriter()
        {
        }
        public static List<Level> Read(string[] path)
        {
            XmlSerializer x = new XmlSerializer(typeof(LevelSerializable));
            List<Level> l = new List<Level>();
            try
            {
                foreach(string s in path) { 
                    using (var f = new FileStream(s, FileMode.Open))
                    {
                        Level level = new Level((LevelSerializable)x.Deserialize(f));
                        l.Add(level);
                    }
                }
                return l;
            }
            catch(FileNotFoundException ex)
            {
                return null;
            }
        }
        
    }
    public class LevelHandler
    {
        public bool loaded;
        public BackgroundWorker worker = new BackgroundWorker();
        int SuccessCode;
        public string PercentProgress = "";
        WorkerAction Action;

        public bool LoadingSuccess() //just in case
        {
            return (SuccessCode > 0);
        }
        public LevelHandler(WorkerAction action, bool checkLoad)
        {
            Action = action;
            loaded = checkLoad;
            worker.DoWork += new DoWorkEventHandler(action);
            worker.ProgressChanged += new ProgressChangedEventHandler(ReportProgress);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Complete);
        }

        public void BeginLoad()
        {
            worker.RunWorkerAsync();
        }
        public void ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            PercentProgress = e.ProgressPercentage.ToString() + "%";
        }
        public void Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.DoWork -= new DoWorkEventHandler(Action);
            worker.RunWorkerCompleted -= Complete;
            worker = null;
            SuccessCode = 1;
            loaded = true;
        }
    }
}
