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
using SpeechLibrary;
using Timer = System.Timers.Timer;

namespace HGPS_Robot
{
    public partial class ChatbotUI : Form
    {
        private int waitingActivateCount;
        private Timer checkingTimer = new Timer();
        InConversationUI conversationUI = new InConversationUI();
        
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
                Task.Factory.StartNew(() => WaitForConversation());
            }   
        }

        public ChatbotUI()
        {
            InitializeComponent();
            InitTimers();
        }

        private void ChatbotUI_Shown(object sender, EventArgs e)
        {            

            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            var area = Screen.FromControl(this).WorkingArea;
            picBackground.Location = new Point(0, 0);
            picBackground.Size = new Size(area.Width, area.Height);
            picBackground.Controls.Add(picClose);

            picClose.Location = new Point(300, 820);
            picClose.Size = new Size(170, 200);
            picClose.BackColor = Color.Transparent;

            Task.Factory.StartNew(() => WaitForConversation());

        }

        private async Task WaitForConversation()
        {            
            waitingActivateCount = 0;
            do
            {
                await Recognizer.RecognizeKeywordWithTimeout(GlobalData.ActivationKeywords, 10)
                                    .ConfigureAwait(false);
                
                if (Recognizer.KeywordRecognized)
                {                    
                    checkingTimer.Start();
                    conversationUI.ShowDialog();               
                    return;
                }            
                waitingActivateCount++;                
                Debug.WriteLine("Not recognized count " + waitingActivateCount);
            } while (waitingActivateCount < 2);
            this.Invoke(new Action(() => this.Close()));
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChatbotUI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
