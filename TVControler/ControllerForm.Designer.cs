namespace TVControler
{
    partial class ControllerForm
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
            this.volumePlus = new System.Windows.Forms.Button();
            this.volumeMinus = new System.Windows.Forms.Button();
            this.playedPathLabel = new System.Windows.Forms.Label();
            this.stop = new System.Windows.Forms.Button();
            this.browseFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // volumePlus
            // 
            this.volumePlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.volumePlus.Location = new System.Drawing.Point(133, 54);
            this.volumePlus.Name = "volumePlus";
            this.volumePlus.Size = new System.Drawing.Size(115, 37);
            this.volumePlus.TabIndex = 0;
            this.volumePlus.Text = "Volume +";
            this.volumePlus.UseVisualStyleBackColor = true;
            this.volumePlus.Click += new System.EventHandler(this.volumePlus_Click);
            // 
            // volumeMinus
            // 
            this.volumeMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.volumeMinus.Location = new System.Drawing.Point(12, 54);
            this.volumeMinus.Name = "volumeMinus";
            this.volumeMinus.Size = new System.Drawing.Size(115, 37);
            this.volumeMinus.TabIndex = 0;
            this.volumeMinus.Text = "Volume -";
            this.volumeMinus.UseVisualStyleBackColor = true;
            this.volumeMinus.Click += new System.EventHandler(this.volumeMinus_Click);
            // 
            // playedPathLabel
            // 
            this.playedPathLabel.AutoSize = true;
            this.playedPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.playedPathLabel.Location = new System.Drawing.Point(12, 9);
            this.playedPathLabel.Name = "playedPathLabel";
            this.playedPathLabel.Size = new System.Drawing.Size(105, 24);
            this.playedPathLabel.TabIndex = 1;
            this.playedPathLabel.Text = "Played file..";
            // 
            // stop
            // 
            this.stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.stop.Location = new System.Drawing.Point(254, 54);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(93, 37);
            this.stop.TabIndex = 0;
            this.stop.Text = "STOP";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // browseFile
            // 
            this.browseFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.browseFile.Location = new System.Drawing.Point(12, 9);
            this.browseFile.Name = "browseFile";
            this.browseFile.Size = new System.Drawing.Size(335, 37);
            this.browseFile.TabIndex = 2;
            this.browseFile.Text = "Browse file";
            this.browseFile.UseVisualStyleBackColor = true;
            this.browseFile.Click += new System.EventHandler(this.browseFile_Click);
            // 
            // ControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 103);
            this.Controls.Add(this.browseFile);
            this.Controls.Add(this.playedPathLabel);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.volumeMinus);
            this.Controls.Add(this.volumePlus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ControllerForm";
            this.Text = "DLNA Player - version 0.3 (made by m9ra)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControllerForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button volumePlus;
        private System.Windows.Forms.Button volumeMinus;
        private System.Windows.Forms.Label playedPathLabel;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.Button browseFile;
    }
}