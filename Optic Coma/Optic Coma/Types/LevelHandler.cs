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
    }
    public class TexLoc
    {
        public Vector2 Location;
        public Vector2 TextureMapPos;
        string TexturePath;
        public Texture2D Texture;
        int mapLoc;
        public TexLoc(string tex, Vector2 loc, int maploc)
        {
            TexturePath = tex;
            Location = loc;
            mapLoc = maploc;
            Texture = BaseScreen.BaseScreenContent.Load<Texture2D>(TexturePath);
            ParseMapLoc();
        }
        private void ParseMapLoc()
        {
            for(int i = 0; i < Texture.Width * Texture.Height; i += 32)
            {
                if(i == mapLoc)
                {
                    TextureMapPos = new Vector2(i / 32, i - (i * 32));
                }
            }
        }
    }
    public class LevelDataHandler
    {
        string[] sToParse;
        int[] iToParse;
        bool[] bToParse;

        public string LevelName;
        public string BGImageFilePath;
        public string BGTexMapFilePath, MGTexMapFilePath, FGTexMapFilePath;
        public string OtherImagesFilePath;

        public List<Vector2> BGTileCoords;
        public List<Vector2> MGTileCoords;
        public List<Vector2> FGTileCoords;
        public List<Vector2> OtherImageCoords;
        public List<TexLoc> BGTexLocList;
        public List<TexLoc> MGTexLocList;
        public List<TexLoc> FGTexLocList;

        public LevelDataHandler(string[] strings, int[] ints, bool[] bools)
        {
            bToParse = bools;
            iToParse = ints;
            sToParse = strings;
        }
        public void ParseData()
        {
            LevelName = sToParse[0];
            BGTexMapFilePath = sToParse[1];
            MGTexMapFilePath = sToParse[2];
            FGTexMapFilePath = sToParse[3];
            BGImageFilePath = sToParse[4];
            OtherImagesFilePath = sToParse[5];

            int row;
            int column;
            for(int i = 0; i < 3072; i++)
            {
                row = (int)Math.Floor(i / 64f);
                column = i - row * i;
                if(i < 1024)
                {
                    if (bToParse[i])
                    {
                        BGTexLocList.Add(new TexLoc(BGTexMapFilePath, new Vector2(row, column), iToParse[i]));
                    }
                }else if(i > 1023 && i < 2048)
                {
                    if (bToParse[i])
                    {
                        MGTexLocList.Add(new TexLoc(MGTexMapFilePath, new Vector2(row, column), iToParse[i]));
                    }
                }
                else if(i > 2047)
                {
                    if (bToParse[i])
                    {
                        FGTexLocList.Add(new TexLoc(FGTexMapFilePath, new Vector2(row, column), iToParse[i]));
                    }
                }
            }
        }

        public List<Vector2> WalkTiles;
    }
    public class Level
    {
        public LevelHandler Handler;
        public Level()
        {
            Handler = new LevelHandler(Loader);
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
        public static LevelDataHandler Load(XmlSerializer[] xml, string strFilePath, string boolFilePath, string intFilePath)
        {
            ///Structure:
            ///rS[0] = LevelName
            ///rS[1] BG texturemap filepath (16x16 map)
            ///rS[2] MG texturemap filepath (16x16 map)
            ///rS[3] FG texturemap filepath (16x16 map)
            ///rS[4] Background pic
            ///rs[5] other textures map
            ///rB[0 to 1023] background tileindicator
            ///rB[1024 to 2047] midground tileindicator
            ///rB[2048 to 3071] foreground tileindicator
            ///rB[0 to 1023] background tileindicator
            ///rB[1024 to 2047] midground tileindicator
            ///rB[2048 to 3071] foreground tileindicator
            string[] recievedStrings = { };
            int[] recievedInts = { };
            bool[] recievedBools = { };
            for (int i = 0; i < xml.Length; i++)
            {
                if (xml.GetType() == typeof(string[]))
                {
                    try
                    {
                        using (var f = new FileStream(strFilePath, FileMode.Open))
                            recievedStrings = (string[])xml[i].Deserialize(f);
                    }
                    catch (FileNotFoundException ex)
                    {
                        return null;
                    }
                }
                if (xml.GetType() == typeof(int[]))
                {
                    try
                    {
                        using (var f = new FileStream(intFilePath, FileMode.Open))
                            recievedInts = (int[])xml[i].Deserialize(f);
                    }
                    catch (FileNotFoundException ex)
                    {
                        return null;
                    }
                }
                if (xml.GetType() == typeof(bool[]))
                {
                    try
                    {
                        using (var f = new FileStream(boolFilePath, FileMode.Open))
                            recievedBools = (bool[])xml[i].Deserialize(f);
                    }
                    catch (FileNotFoundException ex)
                    {
                        return null;
                    }
                }
            }
            return new LevelDataHandler(recievedStrings, recievedInts, recievedBools);
        }
        public void InitializeMethods(out Level level, LevelDataHandler data)
        {
            level = new Level();
            level.Loader += (object sender, DoWorkEventArgs e) => //lambda operator "=>" lets me add content to methods
            {
                foreach(TexLoc t in data.BGTileSprites)
                {
                    BaseScreen.BaseScreenContent.Load<Texture2D>(t.TexturePath);
                }
                foreach (TexLoc t in data.MGTileSprites)
                {
                    BaseScreen.BaseScreenContent.Load<Texture2D>(t.TexturePath);
                }
                foreach (TexLoc t in data.FGTileSprites)
                {
                    BaseScreen.BaseScreenContent.Load<Texture2D>(t.TexturePath);
                }
            }; //this is why we need async loading
        }
    }
    public class LevelHandler
    {
        WorkerAction levelLoadingMethod;
        BackgroundWorker worker;
        DoWorkEventHandler handler;
        int SuccessCode;
        public bool LoadingSuccess() //just in case
        {
            if (SuccessCode > 0)
                return true;
            else
                return false;
        }
        public LevelHandler(WorkerAction action)
        {
            levelLoadingMethod = action;
        }

        public void BeginLoad()
        {
            worker = new BackgroundWorker();
            handler = new DoWorkEventHandler(levelLoadingMethod);
            worker.DoWork += handler;
     
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Complete);
            try
            {
                worker.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                //todo?
            }
        }
        public void Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.DoWork -= handler;
            worker.RunWorkerCompleted -= Complete;
            worker = null;
            SuccessCode = 1;
        }
    }
}
