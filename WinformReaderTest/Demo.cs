using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GomuLibrary.IO;
using GomuLibrary.IO.DiscImage;

namespace WinformReaderTest
{
    public partial class Demo : Form
    {
        SimpleIso9660Reader iso = new SimpleIso9660Reader();
        VolumeInfo volInfo = null;
        delegate void Iso9660FileExtractEventArgsHandler(Iso9660FileExtractEventArgs e);
        delegate void Iso9660EndEventArgsHandler();

        public Demo()
        {
            InitializeComponent();
        }

        private void ouvrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = @"Iso file (.iso)|*.iso|Bin file (.bin)|*.bin|Mdf file (.mdf)|*.mdf|CloneCd image file (.img)|*.img";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ImageFileFormat iff = new ImageFileFormat();
                    switch (ofd.FilterIndex)
                    {
                        case 1:
                            iff = ImageFileFormat.ISO;
                            break;
                        case 2:
                            iff = ImageFileFormat.BIN_Mode1;
                            break;
                        case 3:
                            iff = ImageFileFormat.MDF;
                            break;
                        case 4:
                            iff = ImageFileFormat.CCD_Mode1;
                            break;
                    }

                    volInfo = iso.Open(ofd.FileName, iff);
                    if (volInfo != null)
                    {
                        //Fill treeview.
                        treeViewTable1.SuspendLayout();
                        treeViewTable1.Nodes.Clear();

                        string[] paths = iso.GetPathsTable();
                        treeViewTable1.FillTreeviewPaths(paths, @"\");

                        treeViewTable1.ResumeLayout();

                        //Fill listview.
                        FillListview(@"\");

                        iso.Reading += new EventHandler<Iso9660FileExtractEventArgs>(iso_Reading);
                        iso.Terminate += new EventHandler(iso_Terminate);
                        iso.Aborted += new EventHandler<Iso9660FileExtractEventArgs>(iso_Aborted);
                    }
                }
            }
        }

        void iso_Aborted(object sender, Iso9660FileExtractEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Iso9660EndEventArgsHandler del = new Iso9660EndEventArgsHandler(EndExtract);
                this.Invoke(del);
            }

            MessageBox.Show("Extraction aborted !.");
        }

        void iso_Terminate(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Iso9660EndEventArgsHandler del = new Iso9660EndEventArgsHandler(EndExtract);
                this.Invoke(del);
            }

            MessageBox.Show("Extraction complete !.");
        }

        void iso_Reading(object sender, Iso9660FileExtractEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Iso9660FileExtractEventArgsHandler del = new Iso9660FileExtractEventArgsHandler(DisplayProgress);
                this.Invoke(del, e);
            }
        }

        private void DisplayProgress(Iso9660FileExtractEventArgs e)
        {
            labelSource.Text = e.File;
            progressBarFile.Maximum = Convert.ToInt32(e.Length / 10);
            progressBarFile.Value = Convert.ToInt32(e.Position / 10);
        }

        private void EndExtract()
        {
            panelProgress.Visible = false;
        }

        private void Demo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (iso != null)
                iso.Dispose();
        }

        private void volInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (VolumeInfos fVolinfo = new VolumeInfos())
            {
                if (this.volInfo != null)
                {
                    fVolinfo._volInfo = this.volInfo;
                    fVolinfo.ShowDialog();
                }
            }
        }

        private void treeViewTable1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FillListview(treeViewTable1.SelectedNode.FullPath);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fullExtractDuVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    panelProgress.Visible = true;
                    iso.ExtractFullImage(fbd.SelectedPath, true);
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            //Abort extraction.
            iso.Abort();
        }

        private void extractFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                if (listView1.SelectedItems[0].ImageIndex == 1)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            panelProgress.Visible = true;

                            string szPath = treeViewTable1.SelectedNode.FullPath;
                            if (!szPath.StartsWith(@"\"))
                                szPath = szPath.Insert(0, @"\");

                            string szFullPath = System.IO.Path.Combine(szPath, listView1.SelectedItems[0].Text);

                            iso.ExtractFile(szFullPath, fbd.SelectedPath, true);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You must select a file !.");
                }
            }
        }

        private void FillListview(string path)
        {
            string[] dirs = new string[] { };
            string[] files = new string[] { };

            if (path != @"\")
            {
                dirs = iso.GetDirectories(path.StartsWith(@"\") ? path : @"\" + path,
                     System.IO.SearchOption.TopDirectoryOnly);
            }
            else
            {
                dirs = iso.GetDirectories(path, System.IO.SearchOption.TopDirectoryOnly);
            }

            listView1.SuspendLayout();
            listView1.Items.Clear();

            for (int i = 0; i < dirs.Length; i++)
            {
                listView1.Items.Add(System.IO.Path.GetFileName(dirs[i]));
                listView1.Items[listView1.Items.Count - 1].ImageIndex = 0;
            }

            if (path != @"\")
            {
                files = iso.GetFiles(path.StartsWith(@"\") ? path : @"\" + path,
                     System.IO.SearchOption.TopDirectoryOnly);
            }
            else
            {
                files = iso.GetFiles(path, System.IO.SearchOption.TopDirectoryOnly);
            }

            for (int i = 0; i < files.Length; i++)
            {
                listView1.Items.Add(System.IO.Path.GetFileName(files[i]));
                listView1.Items[listView1.Items.Count - 1].ImageIndex = 1;
            }

            listView1.ResumeLayout();
        }
    }
}