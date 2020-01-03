using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace HGPS_Robot
{
    public partial class InConversationUI : Form
    {
        private Timer checkingTimer = new Timer();
        public InConversationUI()
        {
            InitializeComponent();
            InitTimers();
        }
        private void InitTimers()
        {
            checkingTimer.Interval = 1000;
            checkingTimer.AutoReset = true;
            checkingTimer.Elapsed += CheckingTimer_Elapsed;
        }

        private void CheckingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (GlobalFlowControl.ChatBot.ConversationEnable == false)
            {
                checkingTimer.Stop();
                this.Invoke(new Action(() => this.Close()));
            }
        }

        private void InConversationUI_Shown(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            var area = Screen.FromControl(this).WorkingArea;
            picBackground.Location = new Point(0, 0);
            picBackground.Size = new Size(area.Width, area.Height);
            checkingTimer.Start();
            Conversation.Start();
        }
    }
}
