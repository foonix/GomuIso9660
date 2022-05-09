namespace WinformReaderTest
{
    partial class Demo
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Demo));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.volInfosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullExtractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panelProgress = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBarFile = new System.Windows.Forms.ProgressBar();
            this.labelSource = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.treeViewTable1 = new WinformReaderTest.UC.TreeViewTable();
            this.menuStrip1.SuspendLayout();
            this.panelProgress.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.infosToolStripMenuItem,
            this.extractionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(828, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.toolStripMenuItem1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.OpenToolStripMenuItem.Text = "Open";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.ouvrirToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(137, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // infosToolStripMenuItem
            // 
            this.infosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.volInfosToolStripMenuItem});
            this.infosToolStripMenuItem.Name = "infosToolStripMenuItem";
            this.infosToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.infosToolStripMenuItem.Text = "Infos";
            // 
            // volInfosToolStripMenuItem
            // 
            this.volInfosToolStripMenuItem.Name = "volInfosToolStripMenuItem";
            this.volInfosToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.volInfosToolStripMenuItem.Text = "Volume infos";
            this.volInfosToolStripMenuItem.Click += new System.EventHandler(this.volInfosToolStripMenuItem_Click);
            // 
            // extractionToolStripMenuItem
            // 
            this.extractionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractFileToolStripMenuItem,
            this.fullExtractToolStripMenuItem});
            this.extractionToolStripMenuItem.Name = "extractionToolStripMenuItem";
            this.extractionToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.extractionToolStripMenuItem.Text = "Extraction";
            // 
            // extractFileToolStripMenuItem
            // 
            this.extractFileToolStripMenuItem.Name = "extractFileToolStripMenuItem";
            this.extractFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.extractFileToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.extractFileToolStripMenuItem.Text = "Extract selected file";
            this.extractFileToolStripMenuItem.Click += new System.EventHandler(this.extractFileToolStripMenuItem_Click);
            // 
            // fullExtractToolStripMenuItem
            // 
            this.fullExtractToolStripMenuItem.Name = "fullExtractToolStripMenuItem";
            this.fullExtractToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.fullExtractToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.fullExtractToolStripMenuItem.Text = "Extract full Volume";
            this.fullExtractToolStripMenuItem.Click += new System.EventHandler(this.fullExtractDuVolumeToolStripMenuItem_Click);
            // 
            // listView1
            // 
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(296, 24);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(532, 472);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.png");
            this.imageList1.Images.SetKeyName(1, "page.png");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 498);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(828, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // panelProgress
            // 
            this.panelProgress.Controls.Add(this.buttonCancel);
            this.panelProgress.Controls.Add(this.groupBox1);
            this.panelProgress.Location = new System.Drawing.Point(143, 159);
            this.panelProgress.Name = "panelProgress";
            this.panelProgress.Size = new System.Drawing.Size(486, 121);
            this.panelProgress.TabIndex = 5;
            this.panelProgress.Visible = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(195, 87);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Abort";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBarFile);
            this.groupBox1.Controls.Add(this.labelSource);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 78);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // progressBarFile
            // 
            this.progressBarFile.Location = new System.Drawing.Point(9, 48);
            this.progressBarFile.Name = "progressBarFile";
            this.progressBarFile.Size = new System.Drawing.Size(455, 20);
            this.progressBarFile.TabIndex = 2;
            // 
            // labelSource
            // 
            this.labelSource.Location = new System.Drawing.Point(85, 16);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(379, 28);
            this.labelSource.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source file:";
            // 
            // treeViewTable1
            // 
            this.treeViewTable1.ImageIndex = 0;
            this.treeViewTable1.ImageList = this.imageList1;
            this.treeViewTable1.Location = new System.Drawing.Point(0, 24);
            this.treeViewTable1.Name = "treeViewTable1";
            this.treeViewTable1.SelectedImageIndex = 0;
            this.treeViewTable1.Size = new System.Drawing.Size(290, 472);
            this.treeViewTable1.TabIndex = 3;
            this.treeViewTable1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTable1_AfterSelect);
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 520);
            this.Controls.Add(this.panelProgress);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.treeViewTable1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Demo";
            this.Text = "Test - ISO reader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Demo_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelProgress.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem volInfosToolStripMenuItem;
        private System.Windows.Forms.ListView listView1;
        private WinformReaderTest.UC.TreeViewTable treeViewTable1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullExtractToolStripMenuItem;
        private System.Windows.Forms.Panel panelProgress;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar progressBarFile;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.Label label1;
    }
}

