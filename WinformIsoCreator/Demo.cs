using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GomuLibrary.IO.DiscImage;

namespace WinformIsoCreator
{
    public partial class Demo : Form
    {
        delegate void ProgressionEventHandler(Iso9660CreatorEventArgs e);

        Iso9660Creator iso = new Iso9660Creator();

        public Demo()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            iso.Terminate += new EventHandler(iso_Terminate);
            iso.Aborted += new EventHandler<Iso9660CreatorEventArgs>(iso_Aborted);
            iso.Progression += new EventHandler<Iso9660CreatorEventArgs>(iso_Progression);

            System.IO.DriveInfo[] dis = System.IO.DriveInfo.GetDrives();

            for (int i = 0; i < dis.Length; i++)
            {
                if (dis[i].DriveType == System.IO.DriveType.CDRom)
                    cmbDrive.Items.Add(dis[i].Name);
            }

            if (cmbDrive.Items.Count > 0)
                cmbDrive.SelectedIndex = 0;
        }

        void iso_Progression(object sender, Iso9660CreatorEventArgs e)
        {
            if (this.InvokeRequired)
            {
                ProgressionEventHandler del = new ProgressionEventHandler(DisplayProgress);

                this.Invoke(del, e);
            }
        }

        void DisplayProgress(Iso9660CreatorEventArgs arg)
        {
            prgCreate.Value = Convert.ToInt32(arg.BytesWritted / 10);
        }

        void iso_Aborted(object sender, Iso9660CreatorEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            prgCreate.Maximum = Convert.ToInt32(new System.IO.DriveInfo(cmbDrive.SelectedItem.ToString()).TotalSize / 10);
            iso.CreateImage(cmbDrive.SelectedItem.ToString(), tbxSaveas.Text, true);
        }

        void iso_Terminate(object sender, EventArgs e)
        {
            MessageBox.Show("Terminate.");
        }

        private void lnkSaveas_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {

                sfd.CheckPathExists = true;
                sfd.Filter = "Iso File (.iso)|*.iso";
                sfd.DefaultExt = ".iso";
                sfd.AddExtension = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    tbxSaveas.Text = sfd.FileName;
                }
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            iso.Abort();
        }
    }
}