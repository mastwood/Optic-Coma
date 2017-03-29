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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Level_Editor
{
    public partial class frmMain : Form
    {
        #region Fields

        // States of the actual level being edited and the editor (TAKE THAT OBJECT ORIENTED PROGRAMMERS, WE'RE USING STATE ORIENTED PROGRAMMING)
        public Level DefaultLevel;
        public bool ShowGridLines = true;
        public Level CurrentLevel;
        public LayerMode CurrentLayer = LayerMode.Midground;
        public Tool CurrentTool = Tool.Draw;
        public Point PanOffset;

        // For loading the tile grid asynchronously
        private BackgroundWorker bW;

        // Images used by a level (These are for loading and saving a level)
        public List<Image> ImageResources = new List<Image>();
        public List<string> ImageResourcesPaths = new List<string>();
        public XmlSerializer xml;

        // This holds the level tiles on the screen
        public BufferedPanel TilePanel = new BufferedPanel();

        // For painter tool
        public Image ImageToPaint = Properties.Resources.defaultTileImage;
        public Cursor CurrentGridCursor;
        
        // Objects exported by editor
        public List<OpticComa_Types.EnemySpawnerProperties> Spawners;
        public List<Microsoft.Xna.Framework.Vector2> PointLights;
        public List<OpticComa_Types.TriHitBox> TriHitBoxes;
        public List<OpticComa_Types.RectHitBox> RectHitBoxes;
        public List<OpticComa_Types.EnemySpawnerProperties> EnemySpawners;

        #endregion

        public frmMain()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// This section is mainly for loading the tile grid onto the screen, since it takes a
        /// bit of processing power just to get all of the images loaded
        /// Luckily, async workers as well as some black magic (win32 graphics objects) make this faster
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        #region FormLoad
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
                    try
                    {
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
                    }catch(Exception ex) { }
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
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            TilePanel.Size = tilePanel.Size; TilePanel.Location = tilePanel.Location; TilePanel.Anchor = AnchorStyles.Right;
            TilePanel.MouseMove += new MouseEventHandler(TilePanel_MouseMove);
            TilePanel.Click += new EventHandler(TilePanel_Click);
            newLevel.Controls.Add(TilePanel);
            newLevel.Controls.Remove(tilePanel);
            CurrentGridCursor = new Cursor(((Bitmap)ImageToPaint).GetHicon());

            bW = new BackgroundWorker();
            bW.WorkerReportsProgress = true;
            bW.WorkerSupportsCancellation = true;
            bW.DoWork += new DoWorkEventHandler(bW_DoWork);
            bW.ProgressChanged += new ProgressChangedEventHandler(bW_ProgressChanged);
            bW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bW_RunWorkerCompleted);
            bW.RunWorkerAsync();
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            DialogResult r = MessageBox.Show("Save?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (r == DialogResult.Yes)
            {
                SaveLevel("saveFile");
            }
            */
        }

        #endregion

        ///<summary>
        /// This section is for any methods that can/need to be be used throughout the program without problems
        ///</summary>
        #region Static Methods
        /// <summary>
        /// Shrinks or expands an image into a rectangle
        /// Used for making the little icon that appears beside the mouse in painting mode
        /// </summary>
        /// <param name="img"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Image GetResizedImage(Image img, Rectangle rect)
        {
            Bitmap b = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, rect.Width, rect.Height);
                g.Dispose();
                return (Image)b.Clone();
            }
        }
        #endregion

        /// <summary>
        /// This section is for logic pertaining to moving around the level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region scrollbars
        private void vScrollBarLevel_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollEventArgs s = e as ScrollEventArgs; //used to be necessary, idk if it still is
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
        #endregion

        /// <summary>
        /// This section is for the bar at the top (new, edit, view, etc.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region toolstrip item click events
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelNameInputDialog f = new LevelNameInputDialog();
            f.ShowDialog();
            string s = f.GetLevelName();
            levelTabControl.TabPages.Add(s);
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
        private void textureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialogImages.ShowDialog();
        }

        private void openFileDialogImages_FileOk(object sender, CancelEventArgs e)
        {
            Bitmap i = (Bitmap)Image.FromFile(openFileDialogImages.FileName);
            Bitmap o = (Bitmap)GetResizedImage(i, new Rectangle(0, 0, 32, 32));
            ImageResources.Add(o);
            ImageResourcesPaths.Add(openFileDialogImages.FileName);
            UpdateImageResourceToolBar();
        }
        
        #endregion

        /// <summary>
        /// This section is for saving and loading leveleditor files, not for exporting levels for the game
        /// Allows you to save and continue working on a level later
        /// </summary>
        #region serialization
        public void SaveLevel()
        {
            
        }
        public void ExportLevel()
        {
            XmlSerializer xml = new XmlSerializer(typeof(OpticComa_Types.LevelSerializable));
            OpticComa_Types.LevelSerializable toSerialize = new OpticComa_Types.LevelSerializable();
            toSerialize.Background = "level_" + CurrentLevel.LevelNumber + "_backgroundmap";
            toSerialize.Midground = "level_" + CurrentLevel.LevelNumber + "_midgroundmap";
            toSerialize.Foreground = "level_" + CurrentLevel.LevelNumber + "_foregroundmap";
            toSerialize.PointLightLocations = PointLights;
            toSerialize.TriHitBoxes = TriHitBoxes;
            toSerialize.RectHitBoxes = RectHitBoxes;
            toSerialize.EnemySpawners = EnemySpawners;
            using (var f = new FileStream("level_" + CurrentLevel.LevelNumber + ".lvl", FileMode.Create))
            {
                xml.Serialize(f, toSerialize);
            }
            CurrentLevel.bTileGrid.GetComposedImage().Save("level_" + CurrentLevel.LevelNumber + "_backgroundmapImg");
            CurrentLevel.mTileGrid.GetComposedImage().Save("level_" + CurrentLevel.LevelNumber + "_midgroundmapImg");
            CurrentLevel.fTileGrid.GetComposedImage().Save("level_" + CurrentLevel.LevelNumber + "_foregroundmapImg");
        }
        public void LoadLevel(string path)
        {
            using (var f = new FileStream(path, FileMode.Open))
            {
            }
        }
        private void openFileDialogLevels_FileOk(object sender, CancelEventArgs e)
        {
            string s = openFileDialogLevels.FileName;
            LoadLevel(s);
        }
        #endregion

        /// <summary>
        /// The radio buttons on the top left of the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region toolmenu radio buttons
        private void rdoToolPainter_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoToolPainter.Checked) CurrentTool = Tool.Draw;
        }
        private void rdoToolHitBox_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoToolHitBox.Checked) CurrentTool = Tool.HitBox;
        }
        private void rdoToolHitTri_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoToolHitTri.Checked) CurrentTool = Tool.HitTri;
        }
        private void rdoToolEnemyPlace_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoToolEnemyPlace.Checked) CurrentTool = Tool.EnemySpawner;
        }
        #endregion

        /// <summary>
        /// For logic when actually interacting with the level within the editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region panel events and methods
        private void tilePanel_Click(object sender, EventArgs e)
        {

        }
        private void panelResources_MouseClick(object sender, MouseEventArgs e)
        {
            
        }
        private void UpdateImageResourceToolBar()
        {
            foreach (Image i in ImageResources)
            {
                PictureBox p = new PictureBox()
                {
                    Size = new Size(32, 32),
                    Image = i,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                //lambda operation allows me to add specific stuff to events
                p.MouseClick += (object sender, MouseEventArgs e) =>
                {
                    ImageToPaint = p.Image;
                };
                panelResources.Controls.Add(p);
            }
            panelResources.Update();
        }
        private void TilePanel_MouseMove(object sender, EventArgs e)
        {
            MouseEventArgs k = e as MouseEventArgs;
            if (CurrentTool == Tool.Draw)
            {
                try
                {
                    TilePanel.Cursor = new Cursor(((Bitmap)ImageToPaint).GetHicon());
                }
                catch (Exception ex)
                {
                    ErrorHandler.AppendLog(ex);
                }
            }
        }
        private void TilePanel_Click(object sender, EventArgs e)
        {
            MouseEventArgs m = e as MouseEventArgs;
            if (CurrentTool == Tool.Draw)
            {
                foreach (Tile t in CurrentLevel.currentTileGrid.Tiles)
                {
                    Point loc = new Point(t.Location.X + PanOffset.X, t.Location.Y + PanOffset.Y);
                    Rectangle r = new Rectangle(loc, t.Texture.Size);
                    if (r.Contains(m.Location))
                    {
                        CurrentLevel.currentTileGrid.Tiles.Remove(t);
                        t.SetTexture(ImageToPaint);
                        CurrentLevel.currentTileGrid.Tiles.Add(t);
                        CurrentLevel.currentTileGrid.LevelEditorDraw(TilePanel.CreateGraphics(), PanOffset);
                        TilePanel.Update();
                        return;
                    }
                }
            }
            else if (CurrentTool == Tool.EnemySpawner)
            {

            }
        }
        #endregion

        
    }

    /// <summary>
    /// This object is just a panel which loads more smoothly but slightly less efficiently
    /// </summary>
    public class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            DoubleBuffered = true;
        }
    }
}
