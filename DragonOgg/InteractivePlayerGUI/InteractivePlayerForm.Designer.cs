namespace InteractivePlayerGUI
{
    partial class InteractivePlayerForm
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
            this.buttonPlayGuitar = new System.Windows.Forms.Button();
            this.buttonPlayBoing = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxFile = new System.Windows.Forms.TextBox();
            this.buttonLoadFile = new System.Windows.Forms.Button();
            this.buttonPlayExternal = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPlayGuitar
            // 
            this.buttonPlayGuitar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPlayGuitar.Location = new System.Drawing.Point(12, 105);
            this.buttonPlayGuitar.Name = "buttonPlayGuitar";
            this.buttonPlayGuitar.Size = new System.Drawing.Size(115, 23);
            this.buttonPlayGuitar.TabIndex = 0;
            this.buttonPlayGuitar.Text = "Play Guitar";
            this.buttonPlayGuitar.UseVisualStyleBackColor = true;
            this.buttonPlayGuitar.Click += new System.EventHandler(this.buttonPlayGuitar_Click);
            // 
            // buttonPlayBoing
            // 
            this.buttonPlayBoing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlayBoing.Location = new System.Drawing.Point(262, 105);
            this.buttonPlayBoing.Name = "buttonPlayBoing";
            this.buttonPlayBoing.Size = new System.Drawing.Size(115, 23);
            this.buttonPlayBoing.TabIndex = 1;
            this.buttonPlayBoing.Text = "Play Boing";
            this.buttonPlayBoing.UseVisualStyleBackColor = true;
            this.buttonPlayBoing.Click += new System.EventHandler(this.buttonPlayBoing_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.buttonPlayExternal);
            this.groupBox1.Controls.Add(this.buttonLoadFile);
            this.groupBox1.Controls.Add(this.textBoxFile);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(365, 78);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "External Clip";
            // 
            // textBoxFile
            // 
            this.textBoxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFile.Location = new System.Drawing.Point(6, 19);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.ReadOnly = true;
            this.textBoxFile.Size = new System.Drawing.Size(298, 20);
            this.textBoxFile.TabIndex = 0;
            // 
            // buttonLoadFile
            // 
            this.buttonLoadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoadFile.Location = new System.Drawing.Point(319, 17);
            this.buttonLoadFile.Name = "buttonLoadFile";
            this.buttonLoadFile.Size = new System.Drawing.Size(40, 23);
            this.buttonLoadFile.TabIndex = 1;
            this.buttonLoadFile.Text = "...";
            this.buttonLoadFile.UseVisualStyleBackColor = true;
            this.buttonLoadFile.Click += new System.EventHandler(this.buttonLoadFile_Click);
            // 
            // buttonPlayExternal
            // 
            this.buttonPlayExternal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPlayExternal.Enabled = false;
            this.buttonPlayExternal.Location = new System.Drawing.Point(6, 45);
            this.buttonPlayExternal.Name = "buttonPlayExternal";
            this.buttonPlayExternal.Size = new System.Drawing.Size(115, 23);
            this.buttonPlayExternal.TabIndex = 2;
            this.buttonPlayExternal.Text = "Play";
            this.buttonPlayExternal.UseVisualStyleBackColor = true;
            this.buttonPlayExternal.Click += new System.EventHandler(this.buttonPlayExternal_Click);
            // 
            // InteractivePlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 140);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonPlayBoing);
            this.Controls.Add(this.buttonPlayGuitar);
            this.MinimumSize = new System.Drawing.Size(300, 178);
            this.Name = "InteractivePlayerForm";
            this.Text = "Interactive Ogg Player";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonPlayGuitar;
        private System.Windows.Forms.Button buttonPlayBoing;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonPlayExternal;
        private System.Windows.Forms.Button buttonLoadFile;
        private System.Windows.Forms.TextBox textBoxFile;
    }
}

