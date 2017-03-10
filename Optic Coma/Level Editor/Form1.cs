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
    public partial class frmMain : Form
    {
        public Level DefaultLevel;
        public bool ShowGridLines = true;
        public Level CurrentLevel;
        public LayerMode CurrentLayer = LayerMode.Midground;
        public Tool CurrentTool = Tool.Pan;
        public Point PanOffset;
        BackgroundWorker bW;
        public List<Image> ImageResources = new List<Image>();
        public List<string> ImageResourcesPaths = new List<string>();
        public XmlSerializer xml;
        public BufferedPanel TilePanel = new BufferedPanel();
        public Image ImageToPaint = Properties.Resources.defaultTileImage;
        public frmMain()
        {
            InitializeComponent();
        }
        private TileGrid[] LoadAsync(ref BackgroundWorker w)
        {
            List<Tile>
                f = new List<Tile>(),
                m = new List<Tile>(),
                b = new List<Tile>();

            for (int j = 0; j < 40; j++)
            {
                for (int i = 0; i < 40; i++)
                {
                    Tile p = new Tile(new Point(i * 32, j * 32), Properties.Resources.defaultTileImage);

                    m.Add(p);
                    f.Add(p);
                    b.Add(p);
                }
                w.ReportProgress((j * 2));
            }
            TileGrid[] r = { new TileGrid(f), new TileGrid(m), new TileGrid(b) };
            return r;
        }
        private void bW_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            for (int i = 1; i <= 10; i++)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    TileGrid[] a = LoadAsync(ref worker);
                    DefaultLevel = new Level(new Size(320 * 4, 320 * 4), 0, a[0], a[1], a[2]);
                    
                    using (var f = new FileStream("recentpaths", FileMode.Open))
                    {
                        try
                        {
                            xml = new XmlSerializer(typeof(string));
                            openRecentToolStripMenuItem.DropDownItems.Add((string)xml.Deserialize(f));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }
          
        private void bW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            levelLoadProgress.Value = levelLoadProgress.Maximum * (e.ProgressPercentage / 100);
        }
        private void bW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CurrentLevel = DefaultLevel;
            levelLoadProgress.Value = 0;
            CurrentLevel.Display(ref TilePanel, LayerMode.Midground, ShowGridLines, PanOffset);
            TilePanel.Update();
            hScrollBarLevel.Maximum = (CurrentLevel.TileGridDimensions.Width) - TilePanel.Width;
            vScrollBarLevel.Maximum = (CurrentLevel.TileGridDimensions.Height) - TilePanel.Height;
            hScrollBarLevel.Enabled = true;
            vScrollBarLevel.Enabled = true;
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            ActiveForm.StartPosition = FormStartPosition.CenterScreen;
            TilePanel.Size = tilePanel.Size; TilePanel.Location = tilePanel.Location; TilePanel.Anchor = AnchorStyles.Right;
            newLevel.Controls.Add(TilePanel);
            newLevel.Controls.Remove(tilePanel);

            bW = new BackgroundWorker();
            bW.WorkerReportsProgress = true;
            bW.WorkerSupportsCancellation = true;
            bW.DoWork += new DoWorkEventHandler(bW_DoWork);
            bW.ProgressChanged += new ProgressChangedEventHandler(bW_ProgressChanged);
            bW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bW_RunWorkerCompleted);
            bW.RunWorkerAsync();
        }

        private void showGridlinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowGridLines)
            {
                ShowGridLines = false;
                CurrentLevel.Display(ref TilePanel, CurrentLayer, ShowGridLines, PanOffset);
                TilePanel.Update();
            }
            else
            {
                ShowGridLines = true;
                CurrentLevel.Display(ref TilePanel, CurrentLayer, ShowGridLines, PanOffset);
                TilePanel.Update();
            }
        }

        private void UpdateImageResourceToolBar()
        {
            foreach(Image i in ImageResources)
            {
                panelResources.Controls.Add(new PictureBox() { Size = new Size(32, 32), Image = i, SizeMode = PictureBoxSizeMode.StretchImage });
            }
            panelResources.Update();
        }

        private void vScrollBarLevel_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollEventArgs s = e as ScrollEventArgs;
            PanOffset.Y = -s.NewValue;
            lblScrollDebug.Text = string.Format("X: {0}, Y: {1} ", PanOffset.X, PanOffset.Y);
            CurrentLevel.Display(ref TilePanel, CurrentLayer, ShowGridLines, PanOffset);
            TilePanel.Update();
        }

        private void hScrollBarLevel_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollEventArgs s = e as ScrollEventArgs;
            PanOffset.X = -s.NewValue;
            lblScrollDebug.Text = string.Format("X: {0}, Y: {1} ", PanOffset.X, PanOffset.Y);
            CurrentLevel.Display(ref TilePanel, CurrentLayer, ShowGridLines, PanOffset);
            TilePanel.Update();
        }

        private void textureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialogImages.ShowDialog();
        }

        private void openFileDialogImages_FileOk(object sender, CancelEventArgs e)
        {
            ImageResources.Add(Image.FromFile(openFileDialogImages.FileName));
            UpdateImageResourceToolBar();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult r = MessageBox.Show("Save?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if(r == DialogResult.Yes)
            {
                Save("saveFile");
            }
        }

        public void Save(string path)
        {
            using (var f = new FileStream(path, FileMode.Create))
            {
                xml = new XmlSerializer(typeof(string));
                if(ImageResourcesPaths.Count > 0)
                    xml.Serialize(f, ImageResourcesPaths);
                xml = new XmlSerializer(typeof(Bitmap));
                if(CurrentLevel.fTileGrid.GetComposedImage() != null)
                    xml.Serialize(f, CurrentLevel.fTileGrid.GetComposedImage());
                if (CurrentLevel.mTileGrid.GetComposedImage() != null)
                    xml.Serialize(f, CurrentLevel.mTileGrid.GetComposedImage());
                if (CurrentLevel.bTileGrid.GetComposedImage() != null)
                    xml.Serialize(f, CurrentLevel.bTileGrid.GetComposedImage());
                //TODO: Enemy spawners, npc, etc
            }
        }

        private void openFileDialogLevels_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void rdoToolPainter_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoToolPainter.Checked) CurrentTool = Tool.Draw;
        }

        private void panelResources_MouseClick(object sender, MouseEventArgs e)
        {
            if (panelResources.Controls.Count > 0)
            {
                foreach (PictureBox c in panelResources.Controls)
                {
                    if (c.Bounds.Contains(e.Location))
                    {
                        ImageToPaint = c.Image;
                    }
                }
            }
        }
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }
        private void tilePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentTool == Tool.Draw)
            {
                tilePanel.Cursor = CreateCursor((Bitmap)ImageToPaint, 0, 0);
                Rectangle b = new Rectangle
                (
                    e.Location,
                    ImageToPaint.Size
                );
                tilePanel.Cursor.Draw(CreateGraphics(), b);
                
            }
        }
    }
    public class Tile
    {
        public Point Location;
        public Image Texture;
        public bool ShowGridLines;
        public Tile(Point l, Image i)
        {
            Location = l;
            Texture = i;
        }
        public void SetLocation(Point l)
        {
            Location = l;
        }
        public void SetTexture(Image i)
        {
            Texture = i;
        }
        public void Draw(Graphics g)
        {
            g.DrawImage(Texture, Location);
            if (ShowGridLines)
            {
                GraphicsUnit f = GraphicsUnit.Pixel;
                PointF[] points = {
                    new PointF(Texture.GetBounds(ref f).Top, Texture.GetBounds(ref f).Left),
                    new PointF(Texture.GetBounds(ref f).Top, Texture.GetBounds(ref f).Right),
                    new PointF(Texture.GetBounds(ref f).Bottom, Texture.GetBounds(ref f).Right),
                    new PointF(Texture.GetBounds(ref f).Bottom, Texture.GetBounds(ref f).Left)
                    };
                g.DrawLines(Pens.Black, points);
            }
        }
    }
    public class TileGrid
    {
        public List<Tile> Tiles;
        Bitmap ComposedImage;
        public TileGrid(List<Tile> t)
        {
            Tiles = t;
        }
        public void Composite()
        {
            int Width = 0, Height = 0;
            foreach (Tile t in Tiles)
            {
                if (t.Location.X > Width) Width = t.Location.X;
                if (t.Location.Y > Height) Height = t.Location.Y;
            }
            Bitmap b = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics canvas = Graphics.FromImage(b))
            {
                canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                foreach(Tile j in Tiles)
                {
                    canvas.DrawImage(j.Texture, j.Location);
                }
            }
            ComposedImage = b;
        }
        public Image GetComposedImage()
        {
            Composite();
            return ComposedImage;
        }
        public void Draw(Graphics g, Point PanOffset)
        {
            g.DrawImage(ComposedImage, PanOffset);
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
                currentTileGrid.Composite();
                currentTileGrid.Draw(f.CreateGraphics(), panOffset);
            }
            else
            {
                //TODO: Combine all layers
            }
        }
    }
    public class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            DoubleBuffered = true;
        }
    }
}
