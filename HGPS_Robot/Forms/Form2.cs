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
    public partial class Form2 : Form
    {
        private int _width, _height;
        private bool _paused = false;
        public Form2()
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
            //this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            lblMessage.Location = new Point(0, 0);
            lblMessage.Width = _width;
            lblMessage.Height = _height;
            lblMessage.Text = "Lesson starting...";

            picLogo.Location = new Point(_width - picLogo.Width, 0);
            picRoboTA.Location = new Point(0, 0);
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

        private void PicRoboTA_Click(object sender, EventArgs e)
        {
            if (_paused == false)
            {
                LessonHelper.Pause();
                _paused = true;

            }
            else if (_paused == true)
            {
                LessonHelper.Resume();
                _paused = false;
                
            }
        }

        private void picLogo_Click(object sender, EventArgs e)
        {
            LessonHelper.ForceStop();
        }
    }
}
