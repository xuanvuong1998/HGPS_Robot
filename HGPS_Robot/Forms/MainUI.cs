using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpeechLibrary;

namespace HGPS_Robot
{
    public partial class MainUI : Form
    {
        LessonUI lessonUI = new LessonUI();
        ChatbotUI chatbotUI = new ChatbotUI();

        public MainUI()
        {
            InitializeComponent();
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            var area = Screen.FromControl(this).WorkingArea;
            picBackground.Location = new Point(0, 0);
            picBackground.Size = new Size(area.Width, area.Height);

            SystemUpdateHelper.Start();
            SystemUpdateHelper.SystemUpdated += SystemUpdateHelper_SystemUpdated;

            InitSpeechAndChatbot();

            UpperBodyHelper.Init();
            BaseHelper.Connect();
        }

        private void SystemUpdateHelper_SystemUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    picBackground.Image = Properties.Resources.CoddieMainUI;
                    picBackground.Controls.Add(picLesson);
                    picBackground.Controls.Add(picTalk);

                    var btnSize = new Size(520, 150);

                    picLesson.Location = new Point(120, 620);
                    picLesson.Size = btnSize;
                    picLesson.BackColor = Color.Transparent;
                    picLesson.Visible = true;

                    picTalk.Location = new Point(120, 780);
                    picTalk.Size = btnSize;
                    picTalk.BackColor = Color.Transparent;
                    picTalk.Visible = true;
                    return;
                }));
            }
            
        }

        private void picLesson_Click(object sender, EventArgs e)
        {
            lessonUI.ShowDialog();
        }

        private void picTalk_Click(object sender, EventArgs e)
        {
            chatbotUI.ShowDialog();
        }

        private void InitSpeechAndChatbot()
        {
            Synthesizer.Setup();
            Synthesizer.SelectVoiceByName(GlobalData.Voice1);
            Conversation.Init();
        }

        private void MainUI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chatbotUI.ShowDialog();
            }
            if (e.KeyCode == Keys.Space)
            {
                lessonUI.ShowDialog();
            }
        }
    }
}
