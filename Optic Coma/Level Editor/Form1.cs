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
        #region Fields
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
        public Cursor CurrentGridCursor;
        #endregion
        public frmMain()
        {
            InitializeComponent();
        }
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
            newLevel.Controls.Add(TilePanel);
            newLevel.Controls.Remove(tilePanel);
            CurrentGridCursor = Win32ExternalInfrastructure.CreateCursor((Bitmap)ImageToPaint, 0, 0);

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
            DialogResult r = MessageBox.Show("Save?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (r == DialogResult.Yes)
            {
                SaveLevel("saveFile");
            }
        }

        #endregion 
        #region scrollbars
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
        #endregion
        #region toolstrip item click events
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
            ImageResources.Add(Image.FromFile(openFileDialogImages.FileName));
            UpdateImageResourceToolBar();
        }
        #endregion
        #region serialization
        public void SaveLevel(string path)
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
        public void LoadLevel(string path)
        {

        }
        private void openFileDialogLevels_FileOk(object sender, CancelEventArgs e)
        {
            string s = openFileDialogLevels.FileName;
            LoadLevel(s);
        }
        #endregion
        #region toolmenu radio buttons
        private void rdoToolPainter_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoToolPainter.Checked) CurrentTool = Tool.Draw;
        }
        #endregion
        #region panel events
        private void panelResources_MouseClick(object sender, MouseEventArgs e)
        {
            if (panelResources.Controls.Count > 0)
            {
                foreach (PictureBox c in panelResources.Controls)
                {
                    if (new Rectangle(c.Location, c.Size).Contains(e.Location))
                    {
                        ImageToPaint = c.Image;
                    }
                }
            }
        }
        private void UpdateImageResourceToolBar()
        {
            foreach (Image i in ImageResources)
            {
                panelResources.Controls.Add
                (
                    new PictureBox()
                    {
                        Size = new Size(32, 32),
                        Image = i,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    }
                );
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
                    if (!Win32ExternalInfrastructure.CreateCursor((Bitmap)ImageToPaint, 0, 0).Equals(CurrentGridCursor))
                    {
                        TilePanel.Cursor = Win32ExternalInfrastructure.CreateCursor((Bitmap)ImageToPaint, 0, 0);
                    }
                    Rectangle b = new Rectangle
                        (
                            k.Location,
                            ImageToPaint.Size
                        );
                    TilePanel.Cursor.Draw(CreateGraphics(), b);
                }
                catch (Exception ex)
                {
                    ErrorHandler.AppendLog(ex);
                }
            }
        }
        #endregion

    }


    public class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            DoubleBuffered = true;
        }
    }
}
