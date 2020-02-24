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
        private string _offerHint;
        private int timerTick = 0;
        private Timer timer = new Timer();
        private bool hintShown = false;
        private const int HINT_TIMEOUT = 8;
        public LessonSpeechUI()
        {
            InitializeComponent();
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
        }
        
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine("Help? " + timerTick);
            timerTick++;
            if (timerTick == HINT_TIMEOUT)
            {
                GlobalFlowControl.GroupChallenge.IsOfferingHint = false;
                ShowMessage("GROUP CHALLENGE"); // Reset label
                timer.Stop();
            }
        }

        public void OfferHint()
        {
            ShowMessage("TOUCH ME TO SEE THE HINT");
            hintShown = false;
            timerTick = 0;
            timer.Start();
            _offerHint = GlobalFlowControl.GroupChallenge.OfferHint();
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            //this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            var area = Screen.FromControl(this).WorkingArea;
            picBackground.Location = new Point(0, 0);
            picBackground.Size = new Size(area.Width, area.Height);
            picBackground.Controls.Add(lblMessage);
            lblMessage.Location = new Point(0, 300);
            lblMessage.AutoSize = false;
            lblMessage.Size = new Size(area.Width-100, area.Height - (lblMessage.Location.Y + 150));
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

        private void picBackground_Click(object sender, EventArgs e)
        {
            if (hintShown == false && 
                GlobalFlowControl.GroupChallenge.IsOfferingHint == true)
            {
                hintShown = true;
                timerTick = 0;
                ShowMessage(_offerHint);
            }
        }

        private void lblMessage_Click(object sender, EventArgs e)
        {
            if (hintShown == false &&
                GlobalFlowControl.GroupChallenge.IsOfferingHint == true)
            {
                hintShown = true;
                timerTick = 0;
                ShowMessage(_offerHint);
            }
           
        }

    }
}
