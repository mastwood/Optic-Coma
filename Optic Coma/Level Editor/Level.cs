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
using System.Runtime.InteropServices;

namespace Level_Editor
{
    public class Level
    {
        public List<string> DependantTextures; //load textures from this list of strings
        public TileGrid fTileGrid;
        public TileGrid mTileGrid;
        public TileGrid bTileGrid;
        public TileGrid currentTileGrid;
        public Size TileGridDimensions;
        public int LevelNumber; //which level is it

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
            if (layer != LayerMode.Combined)
            {
                foreach (Tile p in currentTileGrid.Tiles)
                {
                    p.ShowGridLines = gridlines;
                }
                currentTileGrid.Draw(f.CreateGraphics(), panOffset);
            }
            else
            {
                //TODO: Combine all layers
            }
        }
    }
}
