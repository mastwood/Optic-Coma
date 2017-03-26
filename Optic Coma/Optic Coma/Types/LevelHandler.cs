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
using OpticComa_Types;

namespace OpticComa_Main
{
    using WorkerAction = Action<object, DoWorkEventArgs>; //rename this to prevent typing cus lazy

    
    public class EnemySpawner
    {
        public EnemySpawnerProperties Properties;
        public Enemy[] Enemies;
        private int wave = 0;

        EnemySpawner(EnemySpawnerProperties inits) //queue is used so that enemies can be loaded in first-to-last and then taken out first-to-last
        {
            Properties = inits;
            Enemies = new Enemy[inits.EnemyConfigs.ToArray().Length];
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i] = new Enemy(inits.EnemyConfigs[i], inits.Position);
            }
        }
        public void SpawnNext()
        {
            Enemies[wave].Spawned = true;
            wave++;
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


    public class HitboxMap
    {
        private List<IHitBox> hB;

        public HitboxMap(List<IHitBox> h)
        {
            hB = h;
        }
        public HitboxMap()
        {
        }

        public bool Collision(Rectangle hBPlayer)
        {
            foreach (IHitBox h in hB)
                if (h.Contains(hBPlayer)) return true;

            return false;
        }
        public void AddNewHitbox(IHitBox h)
        {
            hB.Add(h);
        }
        public void Update(Vector2 offset)
        {
            foreach(IHitBox h in hB)
            {
                h.Update(offset);
            }
        }
    }
    public class LevelContentReader : ContentTypeReader<LevelSerializable>
    {
        protected override LevelSerializable Read(ContentReader input, LevelSerializable existingInstance)
        {
            return input.ReadObject(existingInstance);
        }
    }
    public class Level
    {
        string Name;
        int Index;

        public Player Player;
        public WorkerAction ALoader;
        public TileSystem tileSystem;
        public Texture2D mapBackground;
        public Texture2D mapMidground;
        public Texture2D mapForeground;
        public Texture2D imgBackground;
        public List<Vector2> pointLightLocations;
        public List<IHitBox> HitBoxes;

        public bool HasLoaded;
        LevelHandler Handler;
        public Level(LevelSerializable LS)
        {
            HitBoxes.AddRange(LS.TriHitBoxes);
            HitBoxes.AddRange(LS.RectHitBoxes);
            ALoader += (object sender, DoWorkEventArgs e) =>
            {
                mapBackground = Foundation.GlobalScreenManager.Content.Load<Texture2D>(LS.Background);
                mapMidground = Foundation.GlobalScreenManager.Content.Load<Texture2D>(LS.Midground);
                mapForeground = Foundation.GlobalScreenManager.Content.Load<Texture2D>(LS.Foreground);
            };
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
