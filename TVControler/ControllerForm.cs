using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVControler
{
    public partial class ControllerForm : Form
    {
        private readonly Controller _controller;

        private readonly Queue<string> _filesToPlay = new Queue<string>();

        private DateTime? _imageStart;

        internal bool IsPlayInitiator
        {
            get
            {
                var info = _controller.CurrentInfo;
                if (info == null || _controller.LastUrlBase == null)
                    return false;

                var playedFile = info.PlayedFile;
                if (playedFile == null)
                    return false;

                return playedFile.StartsWith(_controller.LastUrlBase);
            }
        }

        public volatile bool IsClosed = false;

        public ControllerForm(string playPath)
        {
            InitializeComponent();
            ConsoleUtils.WriteLn(
                new Wr(ConsoleColor.Yellow, "Target {0}:{1}", SamsungTV.IP, SamsungTV.ControlPort)
            );
            
            _controller = new Controller(SamsungTV.IP, SamsungTV.ControlPort);
            displayFile(null);
        }

        private void reportConnectionError()
        {
            MessageBox.Show("Connection was not established. Check whether TV is turned on and you are connected to the network.", "Connection problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            displayFile(null);
        }

        private void browseFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            var videoFiles = "*.avi;*.mts;*.ts;*.mov;*.mp4;*.wmv;*.m2ts;*.mpg;*.mpeg;*.mkv";
            var imageFiles = "*.bmp;*.jpg;*.jpeg;*.png";
            dialog.Filter = "Compatible files|" + videoFiles + ";" + imageFiles + "|Video files|" + videoFiles + "|Image files|" + imageFiles;
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            dialog.Multiselect = true;

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                foreach (var filename in dialog.FileNames)
                    _filesToPlay.Enqueue(filename);

                playNextFile();
            }
        }

        private void playNextFile()
        {
            if (_filesToPlay.Count == 0)
                //nothing to play
                return;

            var nextFile = _filesToPlay.Dequeue();
            ConsoleUtils.WriteLn(
                new Wr(ConsoleColor.Green, "Playing file '{0}'", nextFile)
            );

            var extension = System.IO.Path.GetExtension(nextFile);
            switch (extension.ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".png":
                    _imageStart = DateTime.Now;
                    break;

            }

            if (nextFile != null)
                _controller.PlayFile(nextFile);
        }

        private void displayFile(string file)
        {
            if (file == null || file == "")
            {
                this.playedPathLabel.Visible = false;
                this.browseFile.Visible = true;
            }
            else
            {
                this.playedPathLabel.Visible = true;
                this.browseFile.Visible = false;

                var name = System.IO.Path.GetFileName(file);
                if (name.StartsWith("b64_"))
                {
                    var baseName = name.Substring(4).Replace('_', '=');
                    name = System.IO.Path.GetFileName(Encoding.UTF8.GetString(Convert.FromBase64String(baseName)));
                }

                this.playedPathLabel.Text = name;
            }
        }

        private void volumePlus_Click(object sender, EventArgs e)
        {
            _controller.IncrementVolume(+1);
        }

        private void volumeMinus_Click(object sender, EventArgs e)
        {
            _controller.IncrementVolume(-1);
        }

        private void stop_Click(object sender, EventArgs e)
        {
            _filesToPlay.Clear();
            _controller.Stop();
        }

        private void pause_Click(object sender, EventArgs e)
        {
            _controller.Pause();
        }

        private void form_Closing(object sender, FormClosingEventArgs e)
        {
            IsClosed = true;

            if (IsPlayInitiator)
            {
                _controller.Stop();
                System.Threading.Thread.Sleep(500);
                Application.Exit();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            var desiredPosition = timeBar.Value;
            previewPosition(desiredPosition);
        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            this.timer1.Stop();
            this.positionInfo.ForeColor = Color.Green;

            double dblValue;

            // Jump to the clicked location
            dblValue = ((double)e.X / (double)this.timeBar.Width) * (this.timeBar.Maximum - this.timeBar.Minimum);
            this.timeBar.Value = Convert.ToInt32(dblValue);
            previewPosition(this.timeBar.Value);
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            setPosition(timeBar.Value);
            this.timer1.Start();
        }

        private void previewPosition(int secondsFromStart)
        {
            var time = Controller.ToTimeStr(secondsFromStart);

            positionInfo.Text = time;
        }

        private void setPosition(int secondsFromStart)
        {
            _controller.SeekTo(secondsFromStart);
        }

        private void setEnabled(bool isEnabled)
        {
            timeBar.Enabled = isEnabled;
            volumeMinus.Enabled = isEnabled;
            volumePlus.Enabled = isEnabled;
            browseFile.Enabled = isEnabled;
            stop.Enabled = isEnabled;
            play.Enabled = isEnabled;
            pause.Enabled = isEnabled;

            if (!isEnabled)
                previewPosition(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var info = _controller.CurrentInfo;
            if (info == null)
            {
                if (!_controller.IsConnected)
                {
                    displayFile(null);
                    setEnabled(false);
                }
                this.stateInfo.Text = "OFFLINE";
                return;
            }

            if (_imageStart != null)
            {
                if (info.CurrentTransportState == "TRANSITIONING")
                    //Don't count transition time to presentation time.
                    _imageStart = DateTime.Now;

                var playNext = (DateTime.Now - _imageStart.Value).TotalSeconds > 5;
                if (playNext)
                {
                    _controller.Stop();
                    _imageStart = null;
                    playNextFile();
                }
            }

            if (info.CurrentTransportState == "PAUSED_PLAYBACK")
            {
                this.pause.Visible = false;
                this.play.Visible = true;
            }
            else
            {
                this.pause.Visible = true;
                this.play.Visible = false;
            }

            setEnabled(true);

            this.timeBar.Maximum = info.TotalDuration;
            this.timeBar.Value = info.ActualSecond;
            this.positionInfo.Text = Controller.ToTimeStr(info.ActualSecond);
            this.positionInfo.ForeColor = getStateColor(info.CurrentTransportState);
            this.stateInfo.Text = info.CurrentTransportState;
            this.stateInfo.ForeColor = getStateColor(info.CurrentTransportState);
            displayFile(info.PlayedFile);

        //    if (info.IsStopped)
        //        playNextFile();
        }

        private Color getStateColor(string transportState)
        {
            switch (transportState)
            {
                case "TRANSITIONING":
                    return Color.Red;

                default:
                    return Color.Black;
            }
        }

        private void play_Click(object sender, EventArgs e)
        {
            _controller.Play();
        }
    }
}
