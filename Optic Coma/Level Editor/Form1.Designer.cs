namespace Level_Editor
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foregroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.midgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spawnersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGridlinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tilePainterToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enemyTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogImages = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.newLevel = new System.Windows.Forms.TabPage();
            this.hScrollBarLevel = new System.Windows.Forms.HScrollBar();
            this.vScrollBarLevel = new System.Windows.Forms.VScrollBar();
            this.lblScrollDebug = new System.Windows.Forms.Label();
            this.tilePanel = new System.Windows.Forms.Panel();
            this.levelTabControl = new System.Windows.Forms.TabControl();
            this.grpTools = new System.Windows.Forms.GroupBox();
            this.rdoToolPainter = new System.Windows.Forms.RadioButton();
            this.levelLoadProgress = new System.Windows.Forms.ProgressBar();
            this.grpResources = new System.Windows.Forms.GroupBox();
            this.panelResources = new System.Windows.Forms.FlowLayoutPanel();
            this.defaultImagePicBoxInFlowChart = new System.Windows.Forms.PictureBox();
            this.openFileDialogLevels = new System.Windows.Forms.OpenFileDialog();
            this.rdoToolHitBox = new System.Windows.Forms.RadioButton();
            this.rdoToolHitTri = new System.Windows.Forms.RadioButton();
            this.rdoToolEnemyPlace = new System.Windows.Forms.RadioButton();
            this.menuStrip.SuspendLayout();
            this.newLevel.SuspendLayout();
            this.levelTabControl.SuspendLayout();
            this.grpTools.SuspendLayout();
            this.grpResources.SuspendLayout();
            this.panelResources.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.defaultImagePicBoxInFlowChart)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.openRecentToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.addToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            resources.ApplyResources(this.newToolStripMenuItem, "newToolStripMenuItem");
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            // 
            // openRecentToolStripMenuItem
            // 
            this.openRecentToolStripMenuItem.Name = "openRecentToolStripMenuItem";
            resources.ApplyResources(this.openRecentToolStripMenuItem, "openRecentToolStripMenuItem");
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textureToolStripMenuItem,
            this.soundToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            // 
            // textureToolStripMenuItem
            // 
            this.textureToolStripMenuItem.Name = "textureToolStripMenuItem";
            resources.ApplyResources(this.textureToolStripMenuItem, "textureToolStripMenuItem");
            this.textureToolStripMenuItem.Click += new System.EventHandler(this.textureToolStripMenuItem_Click);
            // 
            // soundToolStripMenuItem
            // 
            this.soundToolStripMenuItem.Name = "soundToolStripMenuItem";
            resources.ApplyResources(this.soundToolStripMenuItem, "soundToolStripMenuItem");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator2,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator3,
            this.selectAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            resources.ApplyResources(this.undoToolStripMenuItem, "undoToolStripMenuItem");
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            resources.ApplyResources(this.redoToolStripMenuItem, "redoToolStripMenuItem");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.foregroundToolStripMenuItem,
            this.midgroundToolStripMenuItem,
            this.backgroundToolStripMenuItem,
            this.entitiesToolStripMenuItem,
            this.objectsToolStripMenuItem,
            this.spawnersToolStripMenuItem});
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            // 
            // foregroundToolStripMenuItem
            // 
            this.foregroundToolStripMenuItem.Name = "foregroundToolStripMenuItem";
            resources.ApplyResources(this.foregroundToolStripMenuItem, "foregroundToolStripMenuItem");
            // 
            // midgroundToolStripMenuItem
            // 
            this.midgroundToolStripMenuItem.Name = "midgroundToolStripMenuItem";
            resources.ApplyResources(this.midgroundToolStripMenuItem, "midgroundToolStripMenuItem");
            // 
            // backgroundToolStripMenuItem
            // 
            this.backgroundToolStripMenuItem.Name = "backgroundToolStripMenuItem";
            resources.ApplyResources(this.backgroundToolStripMenuItem, "backgroundToolStripMenuItem");
            // 
            // entitiesToolStripMenuItem
            // 
            this.entitiesToolStripMenuItem.Name = "entitiesToolStripMenuItem";
            resources.ApplyResources(this.entitiesToolStripMenuItem, "entitiesToolStripMenuItem");
            // 
            // objectsToolStripMenuItem
            // 
            this.objectsToolStripMenuItem.Name = "objectsToolStripMenuItem";
            resources.ApplyResources(this.objectsToolStripMenuItem, "objectsToolStripMenuItem");
            // 
            // spawnersToolStripMenuItem
            // 
            this.spawnersToolStripMenuItem.Name = "spawnersToolStripMenuItem";
            resources.ApplyResources(this.spawnersToolStripMenuItem, "spawnersToolStripMenuItem");
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGridlinesToolStripMenuItem,
            this.toolbarToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // showGridlinesToolStripMenuItem
            // 
            this.showGridlinesToolStripMenuItem.Name = "showGridlinesToolStripMenuItem";
            resources.ApplyResources(this.showGridlinesToolStripMenuItem, "showGridlinesToolStripMenuItem");
            this.showGridlinesToolStripMenuItem.Click += new System.EventHandler(this.showGridlinesToolStripMenuItem_Click);
            // 
            // toolbarToolStripMenuItem
            // 
            this.toolbarToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tilePainterToolsToolStripMenuItem,
            this.enemyTypesToolStripMenuItem});
            this.toolbarToolStripMenuItem.Name = "toolbarToolStripMenuItem";
            resources.ApplyResources(this.toolbarToolStripMenuItem, "toolbarToolStripMenuItem");
            // 
            // tilePainterToolsToolStripMenuItem
            // 
            this.tilePainterToolsToolStripMenuItem.Name = "tilePainterToolsToolStripMenuItem";
            resources.ApplyResources(this.tilePainterToolsToolStripMenuItem, "tilePainterToolsToolStripMenuItem");
            // 
            // enemyTypesToolStripMenuItem
            // 
            this.enemyTypesToolStripMenuItem.Name = "enemyTypesToolStripMenuItem";
            resources.ApplyResources(this.enemyTypesToolStripMenuItem, "enemyTypesToolStripMenuItem");
            // 
            // openFileDialogImages
            // 
            this.openFileDialogImages.FileName = "openFileDialog1";
            this.openFileDialogImages.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogImages_FileOk);
            // 
            // newLevel
            // 
            this.newLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.newLevel.Controls.Add(this.hScrollBarLevel);
            this.newLevel.Controls.Add(this.vScrollBarLevel);
            this.newLevel.Controls.Add(this.lblScrollDebug);
            this.newLevel.Controls.Add(this.tilePanel);
            resources.ApplyResources(this.newLevel, "newLevel");
            this.newLevel.Name = "newLevel";
            this.newLevel.UseVisualStyleBackColor = true;
            // 
            // hScrollBarLevel
            // 
            resources.ApplyResources(this.hScrollBarLevel, "hScrollBarLevel");
            this.hScrollBarLevel.Name = "hScrollBarLevel";
            this.hScrollBarLevel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBarLevel_Scroll);
            // 
            // vScrollBarLevel
            // 
            resources.ApplyResources(this.vScrollBarLevel, "vScrollBarLevel");
            this.vScrollBarLevel.Name = "vScrollBarLevel";
            this.vScrollBarLevel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarLevel_Scroll);
            // 
            // lblScrollDebug
            // 
            resources.ApplyResources(this.lblScrollDebug, "lblScrollDebug");
            this.lblScrollDebug.Name = "lblScrollDebug";
            // 
            // tilePanel
            // 
            resources.ApplyResources(this.tilePanel, "tilePanel");
            this.tilePanel.Name = "tilePanel";
            this.tilePanel.Click += new System.EventHandler(this.tilePanel_Click);
            // 
            // levelTabControl
            // 
            this.levelTabControl.Controls.Add(this.newLevel);
            resources.ApplyResources(this.levelTabControl, "levelTabControl");
            this.levelTabControl.Multiline = true;
            this.levelTabControl.Name = "levelTabControl";
            this.levelTabControl.SelectedIndex = 0;
            // 
            // grpTools
            // 
            this.grpTools.Controls.Add(this.rdoToolEnemyPlace);
            this.grpTools.Controls.Add(this.rdoToolHitTri);
            this.grpTools.Controls.Add(this.rdoToolHitBox);
            this.grpTools.Controls.Add(this.rdoToolPainter);
            this.grpTools.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.grpTools, "grpTools");
            this.grpTools.Name = "grpTools";
            this.grpTools.TabStop = false;
            // 
            // rdoToolPainter
            // 
            resources.ApplyResources(this.rdoToolPainter, "rdoToolPainter");
            this.rdoToolPainter.Name = "rdoToolPainter";
            this.rdoToolPainter.TabStop = true;
            this.rdoToolPainter.UseVisualStyleBackColor = true;
            this.rdoToolPainter.CheckedChanged += new System.EventHandler(this.rdoToolPainter_CheckedChanged);
            // 
            // levelLoadProgress
            // 
            resources.ApplyResources(this.levelLoadProgress, "levelLoadProgress");
            this.levelLoadProgress.Name = "levelLoadProgress";
            // 
            // grpResources
            // 
            this.grpResources.Controls.Add(this.panelResources);
            resources.ApplyResources(this.grpResources, "grpResources");
            this.grpResources.Name = "grpResources";
            this.grpResources.TabStop = false;
            // 
            // panelResources
            // 
            this.panelResources.Controls.Add(this.defaultImagePicBoxInFlowChart);
            resources.ApplyResources(this.panelResources, "panelResources");
            this.panelResources.Name = "panelResources";
            // 
            // defaultImagePicBoxInFlowChart
            // 
            this.defaultImagePicBoxInFlowChart.InitialImage = global::Level_Editor.Properties.Resources.defaultTileImage;
            resources.ApplyResources(this.defaultImagePicBoxInFlowChart, "defaultImagePicBoxInFlowChart");
            this.defaultImagePicBoxInFlowChart.Name = "defaultImagePicBoxInFlowChart";
            this.defaultImagePicBoxInFlowChart.TabStop = false;
            // 
            // openFileDialogLevels
            // 
            this.openFileDialogLevels.FileName = "openFileDialog1";
            this.openFileDialogLevels.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogLevels_FileOk);
            // 
            // rdoToolHitBox
            // 
            resources.ApplyResources(this.rdoToolHitBox, "rdoToolHitBox");
            this.rdoToolHitBox.Name = "rdoToolHitBox";
            this.rdoToolHitBox.TabStop = true;
            this.rdoToolHitBox.UseVisualStyleBackColor = true;
            // 
            // rdoToolHitTri
            // 
            resources.ApplyResources(this.rdoToolHitTri, "rdoToolHitTri");
            this.rdoToolHitTri.Name = "rdoToolHitTri";
            this.rdoToolHitTri.TabStop = true;
            this.rdoToolHitTri.UseVisualStyleBackColor = true;
            // 
            // rdoToolEnemyPlace
            // 
            resources.ApplyResources(this.rdoToolEnemyPlace, "rdoToolEnemyPlace");
            this.rdoToolEnemyPlace.Name = "rdoToolEnemyPlace";
            this.rdoToolEnemyPlace.TabStop = true;
            this.rdoToolEnemyPlace.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.grpResources);
            this.Controls.Add(this.levelLoadProgress);
            this.Controls.Add(this.grpTools);
            this.Controls.Add(this.levelTabControl);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.newLevel.ResumeLayout(false);
            this.newLevel.PerformLayout();
            this.levelTabControl.ResumeLayout(false);
            this.grpTools.ResumeLayout(false);
            this.grpTools.PerformLayout();
            this.grpResources.ResumeLayout(false);
            this.panelResources.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.defaultImagePicBoxInFlowChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soundToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foregroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem midgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entitiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spawnersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGridlinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tilePainterToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enemyTypesToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogImages;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TabPage newLevel;
        private System.Windows.Forms.GroupBox grpTools;
        public System.Windows.Forms.Panel tilePanel;
        private System.Windows.Forms.Label lblScrollDebug;
        private System.Windows.Forms.ProgressBar levelLoadProgress;
        private System.Windows.Forms.GroupBox grpResources;
        private System.Windows.Forms.FlowLayoutPanel panelResources;
        private System.Windows.Forms.OpenFileDialog openFileDialogLevels;
        private System.Windows.Forms.VScrollBar vScrollBarLevel;
        private System.Windows.Forms.HScrollBar hScrollBarLevel;
        private System.Windows.Forms.ToolStripMenuItem openRecentToolStripMenuItem;
        private System.Windows.Forms.RadioButton rdoToolPainter;
        private System.Windows.Forms.PictureBox defaultImagePicBoxInFlowChart;
        public System.Windows.Forms.TabControl levelTabControl;
        private System.Windows.Forms.RadioButton rdoToolEnemyPlace;
        private System.Windows.Forms.RadioButton rdoToolHitTri;
        private System.Windows.Forms.RadioButton rdoToolHitBox;
    }
}

