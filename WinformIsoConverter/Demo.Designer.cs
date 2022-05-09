namespace WinformIsoConverter
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lnkIso = new System.Windows.Forms.LinkLabel();
            this.txtIso = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lnkBrowse = new System.Windows.Forms.LinkLabel();
            this.txbImageSrc = new System.Windows.Forms.TextBox();
            this.lblSource = new System.Windows.Forms.Label();
            this.prgConversion = new System.Windows.Forms.ProgressBar();
            this.btnConvert = new System.Windows.Forms.Button();
            this.lblErrorMsg = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lnkIso);
            this.groupBox1.Controls.Add(this.txtIso);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lnkBrowse);
            this.groupBox1.Controls.Add(this.txbImageSrc);
            this.groupBox1.Controls.Add(this.lblSource);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(491, 78);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // lnkIso
            // 
            this.lnkIso.AutoSize = true;
            this.lnkIso.Location = new System.Drawing.Point(443, 52);
            this.lnkIso.Name = "lnkIso";
            this.lnkIso.Size = new System.Drawing.Size(37, 13);
            this.lnkIso.TabIndex = 5;
            this.lnkIso.TabStop = true;
            this.lnkIso.Text = "Select";
            this.lnkIso.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkIso_LinkClicked);
            // 
            // txtIso
            // 
            this.txtIso.BackColor = System.Drawing.SystemColors.Window;
            this.txtIso.Location = new System.Drawing.Point(109, 49);
            this.txtIso.Name = "txtIso";
            this.txtIso.ReadOnly = true;
            this.txtIso.Size = new System.Drawing.Size(328, 20);
            this.txtIso.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Iso destination file :";
            // 
            // lnkBrowse
            // 
            this.lnkBrowse.AutoSize = true;
            this.lnkBrowse.Location = new System.Drawing.Point(443, 21);
            this.lnkBrowse.Name = "lnkBrowse";
            this.lnkBrowse.Size = new System.Drawing.Size(37, 13);
            this.lnkBrowse.TabIndex = 2;
            this.lnkBrowse.TabStop = true;
            this.lnkBrowse.Text = "Select";
            this.lnkBrowse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBrowse_LinkClicked);
            // 
            // txbImageSrc
            // 
            this.txbImageSrc.BackColor = System.Drawing.SystemColors.Window;
            this.txbImageSrc.Location = new System.Drawing.Point(109, 18);
            this.txbImageSrc.Name = "txbImageSrc";
            this.txbImageSrc.ReadOnly = true;
            this.txbImageSrc.Size = new System.Drawing.Size(328, 20);
            this.txbImageSrc.TabIndex = 1;
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(8, 21);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(93, 13);
            this.lblSource.TabIndex = 0;
            this.lblSource.Text = "Image source file :";
            // 
            // prgConversion
            // 
            this.prgConversion.Location = new System.Drawing.Point(12, 96);
            this.prgConversion.Name = "prgConversion";
            this.prgConversion.Size = new System.Drawing.Size(491, 20);
            this.prgConversion.TabIndex = 1;
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(422, 122);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(81, 23);
            this.btnConvert.TabIndex = 2;
            this.btnConvert.Text = "Convert now !";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // lblErrorMsg
            // 
            this.lblErrorMsg.AutoSize = true;
            this.lblErrorMsg.Location = new System.Drawing.Point(12, 132);
            this.lblErrorMsg.Name = "lblErrorMsg";
            this.lblErrorMsg.Size = new System.Drawing.Size(0, 13);
            this.lblErrorMsg.TabIndex = 3;
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 148);
            this.Controls.Add(this.lblErrorMsg);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.prgConversion);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Demo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iso Converter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar prgConversion;
        private System.Windows.Forms.TextBox txbImageSrc;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.LinkLabel lnkBrowse;
        private System.Windows.Forms.TextBox txtIso;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.LinkLabel lnkIso;
        private System.Windows.Forms.Label lblErrorMsg;
    }
}

