using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace Level_Editor
{
    public class Level
    {
        public List<string> DependantTexturePaths; //load textures from this list of strings
        public TileGrid fTileGrid; //foreground
        public TileGrid mTileGrid; //midground
        public TileGrid bTileGrid; //background
        public TileGrid currentTileGrid; //one being displayed
        public Size TileGridDimensions; //size of level
        public int LevelNumber; //which level is it (Passed to exporter)

        /// <summary>
        /// Constructor for a level
        /// </summary>
        /// <param name="Level Size (in pixels"></param>
        /// <param name="Level ID"></param>
        /// <param name="Foreground Tiles"></param>
        /// <param name="Midground Tiles"></param>
        /// <param name="Background Tiles"></param>
        public Level(Size gridSize, int place,
            TileGrid f, TileGrid m, TileGrid b)
        {
            fTileGrid = f;
            mTileGrid = m;
            bTileGrid = b;

            TileGridDimensions = gridSize;
            LevelNumber = place;

            currentTileGrid = mTileGrid;
        }
        
        /// <summary>
        /// Initializes serialization by passing the textures loaded by the program into the object
        /// </summary>
        /// <param name="loadedTexturePaths"></param>
        public void SetUpSerialization(List<string> loadedTexturePaths)
        {
            DependantTexturePaths = loadedTexturePaths;
        }


        /// <summary>
        /// Displays level onto screen
        /// </summary>
        /// <param name="f"></param>
        /// <param name="layer"></param>
        /// <param name="gridlines"></param>
        /// <param name="panOffset"></param>
        public void Display(ref BufferedPanel f, LayerMode layer, bool gridlines, Point panOffset)
        {
            f.CreateGraphics().Clear(Color.White);

            if (layer == LayerMode.Foreground)
            {
                currentTileGrid = fTileGrid;
            }
            else if (layer == LayerMode.Midground)
            {
                currentTileGrid = mTileGrid;
            }
            else if (layer == LayerMode.Background)
            {
                currentTileGrid = bTileGrid;
            }
            //------------------------------//
            if (layer != LayerMode.Combined)
            {
                foreach (Tile p in currentTileGrid.Tiles)
                {
                    p.ShowGridLines = gridlines;
                }
                currentTileGrid.LevelEditorDraw(f.CreateGraphics(), panOffset);
            }
            else
            {
                //TODO: Combine all layers??
            }
        }
    }
}
