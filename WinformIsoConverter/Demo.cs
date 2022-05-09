using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GomuLibrary.IO.DiscImage;

namespace WinformIsoConverter
{
    public partial class Demo : Form
    {
        Iso9660Conv isoConv = new Iso9660Conv();
        delegate void IsoConvProgressHandler(Iso9660CreatorEventArgs e);

        public Demo()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            isoConv.Terminate += new EventHandler(isoConv_Terminate);
            isoConv.Progression += new EventHandler<Iso9660CreatorEventArgs>(isoConv_Progression);
        }

        void isoConv_Progression(object sender, Iso9660CreatorEventArgs e)
        {
            IsoConvProgressHandler del = new IsoConvProgressHandler(DisplayProgress);

            if (this.InvokeRequired)
            {
                this.Invoke(del, new object[] { e });
            }
        }

        void DisplayProgress(Iso9660CreatorEventArgs value)
        {
            prgConversion.Maximum = Convert.ToInt32(value.DiscLength / 10);
            prgConversion.Value = Convert.ToInt32(value.BytesWritted / 10);
        }

        void isoConv_Terminate(object sender, EventArgs e)
        {
            MessageBox.Show(@"Conversion complete.");
        }

        private void lnkBrowse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.AddExtension = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "Bin image file (.bin)|*.bin|Nero image file (.nrg)|*.nrg|Alcohol image file (.mdf)|*.mdf|CloneCd image file (.img)|*.img|DiscJuggler file (.cdi)|*.cdi";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txbImageSrc.Text = ofd.FileName;

                    switch (ofd.FilterIndex)
                    {
                        case 1:
                            isoConv.DiscImage = new GomuLibrary.IO.DiscImage.Type.BinType(ofd.FileName);
                            break;
                        case 2:
                            isoConv.DiscImage = new GomuLibrary.IO.DiscImage.Type.NrgType(ofd.FileName);
                            break;
                        case 3:
                            isoConv.DiscImage = new GomuLibrary.IO.DiscImage.Type.MdfType(ofd.FileName);
                            break;
                        case 4:
                            isoConv.DiscImage = new GomuLibrary.IO.DiscImage.Type.CcdType(ofd.FileName);
                            break;
                        case 5:
                            isoConv.DiscImage = new GomuLibrary.IO.DiscImage.Type.CdiType(ofd.FileName);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void lnkIso_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.CheckPathExists = true;
                sfd.Filter = "Iso file (.iso)|*.iso";
                sfd.AddExtension = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    txtIso.Text = sfd.FileName;
                }
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(ThConvert);
            t.Name = "WinformIsoConverter";
            t.Start();
        }

        void ThConvert()
        {
            try
            {
                isoConv.Convert(txtIso.Text);
            }
            catch (Exception ex)
            {
                lblErrorMsg.Text = ex.Message;
            }
            finally
            {
                isoConv.Close();
            }
        }
    }
}