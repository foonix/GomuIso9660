namespace WinformIsoCreator
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
            this.btnCreate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbDrive = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lnkSaveas = new System.Windows.Forms.LinkLabel();
            this.tbxSaveas = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.prgCreate = new System.Windows.Forms.ProgressBar();
            this.btnAbort = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(267, 142);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 0;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbDrive);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 52);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // cmbDrive
            // 
            this.cmbDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDrive.FormattingEnabled = true;
            this.cmbDrive.Location = new System.Drawing.Point(48, 18);
            this.cmbDrive.Name = "cmbDrive";
            this.cmbDrive.Size = new System.Drawing.Size(276, 21);
            this.cmbDrive.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Drive:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lnkSaveas);
            this.groupBox2.Controls.Add(this.tbxSaveas);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(330, 66);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output";
            // 
            // lnkSaveas
            // 
            this.lnkSaveas.AutoSize = true;
            this.lnkSaveas.Location = new System.Drawing.Point(269, 40);
            this.lnkSaveas.Name = "lnkSaveas";
            this.lnkSaveas.Size = new System.Drawing.Size(46, 13);
            this.lnkSaveas.TabIndex = 2;
            this.lnkSaveas.TabStop = true;
            this.lnkSaveas.Text = "Select...";
            this.lnkSaveas.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSaveas_LinkClicked);
            // 
            // tbxSaveas
            // 
            this.tbxSaveas.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tbxSaveas.Location = new System.Drawing.Point(82, 17);
            this.tbxSaveas.Name = "tbxSaveas";
            this.tbxSaveas.ReadOnly = true;
            this.tbxSaveas.Size = new System.Drawing.Size(242, 20);
            this.tbxSaveas.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Output Iso file:";
            // 
            // prgCreate
            // 
            this.prgCreate.Location = new System.Drawing.Point(12, 178);
            this.prgCreate.Name = "prgCreate";
            this.prgCreate.Size = new System.Drawing.Size(330, 23);
            this.prgCreate.TabIndex = 3;
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(12, 142);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 4;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 211);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.prgCreate);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCreate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Demo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo - Create an ISO file from CD/DVD";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbDrive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel lnkSaveas;
        private System.Windows.Forms.TextBox tbxSaveas;
        private System.Windows.Forms.ProgressBar prgCreate;
        private System.Windows.Forms.Button btnAbort;
    }
}

