using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinformReaderTest
{
    public partial class VolumeInfos : Form
    {
        public VolumeInfos()
        {
            InitializeComponent();
        }

        internal GomuLibrary.IO.DiscImage.VolumeInfo _volInfo = null;

        private void VolumeInfos_Load(object sender, EventArgs e)
        {
            if (_volInfo != null)
                propertyGrid1.SelectedObject = _volInfo;
        }
    }
}