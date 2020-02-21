using System;
using System.Diagnostics;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace HGPS_Robot
{
    public partial class FrmGroupHint : Form
    {
        private string _offerHint;
        private int timerTick = 0;
        private Timer timer = new Timer();
        public FrmGroupHint()
        {
            InitializeComponent();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            lblTitle.Visible = false;
            btnNo.Visible = false;
            btnYes.Visible = false;
            lblHint.Visible = true;
            timerTick = 0;
            lblHint.Text = _offerHint;
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void Serve()
        {
            lblHint.Visible = false;
            timerTick = 0;
            timer.Start();
            _offerHint = GlobalFlowControl.GroupChallenge.OfferHint();
        }
        private void GroupChallengeHints_Shown(object sender, EventArgs e)
        {
            timer.Interval = 1000; // After 10 seconds, if don't choose
            // yes or no, => auto close
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine("Help? " + timerTick);
            timerTick++;
            if (timerTick == 10)
            {
                GlobalFlowControl.GroupChallenge.IsOfferingHint = false;
                Invoke(new Action(() => this.Hide()));
            }
            
        }
    }
}
