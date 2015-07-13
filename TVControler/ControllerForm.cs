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

        public volatile bool IsClosed = false;

        public ControllerForm(string playPath)
        {
            InitializeComponent();

            ConsoleUtils.WriteLn(
                new Wr(ConsoleColor.Yellow, "Target {0}:{1}", SamsungTV.IP,SamsungTV.ControlPort)
            );
            _controller = new Controller(SamsungTV.IP, SamsungTV.ControlPort);

        }

        private void browseFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Video files|*.avi;*.mts;*.ts;*.mp4;*.wmv;*.m2ts;*.mpg;*.mpeg";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var file = dialog.FileName;
                ConsoleUtils.WriteLn(
                    new Wr(ConsoleColor.Green, "Playing file '{0}'", file)
                );

                _controller.PlayFile(file);
                showPath(file);
            }
        }

        private void showPath(string file)
        {
            if (file == null)
            {
                this.playedPathLabel.Visible = false;
                this.browseFile.Visible = true;
            }
            else
            {
                this.playedPathLabel.Visible = true;
                this.browseFile.Visible = false;

                var name = System.IO.Path.GetFileNameWithoutExtension(file);
                _controller.PlayFile(file);

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
            _controller.Stop();
            showPath(null);
        }

        private void ControllerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsClosed = true;
        }
    }
}
