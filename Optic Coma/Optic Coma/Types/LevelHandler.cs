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

    public class EnemySpawner
    {
        Queue<List<Enemy>> enemies;
        EnemySpawner(Queue<List<Enemy>> e) //queue is used so that enemies can be loaded in first-to-last and then taken out first-to-last
        {
            enemies = e;
        }

        List<Enemy> currentlySpawning;
        public void SpawnNextEnemies()
        {
            currentlySpawning = enemies.Dequeue();
            foreach (Enemy e in currentlySpawning)
            {
                if (!e.Spawned)
                    e.Initialize();
            }
        }
    } //TODO: Implement this into level editor
    public class TexLoc
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
    public class LevelDataHandler
    {
        string[] sToParse;
        float[][][] fToParse;
        bool[][][] bToParse;
        bool[][] wToParse;

        public string LevelName;
        public string BGImageFilePath;
        public string BGTexMapFilePath, MGTexMapFilePath, FGTexMapFilePath;
        public string OtherImagesFilePath, PlayerTexturePath;

        public List<Vector2> BGTileCoords;
        public List<Vector2> MGTileCoords;
        public List<Vector2> FGTileCoords;
        public List<Vector2> OtherImageCoords;
        public List<TexLoc> BGTexLocList;
        public List<TexLoc> MGTexLocList;
        public List<TexLoc> FGTexLocList;

        public List<Vector2> walkableTiles;

        public LevelDataHandler(string[] strings, float[][][] floats, bool[][][] textureBools, bool[][] walkBools)
        {
            bToParse = textureBools;
            fToParse = floats;
            sToParse = strings;
            wToParse = walkBools;
        }
        public void ParseData()
        {
            LevelName = sToParse[0];
            BGTexMapFilePath = sToParse[1];
            MGTexMapFilePath = sToParse[2];
            FGTexMapFilePath = sToParse[3];
            BGImageFilePath = sToParse[4];
            OtherImagesFilePath = sToParse[5];
            PlayerTexturePath = sToParse[6];

            for (int i = 0; i < fToParse.GetUpperBound(0); i++)
            {
                for (int j = 0; j < fToParse.GetUpperBound(1); j++)
                {
                    if (bToParse[i][j][0])
                    {
                        BGTexLocList.Add(new TexLoc(BGTexMapFilePath, new Vector2(i * 32, j * 32), new Vector2(fToParse[i][j][0], fToParse[i][j][1])));
                    }
                    if (bToParse[i][j][1])
                    {
                        MGTexLocList.Add(new TexLoc(MGTexMapFilePath, new Vector2(i * 32, j * 32), new Vector2(fToParse[i][j][0], fToParse[i][j][1])));
                    }
                    if (bToParse[i][j][2])
                    {
                        FGTexLocList.Add(new TexLoc(FGTexMapFilePath, new Vector2(i * 32, j * 32), new Vector2(fToParse[i][j][0], fToParse[i][j][1])));
                    }
                    if (wToParse[i][j])
                    {
                        walkableTiles.Add(new Vector2(i * 32, j * 32));
                    }
                }
            }

            Level l;
            LevelHandler.InitializeMethods(out l, this);
            ScreenManager.Instance.PassLevel(l);
        }
        

        public List<Vector2> WalkTiles;
    }
    public class Level
    {
        public Player Player;

        public LevelHandler Handler;
        public Level()
        {
            Handler = new LevelHandler(Loader, false);
        }
        public WorkerAction Loader;
        public Action<LevelHandler> LoadContent;
        public Action UnloadContent;
        public Action<GameTime> Update;
        public Action<SpriteBatch, GameTime> Draw;
    }
    public class LevelReadWriter
    {
        public LevelReadWriter()
        {
        }
        public static LevelDataHandler Load(string strFilePath, string boolFilePath, string intFilePath, string boolWFilePath)
        {
            ///Structure:
            ///rS[0] = LevelName
            ///rS[1] BG texturemap filepath (16x16 map)
            ///rS[2] MG texturemap filepath (16x16 map)
            ///rS[3] FG texturemap filepath (16x16 map)
            ///rS[4] Background pic
            ///rs[5] other textures map
            ///rB[x][y][0] = is there a background tile at this x,y coord?
            ///rB[x][y][1] = is there a midground...?
            ///rB......[2] = foreground?
            ///rI[x][y][0 or 1 or 2] = which texture is it?
            ///rV = playerinitpos
            ///rVv[x][y] = enemy[wave][initpos]
            XmlSerializer[] xml = new XmlSerializer[4];
            xml[0] = new XmlSerializer(typeof(string[]));
            xml[1] = new XmlSerializer(typeof(bool[][][]));
            xml[2] = new XmlSerializer(typeof(float[][][]));
            xml[3] = new XmlSerializer(typeof(bool[][]));
            string[] recievedStrings = { };
            float[][][] recievedFloats = { };
            bool[][][] recievedBools = { };
            bool[][] recievedWBools = { };
            Vector2 playerInitPos;

            try
            {
                using (var f = new FileStream(strFilePath, FileMode.Open))
                    recievedStrings = (string[])xml[0].Deserialize(f);
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }
              
            try
            {
                using (var f = new FileStream(intFilePath, FileMode.Open))
                    recievedFloats = (float[][][])xml[2].Deserialize(f);
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }

            try
            {
                using (var f = new FileStream(boolFilePath, FileMode.Open))
                    recievedBools = (bool[][][])xml[1].Deserialize(f);
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }

            try
            {
                using (var f = new FileStream(boolWFilePath, FileMode.Open))
                    recievedWBools = (bool[][])xml[3].Deserialize(f);
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }
            
            return new LevelDataHandler(recievedStrings, recievedFloats, recievedBools, recievedWBools);
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
            if (SuccessCode > 0)
                return true;
            else
                return false;
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
        public static void InitializeMethods(out Level level, LevelDataHandler data)
        {
            level = new Level();
            level.Loader += (object sender, DoWorkEventArgs e) => //lambda operator "=>" lets me add content to methods
            {
                foreach (TexLoc t in data.BGTexLocList)
                {
                    BaseScreen.BaseScreenContent.Load<Texture2D>(t.TexturePath);
                }
                foreach (TexLoc t in data.MGTexLocList)
                {
                    BaseScreen.BaseScreenContent.Load<Texture2D>(t.TexturePath);
                }
                foreach (TexLoc t in data.FGTexLocList)
                {
                    BaseScreen.BaseScreenContent.Load<Texture2D>(t.TexturePath);
                }
                
            }; //this is why we need async loading
        }
    }
}
