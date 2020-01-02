using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HGPS_Robot
{
    public partial class LessonSpeechUI : Form
    {
        private int _width, _height;
        private bool _paused = false;
        public LessonSpeechUI()
        {
            InitializeComponent();
        }
        private void Form2_Shown(object sender, EventArgs e)
        {
            var screen = Screen.PrimaryScreen.Bounds;
            _width = screen.Width;
            _height = screen.Height;

            this.Width = _width;
            this.Height = _height;
            //this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            picBackground.Controls.Add(lblMessage);
            lblMessage.Location = new Point(0, 300);
            lblMessage.AutoSize = false;
            lblMessage.Size = new Size(_width, _height - (lblMessage.Location.Y + 150));
            lblMessage.BackColor = Color.Transparent;
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            lblMessage.Text = "Lesson starting...";

        }

        public void ShowForm()
        {
            if (InvokeRequired)
            {
                Action action = new Action(() =>
                {
                    this.Show();
                });
                BeginInvoke(action);
            }
            else
            {
                this.Show();
            }
        }
        public void CloseForm()
        {
            if (InvokeRequired)
            {
                Action action = new Action(() =>
                {
                    this.Close();
                });
                BeginInvoke(action);
            }
            else
            {
                this.Close();
            }
        }
        public void ShowMessage(string message)
        {
            try
            {
                if (InvokeRequired)
                {
                    Action action = new Action(() =>
                    {
                        lblMessage.Text = message;
                    });
                    BeginInvoke(action);
                }
                else
                {
                    lblMessage.Text = message;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void PicRoboTA_Click(object sender, EventArgs e)
        //{
        //    if (_paused == false)
        //    {
        //        LessonHelper.Pause();
        //        _paused = true;

        //    }
        //    else if (_paused == true)
        //    {
        //        LessonHelper.Resume();
        //        _paused = false;
                
        //    }
        //}

        //private void picLogo_Click(object sender, EventArgs e)
        //{
        //    LessonHelper.ForceStop();
        //}
    }
}
