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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControllerForm));
            this.volumePlus = new System.Windows.Forms.Button();
            this.volumeMinus = new System.Windows.Forms.Button();
            this.playedPathLabel = new System.Windows.Forms.Label();
            this.stop = new System.Windows.Forms.Button();
            this.browseFile = new System.Windows.Forms.Button();
            this.timeBar = new System.Windows.Forms.TrackBar();
            this.positionInfo = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.stateInfo = new System.Windows.Forms.Label();
            this.pause = new System.Windows.Forms.Button();
            this.play = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).BeginInit();
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
            this.stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.stop.Location = new System.Drawing.Point(255, 54);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(49, 37);
            this.stop.TabIndex = 0;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // browseFile
            // 
            this.browseFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.browseFile.Location = new System.Drawing.Point(12, 9);
            this.browseFile.Name = "browseFile";
            this.browseFile.Size = new System.Drawing.Size(370, 37);
            this.browseFile.TabIndex = 2;
            this.browseFile.Text = "Browse file";
            this.browseFile.UseVisualStyleBackColor = true;
            this.browseFile.Click += new System.EventHandler(this.browseFile_Click);
            // 
            // timeBar
            // 
            this.timeBar.LargeChange = 0;
            this.timeBar.Location = new System.Drawing.Point(12, 104);
            this.timeBar.Maximum = 0;
            this.timeBar.Name = "timeBar";
            this.timeBar.Size = new System.Drawing.Size(624, 45);
            this.timeBar.TabIndex = 3;
            this.timeBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.timeBar.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.timeBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseDown);
            this.timeBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // positionInfo
            // 
            this.positionInfo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.positionInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.positionInfo.Location = new System.Drawing.Point(388, 28);
            this.positionInfo.Name = "positionInfo";
            this.positionInfo.Size = new System.Drawing.Size(248, 73);
            this.positionInfo.TabIndex = 4;
            this.positionInfo.Text = "0:00:00";
            this.positionInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // stateInfo
            // 
            this.stateInfo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.stateInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.stateInfo.Location = new System.Drawing.Point(406, 5);
            this.stateInfo.Name = "stateInfo";
            this.stateInfo.Size = new System.Drawing.Size(216, 24);
            this.stateInfo.TabIndex = 4;
            this.stateInfo.Text = "STOPPED";
            this.stateInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pause
            // 
            this.pause.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.pause.Location = new System.Drawing.Point(310, 54);
            this.pause.Name = "pause";
            this.pause.Size = new System.Drawing.Size(72, 37);
            this.pause.TabIndex = 5;
            this.pause.Text = "Pause";
            this.pause.UseVisualStyleBackColor = true;
            this.pause.Click += new System.EventHandler(this.pause_Click);
            // 
            // play
            // 
            this.play.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.play.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.play.Location = new System.Drawing.Point(310, 54);
            this.play.Name = "play";
            this.play.Size = new System.Drawing.Size(72, 37);
            this.play.TabIndex = 6;
            this.play.Text = "Play";
            this.play.UseVisualStyleBackColor = true;
            this.play.Click += new System.EventHandler(this.play_Click);
            // 
            // ControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 127);
            this.Controls.Add(this.play);
            this.Controls.Add(this.pause);
            this.Controls.Add(this.stateInfo);
            this.Controls.Add(this.positionInfo);
            this.Controls.Add(this.timeBar);
            this.Controls.Add(this.browseFile);
            this.Controls.Add(this.playedPathLabel);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.volumeMinus);
            this.Controls.Add(this.volumePlus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ControllerForm";
            this.Text = "TV Controller - version 1.51 (made by m9ra)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControllerForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button volumePlus;
        private System.Windows.Forms.Button volumeMinus;
        private System.Windows.Forms.Label playedPathLabel;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.Button browseFile;
        private System.Windows.Forms.TrackBar timeBar;
        private System.Windows.Forms.Label positionInfo;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label stateInfo;
        private System.Windows.Forms.Button pause;
        private System.Windows.Forms.Button play;
    }
}