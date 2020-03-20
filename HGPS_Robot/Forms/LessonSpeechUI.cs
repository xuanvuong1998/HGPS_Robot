using SpeechLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace HGPS_Robot
{
    public partial class LessonSpeechUI : Form
    {
        private string hintImagePath;
        private Timer timer = new Timer();
        private bool hintShown = false;
        private const int HINT_TIMEOUT = 30 * 1000; // 30 seconds, hints will hided
        private Image defaultBackgroundImage;

        public LessonSpeechUI()
        {
            InitializeComponent();
            defaultBackgroundImage = picBackground.Image;
            timer.Interval = HINT_TIMEOUT;
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
        }

        private void ResetLabel()
        {
            ShowMessage("GROUP CHALLENGE"); // Reset label
            picBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            picBackground.Image = defaultBackgroundImage;
        }

        
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GlobalFlowControl.GroupChallenge.IsOfferingHint = false;
            ResetLabel();
        }

        public void OfferHint()
        {
            var top = GlobalFlowControl.GroupChallenge.GetNextOffer();
            GlobalFlowControl.GroupChallenge.RemoveCurrentOffer();

            var groupNum = int.Parse(top.Split('-')[0]);

            BaseHelper.GoUntilReachedGoalOrCanceled("A" + groupNum);
            Synthesizer.SetVolume(30);
            int rdmNum = new Random().Next(3);

            switch (rdmNum)
            {
                case 0:
                    Synthesizer.SpeakAsync("Group " + groupNum + ". Do you " +
             "need a hint for this challenge? You can touch the screen to show it. "); break;
                case 1:
                    Synthesizer.SpeakAsync("Group " + groupNum + ". Please touch me if " +
                        "you need help about this challenge "); break;
                case 2:
                    Synthesizer.SpeakAsync("Group " + groupNum + ". Do you need " +
                        "any help? Why don't try to touch me to receive a hint"); break;
            }

            Synthesizer.SetVolume(100);
            hintImagePath = FileHelper.GetHintFolderPath() + top.Split('-')[1];

            ShowMessage("TOUCH ME TO SEE THE HINT");
            hintShown = false;
            timer.Start();

        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            var area = Screen.FromControl(this).WorkingArea;
            picBackground.Location = new Point(0, 0);
            picBackground.Size = new Size(area.Width, area.Height);
            picBackground.Controls.Add(lblMessage);
            lblMessage.Location = new Point(0, 300);
            lblMessage.AutoSize = false;
            lblMessage.Size = new Size(area.Width - 100, area.Height - (lblMessage.Location.Y + 150));
            lblMessage.BackColor = Color.Transparent;
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            lblMessage.Text = "Lesson starting...";

            picBackground.Controls.Add(lblMessage);
            lblMessage.Location = new Point(50, 150);
            lblMessage.BackColor = Color.Transparent;


           
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

        private void ShowHintImage()
        {
            lblMessage.Text = "";

            picBackground.SizeMode = PictureBoxSizeMode.Zoom;

            //MessageBox.Show(hintImagePath);

            picBackground.Image = Image.FromFile(hintImagePath); 
        }
        
        private void picBackground_Click(object sender, EventArgs e)
        {
            if (hintShown == false &&
                GlobalFlowControl.GroupChallenge.IsOfferingHint == true)
            {
                hintShown = true;

                ShowHintImage();
            }
        }

        private void lblMessage_Click(object sender, EventArgs e)
        {
            if (hintShown == false &&
                GlobalFlowControl.GroupChallenge.IsOfferingHint == true)
            {
                hintShown = true;
                ShowHintImage();
            }

        }

    }
}
