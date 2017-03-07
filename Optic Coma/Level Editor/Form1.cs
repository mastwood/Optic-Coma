using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Level_Editor
{
    public partial class frmMain : Form
    {
        public Dictionary<string, Image> LoadedImages;
        public Level DefaultLevel;
        public bool ShowGridLines = true;
        public Level CurrentLevel;
        public LayerMode CurrentLayer = LayerMode.Midground;
        public Tool CurrentTool = Tool.Pan;
        public Point PanOffset;

        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            ActiveForm.StartPosition = FormStartPosition.CenterScreen;

            List<DisplayablePicBox> 
                f = new List<DisplayablePicBox>(), 
                m = new List<DisplayablePicBox>(), 
                b = new List<DisplayablePicBox>();
            int i = 0;
            int j = 0;
            while (i < 100)
            {
                while (j < 100)
                {
                    m.Add(new DisplayablePicBox(new PictureBox() { Location = new Point(i * 32, j * 32) }));
                    f.Add(new DisplayablePicBox(new PictureBox() { Location = new Point(i * 32, j * 32) }));
                    b.Add(new DisplayablePicBox(new PictureBox() { Location = new Point(i * 32, j * 32) }));
                    j++;
                }
                i++;
            }
            DefaultLevel = new Level(new Size(3200, 3200), 0, f ,m ,b);

            CurrentLevel = DefaultLevel;

            CurrentLevel.Display(ref tilePanel, LayerMode.Midground, ShowGridLines, PanOffset);
            tilePanel.Update();
            hScrollBarLevel.Maximum = (CurrentLevel.TileGridDimensions.Width) - tilePanel.Width;
            vScrollBarLevel.Maximum = (CurrentLevel.TileGridDimensions.Height) - tilePanel.Height;
        }
        private void frmMain_Resize(object sender, EventArgs e)
        {

        }

        private void showGridlinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowGridLines)
            {
                ShowGridLines = false;
                CurrentLevel.Display(ref tilePanel, CurrentLayer, ShowGridLines, PanOffset);
                tilePanel.Update();
            }
            else
            {
                ShowGridLines = true;
                CurrentLevel.Display(ref tilePanel, CurrentLayer, ShowGridLines, PanOffset);
                tilePanel.Update();
            }
        }

        private void vScrollBarLevel_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollEventArgs s = e as ScrollEventArgs;
            PanOffset.Y = s.NewValue;
            CurrentLevel.Display(ref tilePanel, CurrentLayer, ShowGridLines, PanOffset);
            tilePanel.Update();
        }

        private void hScrollBarLevel_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollEventArgs s = e as ScrollEventArgs;
            PanOffset.X = s.NewValue;
            lblScrollDebug.Text = string.Format("X: {0}, Y: {1}", PanOffset.X, PanOffset.Y);
            CurrentLevel.Display(ref tilePanel, CurrentLayer, ShowGridLines, PanOffset);
            tilePanel.Update();
        }
    }
    public enum LayerMode
    {
        Foreground,
        Midground,
        Background,
        Combined
    }
    public enum Tool
    {
        Pan,
        Draw,
        Erase,
        Edit,
        SelectBox,
        SelectIndividual
    }
    public class Level
    {
        public List<string> DependantTextures; //load textures from this list of strings
        public List<DisplayablePicBox> fTileGrid = new List<DisplayablePicBox>();
        public List<DisplayablePicBox> mTileGrid = new List<DisplayablePicBox>();
        public List<DisplayablePicBox> bTileGrid = new List<DisplayablePicBox>();
        public List<DisplayablePicBox> currentTileGrid;
        public Size TileGridDimensions;
        public int LevelNumber; //which level is it
        public Point PanOffset = new Point(0,0);

        /// <summary>
        /// Constructor for a level
        /// </summary>
        /// <param name="Level Size (in pixels"></param>
        /// <param name="Level ID"></param>
        /// <param name="Foreground Tiles"></param>
        /// <param name="Midground Tiles"></param>
        /// <param name="Background Tiles"></param>
        public Level(Size gridSize, int place, 
            List<DisplayablePicBox> f, List<DisplayablePicBox> m, List<DisplayablePicBox> b)
        {
            fTileGrid = f;
            mTileGrid = m;
            bTileGrid = b;
            
            TileGridDimensions = gridSize;
            LevelNumber = place;
            foreach (DisplayablePicBox d in fTileGrid)
            {
                d.Box.
                    Size = new Size(32, 32);
                d.Box.
                    Margin = new Padding(0, 0, 0, 0);
                d.Box.
                    BorderStyle = BorderStyle.None;
            }
            foreach (DisplayablePicBox d in mTileGrid)
            {
                d.Box.
                    Size = new Size(32, 32);
                d.Box.
                    Margin = new Padding(0, 0, 0, 0);
                d.Box.
                    BorderStyle = BorderStyle.None;
            }
            foreach (DisplayablePicBox d in bTileGrid)
            {
                d.Box.
                    Size = new Size(32, 32);
                d.Box.
                    Margin = new Padding(0, 0, 0, 0);
                d.Box.
                    BorderStyle = BorderStyle.None;
            }
            currentTileGrid = mTileGrid;
        }
        public void Display(ref Panel f, LayerMode layer, bool gridlines, Point panOffset)
        {
            f.Controls.Clear();

            TileGridDimensions.Width += panOffset.X;
            TileGridDimensions.Height += panOffset.Y;

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
                foreach (DisplayablePicBox p in currentTileGrid)
                {
                    if (gridlines)
                    {
                        p.Box.BorderStyle = BorderStyle.FixedSingle;
                    }
                    else if (!gridlines)
                    {
                        p.Box.BorderStyle = BorderStyle.None;
                    }
                    if (p.IsVisible(TileGridDimensions))
                    {
                        if (panOffset != PanOffset)
                        {
                            p.Box.Location = new Point(p.Box.Location.X + panOffset.X - PanOffset.X,
                                                        p.Box.Location.Y + panOffset.Y - PanOffset.Y);
                            
                        }

                        f.Controls.Add(p.Box);
                    }
                }
                PanOffset = panOffset;
            }
            else
            {
                //TODO: Combine all layers
            }
            TileGridDimensions.Width -= panOffset.X;
            TileGridDimensions.Height -= panOffset.Y;
        }
    }
    public class DisplayablePicBox
    {
        public PictureBox Box { get; set; }
        public DisplayablePicBox(PictureBox box)
        {
            Box = box;
        }
        public bool IsVisible(Size TileGridSize)
        {
            return (TileGridSize.Width > Box.Right || TileGridSize.Height > Box.Bottom);
        }
    }
}
